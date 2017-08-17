using UISleuth.Widgets;

namespace UISleuth.Messages
{
    /// <summary>
    /// Signal's that the client has requested a UIWidget's properties.
    /// Properties can be immutable or mutable.
    /// </summary>
    internal class GetWidgetPropertiesRequest : Request, IWidgetMessage
    {
        /// <summary>
        /// Id of the requested widget's properties.
        /// </summary>
        public string WidgetId { get; set; }
        public bool IncludeValues { get; set; }
    }

    /// <summary>
    /// Result of requesting a widget's properties.
    /// Sent in response to a <see cref="GetWidgetPropertiesRequest"/>.
    /// </summary>
    internal class GetWidgetPropertiesResponse : Response
    {
        /// <summary>
        /// Representation of the target framework's visual element.
        /// </summary>
        public UIWidget Widget { get; set; }


        /// <summary>
        /// <see cref="Widget"/>'s properties.
        /// </summary>
        public UIProperty[] Properties { get; set; }
    }
}