namespace UISleuth.Messages
{
    internal class ScreenShotRequest : Request {}

    internal class ScreenShotResponse : Response
    {
        public byte[] Capture { get; set; }
    }
}