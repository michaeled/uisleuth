using System;
using System.Linq;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reflection;
using UISleuth.Widgets;

namespace UISleuth.Reactions
{
    internal class GetObjectReaction : GetPropertiesReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            if (ctx.Request == null) return;

            var req = ctx.Get<GetObjectRequest>();
            if (req == null) return;

            var props = GetUIProperties(req.WidgetId, true, req.Path);

            if (!props.Any())
            {
                return;
            }

            var prop = props[0];
            if (prop.UIType.Descriptor.HasFlag(UIPropertyDescriptors.Enumerable))
            {
                if (prop.UIType.Descriptor.HasFlag(UIPropertyDescriptors.Enumerable))
                {
                    // grab index value; if null, return without creating an ObjectResponse.
                    var index = ReflectionMethods.GetIndexerValue(req.Path[0]);
                    if (index == null) return;

                    var item = ReflectionMethods.GetItem(prop.Value, index.Value);
                    prop.Value = GetUIProperties(item);
                }
            }

            prop.Path = req.Path?.Union(prop.Path)?.ToArray();
            var cantCast = SetPath(prop.Value, req.Path);

            ctx.SetResponse<ObjectResponse>(res =>
            {
                res.UnknownCondition = cantCast;
                res.Property = prop;
                res.WidgetId = req.WidgetId;
                res.ObjectName = UIProperty.GetLastPath(req.Path);
            });
        }


        private bool SetPath(object value, params string[] path)
        {
            try
            {
                var properties = (UIProperty[])value;

                if (properties != null)
                {
                    foreach (var p in properties)
                    {
                        p.Path = path?.Union(p.Path)?.ToArray();
                    }
                }
            }
            catch (InvalidCastException)
            {
                return false;
            }

            return true;
        }
    }
}