using System;
using System.Linq;
using System.Reflection;
using UISleuth.Widgets;

namespace UISleuth.Reflection
{
    internal static class UIPropertyMethods
    {
        public static UIProperty[] GetObjectProperties(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var type = value.GetType();

            var result = type
                .GetPublicProperties()
                .Select(p => new UIProperty
                {
                    UIType = new UIType
                    {
                        FullName = p.PropertyType.FullName,
                    },
                    Path = new[] { p.Name },
                    CanRead = p.CanRead,
                    CanWrite = p.CanWrite,
                    Value = p.GetValue(value),
                }).ToArray();

            return result;
        }


        /// <summary>
        /// Return primitive value types, such as byte, short, int, long.
        /// String and DateTime are not considered primitive value types.
        /// </summary>
        /// <param name="value">struct</param>
        public static UIProperty[] GetPrimitiveValueTypeProperties(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var type = value.GetType();
            UIProperty[] result = {};

            if (ReflectionMethods.IsNotPrimitiveValueType(value))
            {
                result = type
                    .GetPublicProperties()
                    .Where(p =>
                    {
                        var pi = p.PropertyType.GetTypeInfo();
                        if (pi == null) return false;
                        return (pi.IsPrimitive && pi.IsValueType) || pi.IsEnum;
                    })
                    .Select(p => new UIProperty
                    {
                        UIType = new UIType
                        {
                            FullName = p.PropertyType.FullName,
                        },
                        Path = new[] {p.Name},
                        CanRead = p.CanRead,
                        CanWrite = p.CanWrite,
                        Value = p.GetValue(value),
                    })
                    .ToArray();
            }

            return result;
        }
    }
}