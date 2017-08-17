using System;
using System.Diagnostics;
using UISleuth.Messages;
using UISleuth.Networking;

namespace UISleuth.Reactions
{
    internal class TraceEventsReaction : InspectorReaction
    {
        //private static Tracer _tracer;

        protected override void OnExecute(UIMessageContext ctx)
        {
            if (ctx.Request == null) return;

            var req = ctx.Get<TraceEventsRequest>();
            if (req == null) return;

            var pair = Surface[req.WidgetId];

            //_tracer = new Tracer(pair.VisualElement, OnEventTrace);
            //_tracer.HookAllEvents();
        }

        private void OnEventTrace(object sender, object target, string eventName, EventArgs e)
        {
            string s = String.Format("{0} - args {1} - sender {2} - target {3}",
                eventName,
                e.ToString(),
                sender ?? "null",
                target ?? "null");

            Debug.WriteLine(s);
        }
    }
}