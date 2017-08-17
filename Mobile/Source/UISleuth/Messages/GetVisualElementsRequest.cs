namespace UISleuth.Messages
{
    internal class GetVisualElementsRequest : Request
    {
        // ignored
    }

    internal class GetVisualElementsResponse : Response
    {
        public string[] Views { get; set; }
        public string[] Layouts { get; set; }
        public string[] Others { get; set; }
    }
}