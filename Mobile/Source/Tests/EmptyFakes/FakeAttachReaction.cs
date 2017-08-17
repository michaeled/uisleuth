using System.Linq;
using UISleuth.Networking;
using UISleuth.Reactions;
using UISleuth.Widgets;
using Xamarin.Forms;

namespace UISleuth.Tests.EmptyFakes
{
    sealed class FakeAttachReaction : InspectorReaction
    {
        public Entry Child { get; set; }


        /// <summary>
        /// Returns true if the attach method succeeded; otherwise, false.
        /// </summary>
        public bool AttachResult { get; set; }


        /// <summary>
        /// Returns true if the hierarchy contains the child's UIWidget
        /// </summary>
        public bool HierarchyContainsChild { get; set; }


        /// <summary>
        /// Return true if the child has been attached to the correct UIWidget
        /// </summary>
        public bool ChildHasCorrectParent { get; set; }


        protected override void OnExecute(UIMessageContext ctx)
        {
            /*
                Document Outline:

                ContentPage
                    StackLayout
                        Entry *(this is done here)*
            */

            AttachResult = Surface.SetParent(Child, Surface.Root.Children[0]);
            var widgets = Surface.Root.GetNodeAndDescendants();
            HierarchyContainsChild = widgets.Any(w => w.Id == Child.Id.ToString());

            if (Surface.Root.Children[0].Children[0].Type == nameof(Entry))
            {
                ChildHasCorrectParent = true;
            }
        }
    }
}