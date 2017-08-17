using UISleuth.Widgets;

namespace UISleuth.Messages
{
    internal class XamlResponse : Response
    {
        public UIWidget Widget { get; set; }
        public UIWidget Parent { get; set; }
    }
}