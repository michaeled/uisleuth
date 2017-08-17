using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reflection;
using UISleuth.Widgets;
using Xamarin.Forms;

namespace UISleuth.Reactions
{
    internal class SetPropertyReaction : InspectorReaction
    {
        private SetPropertyRequest _req;

        protected override void OnExecute(UIMessageContext ctx)
        {
            _req = ctx.Get<SetPropertyRequest>();
            if (_req == null) return;
            
            SetPropertyValue(_req.WidgetId, _req.Path, _req.Value, _req.IsBase64, _req.IsAttachedProperty);
        }


        protected void SetPropertyValue(string rWidgetId, string[] rPath, object rValue, bool rIsBase64, bool rIsAttachedProperty)
        {
            var ignoreSet = false;
            var targetPropName = UIProperty.GetLastPath(rPath);
            var pair = Surface[rWidgetId];

            AttachedPropertyInfo targetAttachedProp = null;
            UIReflectionProperty targetProp;

            if (rIsAttachedProperty)
            {
                var apAncestors = ElementHelper.GetParents(pair.UIWidget);
                var apInfos = new List<AttachedPropertyInfo>();

                foreach (var ancestor in apAncestors)
                {
                    var aView = Surface[ancestor.Id];
                    if (aView == null) continue;
                    apInfos.AddRange(ElementHelper.GetAttachedProperties(aView.VisualElement));
                }

                if (apInfos.Count == 0) return;
                targetAttachedProp = apInfos.LastOrDefault(a => a.PropertyName == targetPropName);
                if (targetAttachedProp == null) return;

                object apParent = null;
                object apGrandParent = null;

                if (apAncestors.ElementAtOrDefault(0) != null)
                {
                    apParent = Surface[apAncestors[0].Id].VisualElement;
                }

                if (apAncestors.ElementAtOrDefault(1) != null)
                {
                    apGrandParent = Surface[apAncestors[1].Id].VisualElement;
                }

                targetProp = targetAttachedProp.Convert(apParent, apGrandParent);
            }
            else
            {
                targetProp = pair?
                    .VisualElement?
                    .GetRefProperties(rPath)
                    .FirstOrDefault();

                if (targetProp == null) return;
            }

            if (targetPropName != null && targetPropName.Contains("Color"))
            {
                if (rValue.ToString().StartsWith("#"))
                {
                    rValue = Color.FromHex(rValue.ToString());
                }
                else
                {
                    try
                    {
                        var color = typeof (Color)
                            .GetStaticFields()
                            .FirstOrDefault(c => c.Name == rValue.ToString());

                        if (color != null)
                        {
                            rValue = color.GetValue(null);
                        }
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
            }

            // todo: make into a serializer
            if (targetPropName != null && targetPropName.Contains("Source"))
            {
                if (!rIsBase64) return;
                var bytes = Convert.FromBase64String(rValue.ToString());

                var src = new UIImageSource(ImageSource.FromStream(() => new MemoryStream(bytes)))
                {
                    FileName = _req.Metadata?.ToString()
                };

                rValue = src;
            }

            // get enumeration value
            if (targetProp.IsTargetEnum)
            {
                var eval = ReflectionMethods.CreateEnum(targetProp.TargetType, rValue);

                if (eval == null)
                {
                    ignoreSet = true;
                }
                else
                {
                    rValue = eval;
                }
            }

            if (!ignoreSet)
            {
                Thread.Invoke(() =>
                {
                    // if target property is part of a structure and we're modifying its properties
                    // 1) make a copy of the current value
                    // 2) change the property value
                    // 3) reassign it
                    if (ReflectionMethods.IsValueType(targetProp.ParentObject))
                    {
                        var childProp = targetProp
                            .ParentObject
                            .GetRefProperties()
                            .FirstOrDefault(p => p.TargetName == targetPropName);

                        childProp?.SetTargetObject(rValue);
                        var copy = childProp?.ParentObject;

                        var parentInfo = targetProp
                            .GrandParentObject?
                            .GetType()
                            .GetProperty(targetProp.ParentName);

                        parentInfo?.SetValue(targetProp.GrandParentObject, copy);

                        // supporting View -> Struct -> Struct -> Target (valuetype or ref)
                        if (ReflectionMethods.IsValueType(targetProp.GrandParentObject))
                        {
                            if (rPath.Length == 3)
                            {
                                targetProp
                                    .GrandParentObject?
                                    .GetType()
                                    .GetProperty(targetProp.ParentName)
                                    .SetValue(targetProp.GrandParentObject, targetProp.ParentObject);

                                pair.VisualElement
                                    .GetType()
                                    .GetProperty(rPath[0])
                                    .SetValue(pair.VisualElement, targetProp.GrandParentObject);
                            }
                        }
                    }
                    // this is assinging a field
                    else if (targetProp.IsTargetStruct && rValue is string)
                    {
                        var fieldName = rValue.ToString() ?? string.Empty;
                        var fieldInfo = targetProp.TargetType.GetStaticFields().FirstOrDefault(f => f.Name == fieldName);

                        if (fieldInfo == null)
                        {
                            if (rIsAttachedProperty)
                            {
                                SetAttachedProperty(rValue, targetAttachedProp, pair);
                            }
                            else
                            {
                                targetProp.SetTargetObject(rValue);
                            }
                        }
                        else
                        {
                            var copy = fieldInfo.GetValue(null);

                            if (rIsAttachedProperty)
                            {
                                SetAttachedProperty(copy, targetAttachedProp, pair);
                            }
                            else
                            {
                                targetProp.SetTargetObject(copy);
                            }
                        }
                    }
                    else
                    {
                        // one last attempt
                        if (rIsAttachedProperty)
                        {
                            SetAttachedProperty(rValue, targetAttachedProp, pair);
                        }
                        else
                        {
                            targetProp.SetTargetObject(rValue);
                        }
                    }
                });
            }
        }


        private void SetAttachedProperty(object value, AttachedPropertyInfo attachedProperty, SurfacePair pair)
        {
            var set = value;

            if (set != null && (value.Equals("NULL") || value.Equals("null")))
            {
                set = null;
            }

            if (set != null)
            {
                set = Convert.ChangeType(value, attachedProperty?.GetMethod.ReturnType);
            }

            try
            {
                attachedProperty?.SetMethod.Invoke(null, new[] {pair.VisualElement, set});
            }
            catch (ArgumentException)
            {
                // ignored for now; this should be logged and the user should be notified with a validation message
            }
            catch (Exception)
            {
                // ignored for now; this should be logged and the user should be notified with a validation message
            }
        }
    }
}