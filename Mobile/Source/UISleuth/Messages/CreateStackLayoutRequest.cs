namespace UISleuth.Messages
{
    internal class CreateStackLayoutRequest : Request
    {
        public string ParentId { get; set; }
        public string Orientation { get; set; }
        public double Spacing { get; set; }
    }
}