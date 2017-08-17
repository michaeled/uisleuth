namespace UISleuth
{
    internal struct GesturePath
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    internal interface ITouchEvent
    {
        void Down(int x, int y, int duration);
        void Gesture(GesturePath[] path, int duration);
    }
}