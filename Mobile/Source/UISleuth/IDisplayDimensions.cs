namespace UISleuth
{
    internal class DisplayDimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double Density { get; set; }
        public int StatusBarHeight { get; set; }
        public int NavigationBarHeight { get; set; }
    }

    internal interface IDisplayDimensions
    {
        DisplayDimensions GetDimensions();
    }
}