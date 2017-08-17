namespace UISleuth.Messages
{
    internal class GetBindingContextRequest : Request, IWidgetMessage
    {
        public string WidgetId { get; set; }
    }

    internal class GetBindingContextResponse : Response
    {
        public string Data { get; set; }
        public string WidgetId { get; set; }
    }
}