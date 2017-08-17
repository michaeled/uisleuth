using System.Reflection;
using UISleuth.Reflection;

namespace UISleuth
{
    internal class AttachedPropertyInfo
    {
        public object Target { get; set; }
        public FieldInfo Field { get; set; }
        public MethodInfo GetMethod { get; set; }
        public MethodInfo SetMethod { get; set; }
        public object Value { get; set; }
        public string XamlPropertyName { get; set; }
        public string PropertyName { get; set; }


        public UIReflectionProperty Convert(object parent, object grandparent)
        {
            var xprop = new UIReflectionProperty
            {
                ParentObject = parent,
                GrandParentObject = grandparent,
                TargetType = GetMethod.ReturnType,
                CanReadTarget = true,
                CanWriteTarget = true,
                TargetName = PropertyName,
                ParentType = parent.GetType()
            };

            return xprop;
        }
    }
}