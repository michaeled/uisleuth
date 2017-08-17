using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reflection;

namespace UISleuth.Reactions
{
    internal class EditCollectionReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var r = ctx.Get<EditCollectionRequest>();
            if (r == null) return;

            var pair = Surface[r.WidgetId];
            var prop = pair?.VisualElement.GetRefProperties(r.Path).FirstOrDefault();

            var target = prop?.GetTargetObject();
            if (target == null) return;

            var successful = false;
            var message = string.Empty;

            if (target.IsKindOf(typeof(IList<>)))
            {
                var colType = target.GetType();
                if (colType == null) return;

                var itemType = ReflectionMethods.GetEnumerableItemType(colType);
                if (itemType == null) return;

                if (r.Type == EditCollectionType.Delete)
                {
                    try
                    {
                        var index = ReflectionMethods.GetIndexerValue(r.Path.Last());
                        if (index == null || index < 0) return;

                        colType.GetMethod("RemoveAt").Invoke(target, new[] { (object) index.Value });
                        successful = true;
                    }
                    catch (Exception e)
                    {
                        message = e.Message;
                        successful = false;
                    }
                }

                if (r.Type == EditCollectionType.Add)
                {
                    try
                    {
                        object newItem;

                        if (colType.GetTypeInfo().IsGenericTypeDefinition)
                        {
                            newItem = Activator.CreateInstance(colType.MakeGenericType(itemType));
                        }
                        else
                        {
                            newItem = Activator.CreateInstance(itemType);
                        }

                        colType.GetMethod("Add").Invoke(target, new[] { newItem });
                        successful = true;
                    }
                    catch (Exception e)
                    {
                        message = e.Message;
                        successful = false;
                    }
                }
            }

            ctx.SetResponse<EditCollectionResponse>(c =>
            {
                if (successful)
                {
                    c.Suggest<GetWidgetPropertiesRequest>();
                }

                c.WidgetId = r.WidgetId;
                c.Type = r.Type;
                c.Successful = successful;
                c.Message = message;
            });
        }
    }
}
