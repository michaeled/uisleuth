namespace UISleuth.Messages
{
    internal enum EditCollectionType
    {
        Add = 0,
        Delete = 1
    }

    internal class EditCollectionRequest : Request, IWidgetMessage
    {
        public EditCollectionType Type { get; set; }
        public string WidgetId { get; set; }
        public string[] Path { get; set; }
    }

    internal class EditCollectionResponse : Response, IWidgetMessage
    {
        public bool Successful { get; set; }
        public EditCollectionType Type { get; set; }
        public string WidgetId { get; set; }
        public string Message { get; set; }
    }
}