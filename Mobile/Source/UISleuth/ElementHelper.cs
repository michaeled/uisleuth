using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UISleuth.Reflection;
using UISleuth.Widgets;
using Xamarin.Forms;
using BindingFlags = UISleuth.Reflection.BindingFlags;

namespace UISleuth
{
    internal static class ElementHelper
    {
        public static UIWidget[] GetParents(UIWidget widget)
        {
            var parents = new List<UIWidget>();
            var current = widget;

            while (current != null)
            {
                if (current.Parent != null)
                {
                    parents.Add(current.Parent);
                }

                current = current.Parent;
            }

            return parents.ToArray();
        }


        public static IEnumerable<AttachedPropertyInfo> GetAttachedProperties(BindableObject obj)
        {
            var apis = new List<AttachedPropertyInfo>();

            var dps = obj
                .GetType()
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(t => t.FieldType.IsAssignableFrom(typeof(BindableProperty)))
                .ToArray();

            foreach (var dp in dps)
            {
                if (!dp.Name.EndsWith("Property", StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                var index = dp.Name.LastIndexOf("Property", StringComparison.CurrentCultureIgnoreCase);
                var shortName = dp.Name.Substring(0, index);

                var getMethod = obj.GetType().GetMethod("Get" + shortName);
                var setMethod = obj.GetType().GetMethod("Set" + shortName);

                if (getMethod != null && setMethod != null)
                {
                    var typeName = ReflectionMethods.GetShortTypeName(obj.GetType().FullName);

                    var api = new AttachedPropertyInfo
                    {
                        PropertyName = dp.Name,
                        XamlPropertyName = $"{typeName}.{shortName}",
                        Value = getMethod.Invoke(null, new object[] {obj}),
                        Field = dp,
                        GetMethod = getMethod,
                        SetMethod = setMethod,
                        Target = obj
                    };

                    apis.Add(api);
                }
            }

            return apis.ToArray();
        }


        public static bool IsCell(Element e)
        {
            return e is Cell;
        }


        public static bool IsView(Element e)
        {
            return e is View;
        }


        public static bool IsView(PropertyInfo prop)
        {
            return prop.PropertyType.GetTypeInfo().IsAssignableFrom(typeof (View).GetTypeInfo());
        }


        public static bool IsViewContainer(Element e)
        {
            return e is IViewContainer<View>;
        }


        public static bool IsCollectionOfViews(PropertyInfo prop)
        {
            var isEnumerable = ReflectionMethods.ImplementsIEnumerable(prop);
            if (!isEnumerable) return false;

            var hasViewArg = prop
                .PropertyType
                .GenericTypeArguments
                .Any(a => a.IsAssignableFrom(typeof(View)));

            return hasViewArg;
        }

        public static bool ContentPropertyAllowsManyChildren(Element e)
        {
            var prop = GetContentProperty(e);
            if (prop == null) return false;

            if (IsCollectionOfViews(prop))
            {
                return true;
            }

            return false;
        }


        public static bool IsContentPropertyView(Element e)
        {
            var prop = GetContentProperty(e);
            if (prop == null) return false;

            if (IsCollectionOfViews(prop))
            {
                return true;
            }

            return IsView(prop);
        }


        public static void ClearContentProperty(Element e)
        {
            var viewType = e?.GetType();
            var attribs = viewType?.GetCustomAttributes(typeof(ContentPropertyAttribute), true);
            if (attribs?.Length == 0) return;
            if (attribs == null) return;

            foreach (var attrib in attribs)
            {
                var cp = attrib as ContentPropertyAttribute;
                if (cp == null) continue;

                var propName = cp.Name;
                var prop = viewType.GetProperty(propName);

                var value = prop.PropertyType.GetDefaultValue();
                prop.SetValue(e, value);
            }
        }


        public static string GetContentPropertyTypeName(Element e)
        {
            var prop = GetContentProperty(e);
            if (prop == null) return null;

            var isEnumerable = ReflectionMethods.ImplementsIEnumerable(prop);
            if (!isEnumerable) return prop.PropertyType.FullName;

            var typeArgs = prop
                .PropertyType
                .GenericTypeArguments
                .Select(a => a.FullName)
                .ToArray();

            if (typeArgs.Length == 0)
            {
                return prop.PropertyType.FullName;
            }

            var arg = string.Join(",", typeArgs);
            return $"{typeof(IEnumerable).FullName}<{arg}>";
        }


        public static bool HasContentProperty(Element e)
        {
            var type = e?.GetType();
            var attribs = type?.GetCustomAttributes(typeof(ContentPropertyAttribute), true);
            return attribs?.Length > 0;
        }


        public static PropertyInfo GetContentProperty(Element e)
        {
            PropertyInfo result = null;

            var viewType = e?.GetType();
            var propName = GetContentPropertyName(e);

            var all = viewType?.GetProperties().Where(x => x.Name == propName);

            if (all != null)
            {
                var properties = all as PropertyInfo[] ?? all.ToArray();
                result = properties.FirstOrDefault(x => x.DeclaringType == viewType) ?? properties.First();
            }

            return result;
        }


        public static string GetContentPropertyName(Element e)
        {
            var view = e?.GetType();
            var ca = view?.GetCustomAttributes(typeof(ContentPropertyAttribute), true);
            if (ca?.Length == 0) return null;

            var attrib = ca?[0] as ContentPropertyAttribute;
            return attrib?.Name;
        }
    }
}