using System.Collections.Generic;
using UISleuth.Messages;
using UISleuth.Networking;
using UISleuth.Widgets;
using UISleuth;

namespace UISleuth.Reactions
{
    /// <summary>
    /// All types known by the client are contained in a <see cref="SupportedTypesRequest"/>.
    /// </summary>
    internal class SupportedTypesReaction : Reaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<SupportedTypesRequest>();
            if (request == null) return;

            TypeRegistrar.Instance.Types = new HashSet<UIType>(request.Types);
        }
    }
}