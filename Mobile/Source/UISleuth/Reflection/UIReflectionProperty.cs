using System;
using System.Diagnostics;
using System.Reflection;

namespace UISleuth.Reflection
{
    /// <summary>
    /// Contains a reference to an object instance and a interesting property.
    /// Methods on this type are used as a convenient way to get and set the object's property value.
    /// An object of this type is not meant to be returned to a client.
    /// </summary>
    [DebuggerDisplay("{TargetType.Name} {TargetName}")]
    internal sealed class UIReflectionProperty
    {
        public object GrandParentObject { get; set; }
        public string ParentName { get; set; }
        public object ParentObject { get; set; }
        public Type ParentType { get; internal set; }
        public string TargetName { get; internal set; }
        public Type TargetType { get; internal set; }
        public bool CanReadTarget { get; internal set; }
        public bool CanWriteTarget { get; internal set; }


        public bool IsTargetStruct => ReflectionMethods.IsValueType(TargetType);


        public bool IsTargetEnum
        {
            get
            {
                if (TargetType == null) return false;
                return TargetType.GetTypeInfo().IsEnum;
            }
        }

        public T As<T>()
        {
            return (T) GetTargetObject();
        }


        public object GetTargetObject()
        {
            try
            {
                var stripped = ReflectionMethods.StripIndexer(TargetName);
                return ParentType.GetRuntimeProperty(stripped).GetValue(ParentObject);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public void SetTargetObject(object value)
        {
            try
            {
                var set = value;

                if (set != null && (value.Equals("NULL") || value.Equals("null")))
                {
                    set = null;
                }

                if (set != null)
                {
                    set = Convert.ChangeType(value, TargetType);
                }

                ParentType.GetRuntimeProperty(TargetName).SetValue(ParentObject, set);
            }
            catch (InvalidCastException)
            {
                try
                {
                    ParentType.GetRuntimeProperty(TargetName).SetValue(ParentObject, value);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}