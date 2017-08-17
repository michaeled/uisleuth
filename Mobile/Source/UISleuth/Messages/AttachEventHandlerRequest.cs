using UISleuth.Reflection;

namespace UISleuth.Messages
{
    internal class AttachEventHandlerRequest : Request, IWidgetMessage
    {
        public InspectorAssembly GeneratedAssembly { get; set; }
        public string EventName { get; set; }
        public string WidgetId { get; set; }
    }
}