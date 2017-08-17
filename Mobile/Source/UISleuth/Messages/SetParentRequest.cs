namespace UISleuth.Messages
{
    internal class SetParentRequest : Request
    {
        public string ParentId { get; set; }
        public string ChildId { get; set; }
        public int Index { get; set; }
    }

    internal class SetParentResponse : Response
    {
        public string ParentId { get; set; }
        public string ChildId { get; set; }
        public int Index { get; set; }

        public bool Success { get; set; }   
    }
}