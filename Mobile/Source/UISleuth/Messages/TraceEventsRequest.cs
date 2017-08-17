namespace UISleuth.Messages
{
    internal class TraceEventsRequest : Request, IWidgetMessage
    {
        public string WidgetId { get; set; }
    }
}
