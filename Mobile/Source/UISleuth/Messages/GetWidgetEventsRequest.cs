using UISleuth.Widgets;

namespace UISleuth.Messages
{
    /// <summary>
    /// The client has requested that all events for the <see cref="UIWidget"/> should be returned.
    /// </summary>
    internal class GetWidgetEventsRequest : Request, IWidgetMessage
    {
        /// <summary>
        /// The widget's <see cref="UIWidget.Id"/>.
        /// </summary>
        public string WidgetId { get; set; }
    }

    /// <summary>
    /// The public and protected events for the requested <see cref="UIWidget"/>.
    /// </summary>
    internal class GetWidgetEventsResponse : Response, IWidgetMessage
    {
        /// <summary>
        /// The <see cref="UIWidget.Id"/>.
        /// </summary>
        public string WidgetId { get; set; }


        /// <summary>
        /// The widget's public and protected events.
        /// </summary>
        public UIEvent[] Events { get; set; }
    }
}