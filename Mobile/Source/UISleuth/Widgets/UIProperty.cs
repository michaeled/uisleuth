using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UISleuth.Messages;
using UISleuth.Reflection;

namespace UISleuth.Widgets
{
    /// <summary>
    /// Contains properties for a target framework's visual element.
    /// Examples of properties include: Text, Font Color, Horizontal Text Alignment, etc.
    /// This data is contained in a <see cref="GetWidgetPropertiesResponse"/> in response to <see cref="GetWidgetPropertiesRequest"/>.
    /// The properties are required by the client to display type editors.
    /// </summary>
    /// <seealso cref="GetWidgetPropertiesRequest"/>
    [DebuggerDisplay("{PropertyName} = {Value}")]
    public class UIProperty : IShallowClone<UIProperty>
    {
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public object Value { get; set; }
        public object Metadata { get; set; }
        public string[] Path { get; set; }
        public UIType UIType { get; set; }
        public string XamlPropertyName { get; set; }

        [JsonIgnore]
        public int? ItemIndex => ReflectionMethods.GetIndexerValue(PropertyName);

        [JsonIgnore]
        public string PropertyName => GetLastPath(Path);

        [JsonIgnore]
        public bool IsValueType => Value != null && Value.GetType().GetTypeInfo().IsValueType;


        public static string GetLastPath(string[] path)
        {
            if (path == null || path.Length == 0)
            {
                return null;
            }

            return path.Last();
        }


        public UIProperty ShallowClone()
        {
            return new UIProperty
            {
                XamlPropertyName = XamlPropertyName,
                Value = Value,
                CanRead = CanRead,
                CanWrite = CanWrite,
                UIType = UIType,
                Path = Path
            };
        }
    }
}