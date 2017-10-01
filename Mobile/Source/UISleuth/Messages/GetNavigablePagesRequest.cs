namespace UISleuth.Messages
{
    internal enum NavigablePageTypes
    {
        Page = 0,
        MasterPage = 1,
        DetailPage = 2,
        TabbedPage = 3
    }

    internal class NavigablePage
    {
        public string Id { get; set; }
        public NavigablePageTypes Type { get; set; }
    }

    internal class GetNavigablePagesRequest : Request
    {
    }

    internal class GetNavigablePagesResponse : Response
    {
        public NavigablePage[] Pages { get; set; }
    }
}
