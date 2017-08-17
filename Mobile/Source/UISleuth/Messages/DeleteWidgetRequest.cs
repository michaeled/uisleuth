namespace UISleuth.Messages
{
    internal class DeleteWidgetRequest : Request, IWidgetMessage
    {
        public string WidgetId { get; set; }
    }
}
