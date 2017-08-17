using UISleuth.Messages;
using UISleuth.Widgets;
using Xamarin.Forms;

namespace UISleuth.Reactions
{
    /// <summary>
    /// An operation performed on a page.
    /// </summary>
    internal abstract class InspectorReaction : Reaction
    {
        protected InspectorReaction() { }
        protected InspectorReaction(Element rootElement)
        {
            RootElement = rootElement;
        }

        private SurfaceManager _surfaceManager;
        public SurfaceManager Surface
        {
			get
			{
				return _surfaceManager;
			}
            set
            {
                _surfaceManager = value;
                if (RootElement != null)
                {
                    RootWidget = Surface.SetInspectorSurface(RootElement);
                }
            }
        }

        protected UIWidget RootWidget { get; private set; }
        protected Element RootElement { get; private set; }

        /// <summary>
        /// Create an association between a request and an action to be performed on the designer.
        /// The <paramref name="page"/> parameter will be the root view for the designer action.
        /// </summary>
        /// <typeparam name="TRequestType"></typeparam>
        /// <typeparam name="TReaction"></typeparam>
        /// <param name="page"></param>
        public static void Register<TRequestType, TReaction>(Page page) where TReaction : InspectorReaction, new() where TRequestType : UIMessage
        {
            Register<TRequestType, TReaction>(() => new TReaction { RootElement = page });
        }
    }
}