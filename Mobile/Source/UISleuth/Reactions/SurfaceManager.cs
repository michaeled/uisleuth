using System;
using UISleuth.Widgets;
using Xamarin.Forms;

namespace UISleuth.Reactions
{
    /// <summary>
    /// <see cref="Reaction"/>'s can walk the visual tree, or document outline, of the design surface.
    /// When targeting a UI framework, this class must be extended with the details that are
    /// specific to the targeted UI framework.
    /// </summary>
    internal abstract class SurfaceManager
    {
        /// <summary>
        /// A reference to the top-most UI element, usually a page.
        /// </summary>
        public UIWidget Root { get; set; }


        /// <summary>
        /// Checks whether or not the <paramref name="view"/> is a descendant of <see cref="Root"/>.
        /// </summary>
        /// <param name="view"></param>.
        /// <returns></returns>
        public abstract bool Contains(Element view);


        /// <summary>
        /// Checks whether or not the <paramref name="widget"/> is a descendant of <see cref="Root"/>.
        /// </summary>
        /// <param name="widget"></param>.
        /// <returns></returns>
        public abstract bool Contains(UIWidget widget);


        /// <summary>
        /// Return a <see cref="SurfacePair"/> for the requested <paramref name="id"/>
        /// </summary>
        /// <param name="id">The <see cref="UIWidget.Id"/></param>
        /// <returns>
        /// The return type includes the <see cref="UIWidget"/> and target framework's base widget type, i.e. VisualElement, View, etc.
        /// </returns>
        public abstract SurfacePair this[string id] { get; }


        /// <summary>
        /// Return a <see cref="SurfacePair"/> for the requested <paramref name="id"/>
        /// </summary>
        /// <param name="id">The <see cref="UIWidget.Id"/></param>
        /// <returns>
        /// The return type includes the <see cref="UIWidget"/> and target framework's base widget type, i.e. VisualElement, View, etc.
        /// </returns>
        public abstract SurfacePair this[Guid id] { get; }


        /// <summary>
        /// Remove the <paramref name="widget"/> from the visual tree.
        /// </summary>
        /// <param name="widget"></param>
        /// <returns>
        /// Returns true if the <paramref name="widget"/> has been removed; otherwise, false.
        /// </returns>
        public abstract bool Remove(UIWidget widget);


        /// <summary>
        /// Remove the <see cref="UIWidget"/> from the visual tree.
        /// </summary>
        /// <returns>
        /// Returns true if the <see cref="id"/> is a tracked <see cref="UIWidget"/> and it has been removed
        /// from the visual tree; otherwise, false.
        /// </returns>
        public abstract bool Remove(string id);


        /// <summary>
        /// Using the <paramref name="view"/> as the root element of a visual tree, recursively record and traverse
        /// each child element.
        /// </summary>
        /// <param name="view">The top-most parent widget. Usually a "page" type that can be pushed on a navigation stack.</param>
        /// <returns>
        /// Returns the top-most element of a visual tree.
        /// </returns>
        public abstract UIWidget SetInspectorSurface(Element view);


        /// <summary>
        /// Attach the child view to the parent.
        /// Once attached, the visual tree will be rebuilt.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <returns>
        /// Returns true if attached; otherwise, false.
        /// </returns>
        public abstract bool SetParent(Element child, Element parent, int index = 0);


        /// <summary>
        /// Attach the child to the parent object.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns>
        /// Returns true if the view was attached to the parent; otherwise, false.
        /// </returns>
        public abstract bool SetParent(Element child, UIWidget parent);


        /// <summary>
        /// Attach the child to the parent object.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns>
        /// Returns true if the view was attached to the parent; otherwise, false.
        /// </returns>
        public abstract bool SetParent(Element child, SurfacePair parent);


        /// <summary>
        /// The target framework implementations should define this method.
        /// Using the <paramref name="source"/> the method should instantiate and return a <see cref="UIWidget"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected abstract UIWidget CreateUIWidget(Element source, UIWidget parent);


        /// <summary>
        /// Using the <paramref name="childView"/> as a starting element, recursively record and traverse each child element.
        /// </summary>
        /// <param name="childView">The target framework's UI element.</param>
        /// <param name="parentWidget">The <paramref name="childView"/>'s parent.</param>
        /// <returns>
        /// Returns the representation of the <paramref name="childView" />.
        /// </returns>
        protected abstract UIWidget BuildTree(Element childView, UIWidget parentWidget);
    }
}