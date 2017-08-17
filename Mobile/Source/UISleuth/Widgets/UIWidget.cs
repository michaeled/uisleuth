using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace UISleuth.Widgets
{
    [DebuggerDisplay("{Type} has {Children.Count} children.")]
    public class UIWidget
    {
        public UIWidget()
        {
            Children = new List<UIWidget>();
            Properties = new List<UIProperty>();
            Events = new List<UIEvent>();
            Dimensions = new UIWidgetDimensions();
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public string FullTypeName { get; set; }
        public string Name { get; set; }

        public bool IsDatabound { get; set; }
        public bool CanDelete { get; set; }
        public bool IsLayout { get; set; }
        public bool IsCell { get; set; }

        public bool AllowsManyChildren { get; set; }
        public bool HasContentProperty { get; set; }
        public bool IsContentPropertyViewType { get; set; }
        public string ContentPropertyTypeName { get; set; }

        public UIWidgetDimensions Dimensions { get; set; }

        public IList<UIWidget> Children { get; set; }
        public IList<UIProperty> Properties { get; set; }
        public IList<UIProperty> AttachedProperties { get; set; }
        public IList<UIEvent> Events { get; set; }

        [JsonIgnore]
        public UIWidget Parent { get; set; }
    }
}