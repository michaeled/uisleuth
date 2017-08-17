using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UISleuth.Reflection
{
    internal static class UIReflectionPropertyExtensions
    {
        public static UIReflectionProperty[] GetRefProperties(this object parent, string[] path, bool includeStatics = false)
        {
            var target = parent;
            var result = new List<UIReflectionProperty>();

            if (path == null || path.Length == 0)
            {
                throw new InvalidOperationException("This method is meant to traverse an object graph.");
            }

            if (path.Length == 1)
            {
                var res = GetObject(parent, path[0], null, includeStatics);

                // null if the index is out of bounds.
                if (res == null) return new UIReflectionProperty[] {};

                // static property
                if (res.PropertyInfo != null)
                {
                    result.Add(CreateRefProperty(null, res.Input, res.PropertyName, null, res.PropertyInfo));
                }
            }
            else
            {
                GetObjectResult res = null;

                object grandParent = null;
                string inputName = null;

                for (var i = 0; i < path.Length; i++)
                {
                    var name = path[i];
                    res = GetObject(target, name, inputName, includeStatics);
                    target = res.Leaf;

                    // should be one behind
                    inputName = name;

                    if (path.Length == 2)
                    {
                        grandParent = parent;
                    }
                    else
                    {
                        // 3 places back is the grandparent
                        if (i == path.Length - 3)
                        {
                            grandParent = target;
                        }
                    }
                }
     
                if (res != null)
                {
                    result.Add(CreateRefProperty(grandParent, res.Input, res.PropertyName, res.InputName, res.PropertyInfo));
                }
            }

            return result.ToArray();
        }


        public static UIReflectionProperty[] GetRefProperties(this object parent, bool includeStatics = false)
        {
            var results = new List<UIReflectionProperty>();
            var properties = parent.GetType().GetPublicProperties(includeStatics);

            foreach (var propInfo in properties)
            {
                var uiProp = CreateRefProperty(null, parent, propInfo.Name, null, propInfo);
                results.Add(uiProp);
            }

            return results.ToArray();
        }


        private static UIReflectionProperty CreateRefProperty(object grandParent, object parent, string propertyName, string parentName, PropertyInfo propInfo)
        {
            var uiProp = new UIReflectionProperty
            {
                GrandParentObject = grandParent,
                ParentName = parentName,
                ParentObject = parent,
                ParentType = parent.GetType(),

                TargetName = propertyName,
                TargetType = propInfo.PropertyType,
                CanReadTarget = propInfo.CanRead,
                CanWriteTarget = propInfo.CanWrite
            };

            if (propInfo.SetMethod?.Attributes != null)
            {
                var setIsPublic = propInfo.SetMethod.Attributes.HasFlag(MethodAttributes.Public);
                uiProp.CanWriteTarget = uiProp.CanWriteTarget && setIsPublic;
            }

            return uiProp;
        }


        private static GetObjectResult GetObject(object parent, string propertyName, string inputName, bool includeStatics)
        {
            var result = new GetObjectResult
            {
                PropertyName = propertyName,
                Input = parent,
                InputName = inputName
            };

            object propVal = null;

            var isEnumerable = ReflectionMethods.EnumerablePattern.IsMatch(propertyName);
            var stripped = ReflectionMethods.StripIndexer(propertyName);

            FieldInfo fieldInfo = null;

            var propInfo = parent
                .GetType()
                .GetPublicProperties(includeStatics)
                .FirstOrDefault(p => ReflectionMethods.NameMatch(p, stripped));

            if (propInfo == null)
            {
                fieldInfo = parent
                    .GetType()
                    .GetFields()
                    .FirstOrDefault(p => ReflectionMethods.FieldNameMatch(p, stripped));
            }

            if (propInfo != null)
            {
                propVal = propInfo.GetValue(parent);
            }

            if (fieldInfo != null)
            {
                propVal = fieldInfo.GetValue(parent);
            }

            if (isEnumerable)
            {
                result.Property = propVal;

                var index = ReflectionMethods.GetIndexerValue(propertyName);
                if (index == null) return null;

                var item = ReflectionMethods.GetItem(propVal, index.Value);
                if (item == null) return null;

                result.Leaf = item;
            }
            else
            {
                result.Property = parent;
                result.Leaf = propVal;
            }

            result.PropertyInfo = result.Input.GetType().GetProperty(stripped);

            return result;
        }


        private class GetObjectResult
        {
            public object Input { get; set; }
            public object Property { get; set; }
            public string PropertyName { get; set; }
            public object Leaf { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
            public string InputName { get; set; }
        }
    }
}