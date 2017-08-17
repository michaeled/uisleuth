using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UISleuth.Reflection;
using UISleuth.Widgets;

namespace UISleuth
{
    internal class DescriptorRegistrar
    {
        #region Singleton


        private DescriptorRegistrar() {}


        private static DescriptorRegistrar _registrar;
        public static DescriptorRegistrar Create(ITypeFinder typeFinder)
        {
            if (_registrar == null)
            {
                _registrar = new DescriptorRegistrar
                {
                    _typeFinder = typeFinder
                };
            }

            return _registrar;
        }


        #endregion


        private readonly Dictionary<Type, Item> _items = new Dictionary<Type, Item>();
        private ITypeFinder _typeFinder;


        private class Item
        {
            public Type Type { get; set; }
            public UIPropertyDescriptors Descriptor { get; set; }
            public IGenerateValues Generator { get; set; }
        }


        public int Count => _items.Count;


        public void Reset()
        {
            _items.Clear();
        }


        public void Add(Type type, IGenerateValues generator)
        {
            Add(type, UIPropertyDescriptors.None, generator);
        }


        public void Add(Type type, UIPropertyDescriptors descriptor, IGenerateValues generator = null)
        {
            var value = new Item
            {
                Type = type,
                Descriptor = descriptor,
                Generator = generator
            };

            _items.Add(type, value);
        }


        public void SetPossibleValues(Type type, UIType uiType)
        {
            var entry = GetItem(type);

            uiType.PossibleValues = entry.Generator.Get(type);

            if (uiType.PossibleValues?.Length > 0)
            {
                uiType.Descriptor |= UIPropertyDescriptors.Literals;
            }
        }


        public void SetPossibleValues(UIProperty property)
        {
            if (property.Value == null) return;

            property.UIType.Descriptor = GetDescriptors(property.Value.GetType());

            var entry = GetItem(property.Value.GetType());
            if (entry?.Generator == null) return;

            var type = _typeFinder.Find(property.UIType.FullName);
            if (type == null) return;

            property.UIType.PossibleValues = entry.Generator.Get(type);

            if (property.UIType.PossibleValues?.Length > 0)
            {
                property.UIType.Descriptor |= UIPropertyDescriptors.Literals;
            }
        }


        public void SetPossibleValues(UIReflectionProperty xRef, UIProperty xProp)
        {
            if (xRef == null || xProp == null) return;

            xProp.UIType = CreateType(xRef);

            var entry = GetItem(xProp.Value?.GetType());
            if (entry?.Generator == null) return;

            var type = _typeFinder.Find(xProp.UIType.FullName);
            if (type == null) return;

            xProp.UIType.PossibleValues = entry.Generator.Get(type);

            if (xProp.UIType.PossibleValues?.Length > 0)
            {
                xProp.UIType.Descriptor |= UIPropertyDescriptors.Literals;
            }
        }


        public void SetPossibleValues(UIReflectionProperty xRef, UIProperty xProp, Enum e)
        {
            if (xRef == null || xProp == null) return;

            xProp.UIType = CreateType(xRef);
            var item = GetItem(xProp.UIType.GetType());

            var gen = new EnumGenerator();
            var result = gen.Get(e.GetType());

            if (item != null)
            {
                xProp.UIType.Descriptor = item.Descriptor;
            }

            xProp.UIType.PossibleValues = result;
            xProp.UIType.Descriptor |= UIPropertyDescriptors.Literals;

            if (e.HasFlags())
            {
                xProp.UIType.Descriptor |= UIPropertyDescriptors.Flags;
            }
        }


        internal UIPropertyDescriptors GetDescriptors(Type type)
        {
            var entry = GetItem(type);
            if (entry == null) return UIPropertyDescriptors.None;

            return entry.Descriptor;
        }


        private Item GetItem(Type type)
        {
            if (type == null) return null;

            var match = _items
                .Where(e => e.Value.Type == type)
                .Select(e => (KeyValuePair<Type, Item>?)e)
                .FirstOrDefault();

            if (match == null)
            {
                if (type.GetTypeInfo().IsValueType)
                {
                    var valueTypeGenerator = _items
                        .Where(e =>
                        {
                            return e.Value.Type == typeof (ValueType);
                        })
                        .Select(e => (KeyValuePair<Type, Item>?)e)
                        .FirstOrDefault();

                    return valueTypeGenerator?.Value;
                }
            }

            return match?.Value;
        }


        public UIType CreateType(MethodInfo info)
        {
            return new UIType
            {
                Descriptor = GetDescriptors(info.ReturnType),
                FullName = info.ReturnType.FullName
            };
        }


        public UIType CreateType(FieldInfo fieldInfo)
        {
            return new UIType
            {
                Descriptor = GetDescriptors(fieldInfo.FieldType),
                FullName = fieldInfo.FieldType.FullName
            };
        }


        public UIType CreateType(UIReflectionProperty xRef)
        {
            return new UIType
            {
                Descriptor = GetDescriptors(xRef.TargetType),
                FullName = xRef.TargetType.FullName,
            };
        }
    }
}