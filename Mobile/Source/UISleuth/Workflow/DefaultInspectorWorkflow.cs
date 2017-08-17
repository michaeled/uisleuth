using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reflection;
using UISleuth.Reactions;

namespace UISleuth.Workflow
{
    /// <summary>
    /// Responsible for dispatching client messages to the appropriate <see cref="Reaction"/>.
    /// </summary>
    internal sealed class DefaultInspectorWorkflow : InspectorWorkflow
    {
        public int ConsumerThreads { get; private set; }
        public int QueuedMessages => _queue.Count;

        private InspectorSocket _socket;
        private IUIMessageFinder _messageFinder;
        private CancellationTokenSource _cancellationSource = new CancellationTokenSource();
        private BlockingCollection<string> _queue = new BlockingCollection<string>();
        private Task _consumerThread;
        private const int AddWaitTimeMs = 500;
        private const int QueueDelayTimeMs = 250;
        private readonly object _lock = new object();


        /// <summary>
        /// Queue a message to be executed.
        /// </summary>
        /// <param name="message"></param>
        public override void Queue(string message)
        {
            if (!_queue.IsCompleted)
            {
                _queue.TryAdd(message, AddWaitTimeMs, _cancellationSource.Token);
            }
        }


        /// <summary>
        /// Start the workflow engine.
        /// </summary>
        public override void Start(IUIMessageFinder messageFinder, InspectorSocket server)
        {
            _messageFinder = messageFinder;
            _socket = server;

            _cancellationSource = new CancellationTokenSource();
            _queue = new BlockingCollection<string>();

            // create 1 new consumer thread
            if (_consumerThread == null)
            {
                Task.Run(() => Consumer());
            }
        }


        /// <summary>
        /// Stop the workflow and reset all internal settings.
        /// </summary>
        public override void Shutdown()
        {
            lock (_lock)
            {
                ConsumerThreads--;
            }

            _queue.CompleteAdding();
            _cancellationSource.Cancel();
            _consumerThread = null;
        }


        /// <summary>
        /// Execute the queued message.
        /// </summary>
        private void Consumer()
        {
            try
            {
                lock (_lock)
                {
                    ConsumerThreads++;
                }

                foreach (var message in _queue.GetConsumingEnumerable(_cancellationSource.Token))
                {
                    if (_queue.IsCompleted) return;

                    var request = Parse(message);
                    // skip null request
                    if (request == null) continue;

                    var context = new UIMessageContext
                    {
                        Message = message,
                        ShouldQueue = false,
                        Request = request,
                        Response = null
                    };

                    ExecuteDesignAction(context);

                    // add it back for later processing
                    if (context.ShouldQueue)
                    {
                        _queue.TryAdd(message, AddWaitTimeMs, _cancellationSource.Token);
                        Task.Delay(QueueDelayTimeMs).Wait();
                    }
                    else
                    {
                        try
                        {
                            var json = context.Response?.ToJson();

                            if (!string.IsNullOrWhiteSpace(json))
                            {
                                _socket.Send(json);
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }

                    context.Response = null;
                }
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
        }


        /// <summary>
        /// Return the corresponding UIMessage.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private UIMessage Parse(string message)
        {
            UIMessage request = null;

            if (!string.IsNullOrWhiteSpace(message))
            {
               UIMessage.TryParseFromAction(_messageFinder, message, out request);
            }

            return request;
        }


        private void ExecuteDesignAction(UIMessageContext ctx)
        {
            // Send request to designer.
            // A true response does not guarantee that a design action set the response object.
            // A false response means that an action hasn't been registerd to handle the request.

            bool executed;
            var unhandledException = false;
            string exceptionMessage = null;

            try
            {
                executed = Reaction.Execute(ctx);
                ctx.ShouldQueue = !executed;
            }
            catch (Exception ex)
            {
                executed = true;
                ctx.ShouldQueue = false;
                unhandledException = true;
                exceptionMessage = ex.ToString();
            }
            
            if (executed)
            {
                // If a specific response hasn't been created for a request type, send an OkResponse to the client.
                if (ctx.Response == null || ctx.Response is UnknownMessage)
                {
                    ctx.SetResponse<OkResponse>(r =>
                    {
                        r.ReplyingTo = ctx.Request.MessageId;
                    });
                }

                var response = ctx.Response as Response;

                if (response != null)
                {
                    response.UnhandledExceptionOccurred = unhandledException;
                    response.ExceptionMessage = exceptionMessage;
                }
            }
        }
    }
}