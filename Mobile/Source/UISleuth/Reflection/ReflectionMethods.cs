using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace UISleuth.Reflection
{
    internal static class ReflectionMethods
    {
        public static Func<FieldInfo, string, bool> FieldNameMatch = (p, n) => !string.IsNullOrWhiteSpace(n) && p.Name.Equals(n);
        public static Func<PropertyInfo, string, bool> NameMatch = (p, n) => !string.IsNullOrWhiteSpace(n) && p.Name.Equals(n);
        public static readonly Regex EnumerablePattern = new Regex(@"\[(\d+)\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);


        public static object CreateEnum(Type type, object value)
        {
            // the value in the request
            var literal = value?.ToString();

            if (string.IsNullOrWhiteSpace(literal))
            {
                return null;
            }
            
            // flags enum
            var formatted = literal.Replace("|", ",");

            if (formatted.Contains(","))
            {
                return Enum.Parse(type, formatted, true);
            }

            return Enum.Parse(type, literal, true);
        }


        public static bool IsUISleuthDiscoverable(Assembly assembly)
        {
            if (assembly?.CustomAttributes == null)
            {
                return false;
            }

            var name = typeof(UISleuthDiscoverable).FullName;

            var discoverable = assembly
                .CustomAttributes.Any(c => c.AttributeType.FullName.Equals(name));

            return discoverable;
        }


        /// <summary>
        /// For a generic enumerable type, return the generic type parameter used when
        /// instantiating the list.
        /// </summary>
        /// <param name="type"></param>
        public static Type GetEnumerableItemType(Type type)
        {
            var match = FindIEnumerable(type);
            return match == null ? type : match.GetGenericArguments()[0];
        }


        /// <summary>
        /// Return a enumerable index and property name for the given <paramref name="fullName"/>.
        /// Example: Columns[1] : index = 1; stripped = Columns.
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="index"></param>
        /// <param name="stripped"></param>
        /// <returns></returns>
        public static bool ParseIndexer(string fullName, out int? index, out string stripped)
        {
            stripped = StripIndexer(fullName);
            index = GetIndexerValue(fullName);

            return EnumerablePattern.IsMatch(fullName);
        }


        /// <summary>
        /// Strip any indexer from the <paramref name="propertyName"/>.
        /// Example: Columns[0] returns Columns.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string StripIndexer(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return null;

            var index = propertyName.IndexOf("[", StringComparison.OrdinalIgnoreCase);
            if (index == -1) return propertyName;

            return propertyName.Substring(0, index);
        }


        /// <summary>
        /// Return the index value from a <paramref name="propertyName"/>.
        /// Example: Columns[1] returns 1.
        /// Example: Rows returns null.
        /// </summary>
        /// <param name="propertyName"></param>
        public static int? GetIndexerValue(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return null;

            var match = EnumerablePattern.Match(propertyName);
            if (match.Success)
            {
                int index;
                var parsed = int.TryParse(match.Groups[1].Value, out index);

                if (parsed)
                {
                    return index;
                }
            }

            return null;
        }


        /// <summary>
        /// Returns true if the object isn't a primitive value type; otherwise, false.
        /// </summary>
        /// <param name="value"></param>
        public static bool IsNotPrimitiveValueType(object value)
        {
            if (value == null) return false;

            var type = value.GetType();
            var typeInfo = type?.GetTypeInfo();
            if (typeInfo == null) return false;

            if (typeInfo.IsValueType && !typeInfo.IsEnum && !typeInfo.IsPrimitive)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// For any enumerable type, return the object at the given index.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="index"></param>
        /// <param name="max"></param>
        public static object GetItem(object obj, int index, int? max = null)
        {
            if (obj == null) return null;

            var array = obj as Array;
            if (array != null)
            {
                return array.GetValue(index);
            }

            var enumerable = obj as IEnumerable;
            if (enumerable != null)
            {
                var i = 0;
                foreach (var item in enumerable)
                {
                    if (i == index)
                    {
                        return item;
                    }

                    // escape this loop if a max is set.
                    if (max != null && (i + 1 >= max))
                    {
                        return null;
                    }

                    i++;
                }
            }

            return null;
        }


        /// <summary>
        /// Determine if the object is a user-defined structure.
        /// </summary>
        /// <param name="value"></param>
        public static bool IsValueType(object value)
        {
            if (value == null) return false;

            var type = value.GetType();
            var info = type?.GetTypeInfo();

            if (info == null)
            {
                return false;
            }

            return info.IsValueType;
        }


        /// <summary>
        /// Determine if the type is a structure (value type).
        /// </summary>
        /// <param name="type"></param>
        public static bool IsValueType(Type type)
        {
            var info = type?.GetTypeInfo();

            if (info == null)
            {
                return false;
            }

            return info.IsValueType;
        }


        /// <summary>
        /// Returned the type name without the containing Assembly or Namespace.
        /// </summary>
        /// <param name="typeName"></param>
        public static string GetShortTypeName(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return string.Empty;
            }

            var isNullable = false;

            if (typeName.Contains(","))
            {
                var split = typeName.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)[0];
                return GetShortTypeName(split);
            }

            if (!typeName.Contains("."))
            {
                return typeName;
            }

            if (typeName.Contains("System.Nullable"))
            {
                isNullable = true;
            }

            var index = typeName.LastIndexOf(".", StringComparison.CurrentCultureIgnoreCase);
            var start = index + 1;

            if (typeName.EndsWith(".", StringComparison.CurrentCultureIgnoreCase))
            {
                return typeName;
            }

            var str = typeName.Substring(start);

            if (isNullable)
            {
                str += "?";
            }

            return str;
        }


        /// <summary>
        /// Determine if the object implements an interface designated by <paramref name="type"/>.
        /// </summary>
        /// <param name="obj">The object in question</param>
        /// <param name="type">The interface</param>
        public static bool IsKindOf(this object obj, Type type)
        {
            return obj.GetType().IsKindOf(type);
        }


        /// <summary>
        /// Determine if <paramref name="x"/> implements the type identified by <paramref name="y"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsKindOf(this Type x, Type y)
        {
            var xi = x.GetTypeInfo();

            if (xi.IsInterface && xi.IsGenericType && xi.GetGenericTypeDefinition() == y)
            {
                return true;
            }

            foreach (var i in x.GetInterfaces())
            {
                if (i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == y)
                {
                    return true;
                }
            }

            return false;
        }


        public static bool ImplementsIEnumerable(PropertyInfo prop)
        {
            var isEnumerable = prop
                .PropertyType
                .GetInterfaces()
                .Any(t => t.GetTypeInfo().IsAssignableFrom(typeof (IEnumerable).GetTypeInfo()));

            return isEnumerable;
        }


        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof (string))
            {
                return null;
            }

            if (seqType.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            }

            if (seqType.GetTypeInfo().IsGenericType)
            {
                foreach (var arg in seqType.GetGenericArguments())
                {
                    var ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            var ifaces = (Type[]) seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (var iface in ifaces)
                {
                    var ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (seqType.GetTypeInfo().BaseType != null && seqType.GetTypeInfo().BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.GetTypeInfo().BaseType);
            }

            return null;
        }
    }
}
