using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Reflection;
using UISleuth.Widgets;

namespace UISleuth.Reactions
{
    internal class GetConstructorsReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            if (ctx.Request == null) return;

            var req = ctx.Get<GetConstructorsRequest>();
            if (req == null) return;

            var type = TypeFinder.Find(req.TypeName);
            if (type == null) return;

            var ctors = UIConstructorMethods.GetConstructors(type);

            foreach (var ctor in ctors)
            {
                foreach (var p in ctor.Parameters)
                {
                    var pType = TypeFinder.Find(p.TypeName);

                    p.UIType = new UIType
                    {
                        Descriptor = Descriptor.GetDescriptors(pType),
                        FullName = pType.FullName,
                    };

                    Descriptor.SetPossibleValues(pType, p.UIType);
                }
            }

            ctx.SetResponse<GetConstructorsResponse>(res =>
            {
                var descs = Descriptor.GetDescriptors(type);

                res.Type = new UIType
                {
                    FullName = req.TypeName,
                    Descriptor = descs,
                    Constructors = ctors
                };
            });
        }
    }
}