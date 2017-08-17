namespace UISleuth.Messages
{
    internal class CreateWidgetRequest : Request
    {
        public string ParentId { get; set; }
        public string TypeName { get; set; }
    }

    internal class CreateWidgetResponse : XamlResponse {}
}