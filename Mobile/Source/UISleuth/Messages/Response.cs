namespace UISleuth.Messages
{
    internal abstract class Response : UIMessage
    {
        public bool UnhandledExceptionOccurred { get; set; }
        public string ExceptionMessage { get; set; }
    }
}