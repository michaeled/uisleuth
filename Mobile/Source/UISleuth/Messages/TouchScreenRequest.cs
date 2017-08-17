namespace UISleuth.Messages
{
    internal class TouchScreenRequest : UIMessage
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Duration { get; set; }
    }
}