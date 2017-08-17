using UISleuth.Widgets;

namespace UISleuth.Messages
{
    internal class AddSupportedTypeRequest : Request
    {
        public UIType Type { get; set; }
    }

    internal class AddSupportedTypeResponse : Response
    {
        public bool Added { get; set; }
        public bool AlreadyRegistered { get; set; }
        public string DisplayMessage { get; set; }
    }
}