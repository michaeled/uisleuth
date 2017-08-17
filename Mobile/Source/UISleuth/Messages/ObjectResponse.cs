using UISleuth.Widgets;

namespace UISleuth.Messages
{
    internal class ObjectResponse : Response, IWidgetMessage
    {
        public string WidgetId { get; set; }
        public string ObjectName { get; set; }
        public UIProperty Property { get; set; }
        public bool UnknownCondition { get; set; }
    }
}