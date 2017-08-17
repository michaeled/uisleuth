using UISleuth.Widgets;
using Xamarin.Forms;

namespace UISleuth.Reactions
{
    /// <summary>
    /// Associate a <see cref="UIWidget"/> with the target framework's <see cref="VisualElement"/>.
    /// </summary>
    internal class SurfacePair
    {
        /// <summary>
        /// The representation of the target framework's visual elemet. i.e. entry, grid, etc.
        /// </summary>
        public UIWidget UIWidget { get; set; }


        /// <summary>
        /// The target framework's visual element, i.e. view.
        /// </summary>
        public Element VisualElement { get; set; }
    }
}