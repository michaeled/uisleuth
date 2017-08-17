using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class SetParentReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<SetParentRequest>();
            if (request == null) return;

            var success = true;

            Thread.Invoke(() =>
            {
                var child = Surface[request.ChildId];
                var parent = Surface[request.ParentId];

                if (child == null || parent == null)
                {
                    success = false;
                }
                else
                {
                    var remove = Surface.Remove(child.UIWidget);

                    if (remove)
                    {
                        success = Surface.SetParent(child.VisualElement, parent.VisualElement, request.Index);
                    }
                    else
                    {
                        success = false;
                    }
                }
            });

            ctx.SetResponse<SetParentResponse>(r =>
            {
                r.Success = success;
                r.ParentId = request.ParentId;
                r.ChildId = request.ChildId;
                r.Index = r.Index;
                r.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}
