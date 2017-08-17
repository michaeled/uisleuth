namespace UISleuth.Messages
{
    /// <summary>
    /// A message from the client requesting that a XAML document be loaded.
    /// </summary>
    internal class OpenXamlRequest : Request
    {
        public string Xaml { get; set; }
        public string FileName { get; set; }
    }

    internal class OpenXamlResponse : XamlResponse
    {
        public string FileName { get; set; }
    }
}