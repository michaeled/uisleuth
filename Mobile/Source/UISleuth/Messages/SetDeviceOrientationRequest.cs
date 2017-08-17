namespace UISleuth.Messages
{
    internal class SetDeviceOrientationRequest : Request
    {
        public UIDeviceOrientation Orientation { get; set; }
    }

    internal class SetDeviceOrientationResponse : Response
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }
}