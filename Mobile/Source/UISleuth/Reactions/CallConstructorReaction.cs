using System;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reflection;

namespace UISleuth.Reactions
{
    internal class CallConstructorReaction : SetPropertyReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            if (ctx.Request == null) return;

            var req = ctx.Get<CallConstructorRequest>();
            if (req == null) return;

            var error = string.Empty;
            var success = false;
            var obj = UIConstructorMethods.Construct(TypeFinder, req.Constructor);

            if (obj != null)
            {
                try
                {
                    SetPropertyValue(req.WidgetId, req.Property.Path, obj, false, false);
                }
                catch (Exception ex)
                {
                    error = ex.ToString();
                    success = false;
                }

                success = true;
            }

            ctx.SetResponse<CallConstructorResponse>(res =>
            {
                res.ErrorMessage = error;
                res.Successful = success;
            });
        }
    }
}
