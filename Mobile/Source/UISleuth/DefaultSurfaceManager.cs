using System;
using System.Collections.Generic;
using System.Linq;
using UISleuth.Reactions;
using UISleuth.Widgets;
using Xamarin.Forms;

namespace UISleuth
{
    internal class DefaultSurfaceManager : SurfaceManager
    {
        private readonly Dictionary<string, Element> _lookup;


        public DefaultSurfaceManager()
        {
            _lookup = new Dictionary<string, Element>();
        }


        public override SurfacePair this[string id]
        {
            get
            {
                // No hierarchy exists, so we return null.
                if (Root == null)
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(id)) return null;

                Element ve;

                if (_lookup.ContainsKey(id))
                {
                    ve = _lookup[id];
                }
                else
                {
                    return null;
                }

                // Placing the recursive lookup here to save cycles if the view is not "cached"
                var all = Root.GetNodeAndDescendants();
                var match = all.FirstOrDefault(w => w.Id != null && w.Id.Equals(id));

                var result = new SurfacePair
                {
                    UIWidget = match,
                    VisualElement = ve
                };

                return result;
            }
        }


        public override SurfacePair this[Guid id] => this[id.ToString()];


        public override bool Remove(UIWidget widget)
        {
            return Remove(widget.Id);
        }


        public override bool Remove(string id)
        {
            var pair = this[id];
            if (pair == null) return false;

            if (pair.UIWidget == Root)
            {
                throw new InvalidOperationException($"You cannot remove the root widget through this method. Call {nameof(SetInspectorSurface)} instead.");
            }

            var uiWidgetRemoved = RemoveUIWidget(pair);
            var xamRemoved = RemoveVisualElement(pair);

            return uiWidgetRemoved && xamRemoved;
        }


        public override UIWidget SetInspectorSurface(Element view)
        {
            ContentPage page = null;

            var tabbedPage = view as TabbedPage;
            if (tabbedPage != null)
            {
                var currentPage = tabbedPage.CurrentPage as ContentPage;
                if (currentPage != null)
                {
                    page = currentPage;
                }
            }
            else if (view is ContentPage)
            {
                page = (ContentPage) view;
            }

            if (page == null)
            {
                Root = null;
                return null;
            }

            var widget = CreateUIWidget(view, null);
            widget.Name = page.Title;
            Root = widget;

            // you can't remove the root widget
            Root.CanDelete = false;

            // the root widget must have children
            Root.IsLayout = true;

            // short-circuit if there's no children to find.
            if (page.Content == null)
            {
                return widget;
            }

            // Create the hierarchy from the views that are already attached to the page.
            Root = BuildTree(page.Content, widget);

            return Root;
        }


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
        public override bool SetParent(Element child, Element parent, int index = 0)
        {
            if (child == null || parent == null) return false;

            if (!Contains(parent))
            {
                throw new InvalidOperationException("The destination must have a corresponding UIWidget attached to the hierarchy.");
            }

            var parentType = parent.GetType();
            var parentPair = this[parent.Id];
            var childView = child as View;

            var page = parentPair.VisualElement as ContentPage;

            // Check if the parent is the Root ContentPage.
            if (page != null && page == parentPair.VisualElement)
            {
                if (childView == null) return false;
                page.Content = childView;
                SetInspectorSurface(page);
                return true;
            }

            // Check if it's possible to add and remove child views from this container type.
            var parentContainer = parent as IViewContainer<View>;
            if (parentContainer != null)
            {
                var result = BuildTree(child, parentPair.UIWidget);
                if (result == null) return false;

                parentContainer.Children.Insert(index, childView);
                return true;
            }

            // check if the parent has a content property, like a ScrollView or ContentView
            var contentAttribute = parentType.GetCustomAttributes(typeof (ContentPropertyAttribute), true);
            if (contentAttribute.Length > 0)
            {
                var attribute = contentAttribute[0] as ContentPropertyAttribute;

                // Check that the Content property name is set.
                if (!string.IsNullOrWhiteSpace(attribute?.Name))
                {
                    var property = parentType.GetProperty(attribute.Name);
                    if (property.PropertyType == typeof(View))
                    {
                        var result = BuildTree(child, parentPair.UIWidget);

                        if (result == null) return false;
                        property.SetValue(parent, childView);
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Attach the child view to the parent.
        /// Once attached, the visual tree will be rebuilt.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns>
        /// Returns true if attached; otherwise, false.
        /// </returns>
        public override bool SetParent(Element child, UIWidget parent)
        {
            if (child == null || parent == null) return false;

            // Returns null when id not found.
            var pair = this[parent.Id];
            if (pair == null) return false;

            return SetParent(child, pair.VisualElement);
        }


        /// <summary>
        /// Attach the child view to the parent.
        /// Once attached, the visual tree will be rebuilt.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns>
        /// Returns true if attached; otherwise, false.
        /// </returns>
        public override bool SetParent(Element child, SurfacePair parent)
        {
            return SetParent(child, parent.VisualElement);
        }


        public override bool Contains(UIWidget widget)
        {
            if (string.IsNullOrWhiteSpace(widget?.Id)) return false;
            return _lookup.ContainsKey(widget.Id);
        }


        public override bool Contains(Element view)
        {
            if (view == null) return false;
            var id = view.Id.ToString();

            return _lookup.ContainsKey(id);
        }


        protected override UIWidget CreateUIWidget(Element source, UIWidget parent)
        {
            if (source == null) return null;

            var id = source.Id.ToString();
            if (!_lookup.ContainsKey(id))
            {
                _lookup.Add(id, source);
            }

            var dimensions = new UIWidgetDimensions();
            
            var widget = new UIWidget
            {
                Id = id,
                Parent = parent,
                Type = source.GetType().Name,
                FullTypeName = source.GetType().FullName,
                Name = "Unknown",
                CanDelete = parent != null,
                IsDatabound = source.BindingContext != null
            };

            if (source is VisualElement)
            {
				var ve = source as VisualElement;
                dimensions = new UIWidgetDimensions
                {
                    Height = Math.Round(ve.Height, 1),
                    Width = Math.Round(ve.Width, 1),
                    X = Math.Round(GetX(ve), 1),
                    Y = Math.Round(GetY(ve), 1)
                };
            }

            if (source is Cell)
            {
				var cell = source as Cell;
                dimensions = new UIWidgetDimensions
                {
                    Height = Math.Round(cell.RenderHeight, 1),
                };

                widget.IsCell = true;
                widget.CanDelete = false;
            }

            widget.Dimensions = dimensions;
            return widget;
        }


        public bool RemoveVisualElement(SurfacePair pair)
        {
            // we don't allow the root element to be removed.
            if (pair.UIWidget.Parent == null) return false;

            var parentId = pair.UIWidget.Parent.Id;
            var parentPair = this[parentId];

            if (parentPair == null) return false;

            // this is the only child of the content page
            if (parentId == Root.Id)
            {
                var page = parentPair.VisualElement as ContentPage;
                if (page != null)
                {
                    page.Content = null;
                    return true;
                }
            }

            // this maybe a layout control
            var viewContainer = parentPair.VisualElement as IViewContainer<View>;
            var childView = pair.VisualElement as View;
            var parentView = parentPair.VisualElement;

            // we can only remove view types; this shouldn't happen.
            if (childView == null) return false;

            // is this a content property?
            if (viewContainer == null && ElementHelper.HasContentProperty(parentView))
            {
                ElementHelper.ClearContentProperty(parentView);
                return true;
            }

            // is it a layout control? i.e. stacklayout, grid..
            if (viewContainer == null) return false;

            // made it this far, it's a layout control.
            viewContainer.Children.Remove(childView);
            return true;
        }


        protected override UIWidget BuildTree(Element childView, UIWidget parentWidget)
        {
            /* 
                1. Return parent widget if child view is null.
                Since, We won't be able to create a UIWidget out of the view.
            */

            if (childView == null)
            {
                return parentWidget;
            }

            /*
                2. Create a UIWidget for the child view.
            */

            var childWidget = CreateUIWidget(childView, parentWidget);
            parentWidget.Children.Add(childWidget);

            if (ElementHelper.HasContentProperty(childView))
            {
                childWidget.AllowsManyChildren = ElementHelper.ContentPropertyAllowsManyChildren(childView);
                childWidget.IsContentPropertyViewType = ElementHelper.IsContentPropertyView(childView);
                childWidget.ContentPropertyTypeName = ElementHelper.GetContentPropertyTypeName(childView);
                childWidget.HasContentProperty = true;
                childWidget.IsLayout = false;
            }
            else
            {
                childWidget.HasContentProperty = false;
            }

            /*
                3. Determine if the child view has children of it's own.
            */
   
            var isParentView = childView as ILayoutController;
            if (isParentView != null)
            {
                childWidget.IsLayout = true;
            }

            // ListView check
            IReadOnlyList<Element> grandChildren;
            var isListView = childView as ITemplatedItemsView<Cell>;

            if (isListView == null)
            {
                grandChildren = isParentView?.Children;
            }
            else
            {
                grandChildren = isListView.TemplatedItems;
            }

            // TableView check
            if (grandChildren == null && childView is TableView)
            {
				var tableView = childView as TableView;
                var cells = new List<Element>();

                foreach (var section in tableView.Root)
                {
                    foreach (var cell in section)
                    {
                        cells.Add(cell);
                    }
                }

                grandChildren = cells.ToArray();
            }

            // no children
            if (grandChildren == null || grandChildren.Count == 0)
            {
                return parentWidget;
            }

            /*
                4. The child does have it's own children.
            */

            foreach (var grandChild in grandChildren)
            {
                if (grandChild != null)
                {
                    if (grandChild is ILayoutController)
                    {
                        BuildTree(grandChild, childWidget);
                    }
                    else if (grandChild is ListView)
                    {
                        BuildTree(grandChild, childWidget);
                    }
                    else if (grandChild is TableView)
                    {
						var grandTable = grandChild as TableView;
                        var tableWidget = CreateUIWidget(grandChild, childWidget);

                        childWidget.Children.Add(tableWidget);

                        foreach (var section in grandTable.Root)
                        {
                            foreach (var cell in section)
                            {
                                BuildTree(cell, tableWidget);
                            }
                        }
                    }
                    else if (grandChild is ViewCell)
                    {
                        var grandViewCell = grandChild as ViewCell;
                        var grandViewCellWidget = CreateUIWidget(grandChild, childWidget);

                        childWidget.Children.Add(grandViewCellWidget);

                        if (grandViewCell.View != null)
                        {
                            BuildTree(grandViewCell.View, grandViewCellWidget);
                        }
                    }
                    else
                    {
                        var grandChildWidget = CreateUIWidget(grandChild, childWidget);

                        if (ElementHelper.HasContentProperty(grandChild))
                        {
                            grandChildWidget.IsLayout = true;
                        }

                        childWidget.Children.Add(grandChildWidget);
                    }
                }
            }

            return parentWidget;
        }


        private bool RemoveUIWidget(SurfacePair pair)
        {
            var child = pair.UIWidget;
            var parent = child.Parent;

            if (parent == null) return false;
            if (!parent.Children.Contains(child)) return false;

            parent.Children.Remove(child);
            return true;
        }


        private double GetX(VisualElement element)
        {
            if (element == null) return 0;

            var x = element.X;

            if (!(element.Parent is VisualElement))
            {
                return x;
            }

            var parent = (VisualElement)element.Parent;

            while (parent != null)
            {
                x += parent.X;

                if (parent is ScrollView)
                {
                    var sv = (ScrollView)parent;
                    x += sv.ScrollX;
                }

                if (parent.Parent is VisualElement)
                {
                    parent = (VisualElement) parent.Parent;
                }
                else
                {
                    break;
                }
            }

            return x;
        }


        private double GetY(VisualElement element)
        {
            if (element == null) return 0;

            var y = element.Y;

            if (!(element.Parent is VisualElement))
            {
                return y;
            }

            var parent = (VisualElement)element.Parent;
            while (parent != null)
            {
                y += parent.Y;

                if (parent is ScrollView)
                {
                    var sv = (ScrollView)parent;
                    y += sv.ScrollY;
                }

                if (parent.Parent is VisualElement)
                {
                    parent = (VisualElement)parent.Parent;
                }
                else
                {
                    break;
                }
            }

            return y;
        }
    }
}