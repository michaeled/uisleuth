using System;
using System.Collections.Generic;
using System.Diagnostics;
using UISleuth.Workflow;
using UISleuth.Networking;
using UISleuth.Reflection;
using UISleuth.Messages;
using Xamarin.Forms;

namespace UISleuth
{
    internal static class InspectorBootstrapper
    {
        internal const string AppName = "UI Sleuth";

        private static InspectorSocket _socket;
        private static InspectorWorkflow _workflow;
        private static IUIMessageFinder _messageFinder;
        private static ITypeFinder _typeFinder;
        private static INetworkAdapter _networkAdapter;
        private static InspectorReactionRegistrar _reactionRegistrar;
        private static PageMonitor _pageMonitor;

        private static short _mobilePort;
        private static bool _initialized;
        private static bool _loggingEnabled = true;

        /*
        internal static void Init(string clientId, bool loggingEnabled)
        {
            if (_initialized) return;

            _loggingEnabled = loggingEnabled;

            InitDependencies();
            InitWorkflow();
            ConnectToRemoteServer(clientId);

            _initialized = true;
        }
        */

        internal static void Init(short mobilePort, bool loggingEnabled)
        {
            if (_initialized) return;

            if (!ServiceEndpoint.IsValidPort(mobilePort))
            {
                throw new InvalidOperationException("The mobilePort must be 1024 or above.");
            }

            _loggingEnabled = loggingEnabled;
            _mobilePort = mobilePort;

            InitDependencies();
            InitWorkflow();
            StartListening();

            _initialized = true;
        }


        internal static void ShowAcceptingConnections()
        {
            if (Application.Current?.MainPage != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert(AppName, $"{GetListeningMessage()}", "OK");
                });
            }
        }


        private static void InitDependencies()
        {
            if (InspectorContainer.Current == null)
            {
                throw new InvalidOperationException("The inspector container has not been initialized.");
            }

            _networkAdapter = InspectorContainer.Current.Resolve<INetworkAdapter>();
            _messageFinder = InspectorContainer.Current.Resolve<IUIMessageFinder>();
            _socket = InspectorContainer.Current.Resolve<InspectorSocket>();
            _workflow = InspectorContainer.Current.Resolve<InspectorWorkflow>();
            _typeFinder = InspectorContainer.Current.Resolve<ITypeFinder>();
            _reactionRegistrar = new InspectorReactionRegistrar(_typeFinder);

            _pageMonitor = new PageMonitor(TimeSpan.FromSeconds(1.5), Application.Current.MainPage);
        }


        private static void InitWorkflow()
        {
            _reactionRegistrar.Init(_pageMonitor.CurrentPage);

            _socket.Message += (sender, args) =>
            {
                _workflow.Queue(args.Message);
            };

            _socket.SocketStarted += (sender, args) =>
            {
                _workflow.Start(_messageFinder, _socket);
            };

            _pageMonitor.PageChanged += (sender, args) =>
            {
                _reactionRegistrar.Init(args.NewPage);
                var msg = UIMessage.Create<PageChanged>().ToJson();
                _socket.Send(msg);
            };

            if (_loggingEnabled)
            {
                _socket.Trace += (sender, args) =>
                {
                    Debug.WriteLine($"{AppName}: {args.Description}");
                };
            }

            _pageMonitor.Start();
        }


        private static void StartListening()
        {
            if (!_socket.IsListening)
            {
                _socket.Listen(_mobilePort);

                if (_loggingEnabled)
                {
                    Debug.WriteLine($"{AppName}: {GetListeningMessage()}");
                }
            }
        }


        /*
        private static void ConnectToRemoteServer(string clientId)
        {
            if (!_socket.IsConnectedToRemoteServer)
            {
                _socket.Connect($"ws://uisleuth.azurewebsites.net/api/pair?clientKey={clientId}::mobile");
                Debug.WriteLine($"{AppName}: Connected to remote server.");
            }
        }
        */

        private static string GetListeningMessage()
        {
            var addresses = _networkAdapter.GetIpAddresses();

            if (addresses.Length == 0)
            {
                return "Error. No IP addresses are available. Please join your device to a network.";
            }

            var formatted = new List<string>();

            foreach (var address in addresses)
            {
                formatted.Add(ServiceEndpoint.FormatAddress(address, _mobilePort));
            }

            return "Listening on " + string.Join(", ", formatted.ToArray());
        }
    }
}