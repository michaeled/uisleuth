namespace UISleuth.Messages
{
    internal class GetDisplayDimensionsResponse : Response
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double Density { get; set; }
        public int StatusBarHeight { get; set; }
        public int NavigationBarHeight { get; set; }
    }
}