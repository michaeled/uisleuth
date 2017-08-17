using UISleuth.Widgets;

namespace UISleuth.Messages
{
    internal class CallConstructorRequest : Request
    {
        public UIConstructor Constructor { get; set; }
        public UIProperty Property { get; set; }
        public string WidgetId { get; set; }
    }

    internal class CallConstructorResponse : Response
    {
        public string ErrorMessage { get; set; }
        public bool Successful { get; set; }
    }
}