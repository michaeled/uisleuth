namespace UISleuth.Messages
{
    internal class GetObjectRequest : Request, IWidgetMessage
    {
        public string WidgetId { get; set; }
        public string[] Path { get; set; }
    }
}