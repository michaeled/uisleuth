using System.Collections.Generic;
using System.Linq;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Widgets;

namespace UISleuth.Reactions
{
    internal class GetAttachedPropertiesReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            if (ctx.Request == null) return;

            var req = ctx.Get<GetAttachedPropertiesRequest>();
            if (req == null) return;

            var target = Surface[req.WidgetId];

            if (target == null) return;

            var all = new List<UIProperty>();
            var parents = ElementHelper.GetParents(target.UIWidget);

            foreach (var parent in parents)
            {
                var view = Surface[parent.Id].VisualElement;

                var aps = ElementHelper.GetAttachedProperties(view).ToArray();
                if (aps == null || aps.Length == 0) continue;

                foreach (var ap in aps)
                {
                    // skip dupe attached props
                    var processed = all.Any(a => a.XamlPropertyName == ap.XamlPropertyName);
                    if (processed) continue;

                    var xt = Descriptor.CreateType(ap.GetMethod);
                    xt.Descriptor |= UIPropertyDescriptors.AttachedProperty;
                    var apval = ap.GetMethod.Invoke(null, new object[] {target.VisualElement});

                    var xp = new UIProperty
                    {
                        XamlPropertyName = ap.XamlPropertyName,
                        CanWrite = true,
                        CanRead = true,
                        UIType = xt,
                        Value = apval,
                        Path = new[] { ap.Field.Name }
                    };

                    all.Add(xp);
                }
            }

            if (target.UIWidget == null) return;

            target.UIWidget.AttachedProperties = all.ToArray();

            ctx.SetResponse<GetAttachedPropertiesResponse>(r =>
            {
                r.Widget = target.UIWidget;
            });
        }
    }
}
