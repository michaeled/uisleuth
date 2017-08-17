using System;
using System.Linq;
using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class GetVisualElementsReaction : InspectorReaction
    {
        public static string[] Ignore =
        {
            "NativeViewWrapper",
            "OpenGLView"
        };

        protected override void OnExecute(UIMessageContext ctx)
        {
            var request = ctx.Get<GetVisualElementsRequest>();
            if (request == null) return;

            var derivedFrom = typeof(Xamarin.Forms.View);
            var assembly = derivedFrom.Assembly;

            var allViewTypes = assembly
                .GetTypes()
                .Where(IsView)
                .Where(t => !Ignore.Any(i => t.FullName.EndsWith(i)))
                .ToArray();

            var views = allViewTypes
                .Where(t => !typeof(Xamarin.Forms.Layout).IsAssignableFrom(t))
                .Select(t => t.FullName)
                .ToArray();

            var layouts = allViewTypes
                .Where(t => typeof(Xamarin.Forms.Layout).IsAssignableFrom(t))
                .Select(t => t.FullName)
                .ToArray();

            var others = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(IsView)
                .Select(t => t.FullName)
                .Except(views)
                .Except(layouts)
                .ToArray();

            ctx.SetResponse<GetVisualElementsResponse>(r =>
            {
                r.Views = views;
                r.Layouts = layouts;
                r.Others = others;
            });
        }

        private bool IsView(Type t)
        {
            if (t == null) return false;

            var viewType = typeof(Xamarin.Forms.View);

            return t != viewType
                   && !t.IsAbstract
                   && t.IsPublic
                   && viewType.IsAssignableFrom(t);
        }
    }
}