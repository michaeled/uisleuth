using UISleuth.Widgets;

namespace UISleuth.Messages
{
    internal class GetConstructorsRequest : Request
    {
        public string TypeName { get; set; }
    }

    internal class GetConstructorsResponse : Response
    {
        public UIType Type { get; set; }
    }
}