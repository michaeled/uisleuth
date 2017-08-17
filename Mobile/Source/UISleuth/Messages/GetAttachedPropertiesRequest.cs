using UISleuth.Widgets;

namespace UISleuth.Messages
{
    internal class GetAttachedPropertiesRequest : Request
    {
        public string WidgetId { get; set; }
    }

    internal class GetAttachedPropertiesResponse : Response
    {
        public UIWidget Widget { get; set; }
    }
}