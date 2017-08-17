using System;
using System.Collections.Generic;
using System.Linq;
using UISleuth.Reflection;
using UISleuth.Widgets;
using UISleuth;
using Xamarin.Forms;

namespace UISleuth.Reactions
{
    /// <summary>
    /// Common logic to build <see cref="UIProperty"/> values.
    /// These properties are requested by the client when type editors should be shown.
    /// </summary>
    internal abstract class GetPropertiesReaction : InspectorReaction
    {
        public UIProperty[] GetUIProperties(UIReflectionProperty[] refProps, bool includeValues = false)
        {
            if (refProps == null) return null;
            if (SupportingTypes.Types == null) return null;

            var result = new List<UIProperty>();

            // The properties are in an intermediate state, right here. 
            // a UIReflectionProperty is not meant to be returned in a request.

            // build ui properties.
            foreach (var curRef in refProps)
            {
                if (curRef.IsTargetEnum && SupportingTypes.IsRegistered(typeof(Enum)))
                {
                    if (!SupportingTypes.IsRegistered(curRef, RegistrarMatches.TypeName | RegistrarMatches.Enum))
                    {
                        continue;
                    }
                }
                else
                {
                    if (!SupportingTypes.IsRegistered(curRef, RegistrarMatches.TypeName))
                    {
                        continue;
                    }
                }

                var uiProp = new UIProperty
                {
                    Path = new[] { curRef.TargetName },
                    Value = curRef.GetTargetObject(),
                    CanRead = curRef.CanReadTarget,
                    CanWrite = curRef.CanWriteTarget
                };

                if (uiProp.Value is double)
                {
                    uiProp.Value = Math.Round((double) uiProp.Value, 2);
                }

                // is the current property an enumeration?
                if (curRef.IsTargetEnum && SupportingTypes.IsRegistered(typeof(Enum)))
                {
                    var value = curRef.As<Enum>();
                    if (value != null)
                    {
                        Descriptor.SetPossibleValues(curRef, uiProp, value);

                        if (uiProp.Value != null)
                        {
                            uiProp.Value = value.ToString();
                        }
                    }
                }
                else
                {
                    Descriptor.SetPossibleValues(curRef, uiProp);

                    // non-primitive structures (not System.DateTime, but Xamarin.Forms.Point)
                    if (ReflectionMethods.IsNotPrimitiveValueType(uiProp.Value))
                    {
                        uiProp.UIType.Descriptor |= UIPropertyDescriptors.ValueType;
            
                        if (uiProp.Value is Color)
                        {
                            var o = (Color) uiProp.Value;

                            if (o.A == -1 && o.R == -1 && o.G == -1 && o.B == -1)
                            {
                                uiProp.Value = "Default";
                            }
                            else if (o.A == 1)
                            {
                                uiProp.Value = $"#{(int) (o.R*255):X2}{(int) (o.G*255):X2}{(int) (o.B*255):X2}";
                            }
                            else
                            {
                                uiProp.Value = $"#{(int)(o.A * 255):X2}{(int)(o.R * 255):X2}{(int)(o.G * 255):X2}{(int)(o.B * 255):X2}";
                            }
                        }
                        else
                        {
                            var vtProps = UIPropertyMethods
                                .GetPrimitiveValueTypeProperties(uiProp.Value)
                                .ToArray();

                            if (vtProps.Length != 0)
                            {
                                foreach (var vtProp in vtProps)
                                {
                                    Descriptor.SetPossibleValues(vtProp);

                                    if (vtProp.Value.GetType() != typeof(string))
                                    {
                                        var tmp = Convert.ToString(vtProp.Value ?? string.Empty);
                                        vtProp.Value = tmp;
                                    }
                                }

                                // don't return values to the client that will never be used.
                                uiProp.Value = includeValues ? vtProps : null;
                            }
                        }
                    }

                    // enumerables, collections, list.
                    if (curRef.TargetType.IsKindOf(typeof(ICollection<>)))
                    {
                        uiProp.UIType.Descriptor |= UIPropertyDescriptors.Collection;
                    }

                    if (curRef.TargetType.IsKindOf(typeof(IList<>)))
                    {
                        uiProp.UIType.Descriptor |= UIPropertyDescriptors.List;
                    }

                    if (curRef.TargetType.IsKindOf(typeof(IEnumerable<>)))
                    {
                        uiProp.UIType.Descriptor |= UIPropertyDescriptors.Enumerable;
                        var collection = uiProp.Value as IEnumerable<object>;

                        if (collection != null)
                        {
                            var count = collection.Count();
                            uiProp.UIType.PossibleValues = new[] { count.ToString() };

                            if (includeValues == false)
                            {
                                // don't send the value back to the client
                                uiProp.Value = null;
                            }
                        }
                    }

                    /*
                     * obj support?
                    if (!curRef.TargetType.GetTypeInfo().IsValueType && !curRef.TargetType.GetTypeInfo().IsPrimitive)
                    {
                        if (!curRef.TargetType.Namespace.StartsWith("System"))
                        {
                            var oProps = XenPropertyMethods
                            .GetObjectProperties(xenProp.Value)
                            .ToArray();

                            if (oProps.Length != 0)
                            {
                                foreach (var oProp in oProps)
                                {
                                    Descriptor.SetPossibleValues(oProp);

                                    if (oProp.Value != null && oProp.Value.GetType() != typeof(string))
                                    {
                                        var tmp = Convert.ToString(oProp.Value ?? string.Empty);
                                        oProp.Value = tmp;
                                    }
                                }

                                // don't return values to the client that will never be used.
                                xenProp.Value = includeValues ? oProps : null;
                            }
                        }
                    }
                    */
                }

                result.Add(uiProp);
            }

            return result.ToArray();
        }


        /// <summary>
        /// The most common method to return an object's properties.
        /// Passing <paramref name="widgetId"/> alone will return all properties of the visual element.
        /// 
        /// A <paramref name="path"/> is an array of property names, starting after the widget.
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="includeValues">Sets whether <see cref="UIProperty.Value"/> be set.</param>
        /// <param name="path"></param>
        protected UIProperty[] GetUIProperties(string widgetId, bool includeValues = false, string[] path = null)
        {
            if (string.IsNullOrWhiteSpace(widgetId)) return null;

            var pair = Surface[widgetId];
            if (pair.VisualElement == null) return null;

            // return all of the visual element's properties.
            var refProps = path == null
                ? pair.VisualElement.GetRefProperties()
                : pair.VisualElement.GetRefProperties(path);

            return refProps == null
                ? null
                : GetUIProperties(refProps, includeValues);
        }


        protected UIProperty[] GetUIProperties(object obj, bool includeValues = false)
        {
            if (obj == null) return null;
            var refProps = obj.GetRefProperties();
            return GetUIProperties(refProps, includeValues);
        }
    }
}