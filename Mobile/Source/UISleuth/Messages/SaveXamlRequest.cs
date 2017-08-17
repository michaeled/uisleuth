namespace UISleuth.Messages
{
    internal class SaveXamlRequest : Request {}

    internal class SaveXamlResponse : Response
    {
        public bool Successful { get; set; }
        public string Xaml { get; set; }
        public string Error { get; set; }
    }
}
