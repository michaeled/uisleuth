namespace UISleuth.Messages
{
    internal class GestureRequest : UIMessage
    {
        public GesturePath[] Path { get; set; }
        public int Duration { get; set; }
    }
}