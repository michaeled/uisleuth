using System;
using NUnit.Framework;
using Xamarin.Forms;

namespace UISleuth.Tests.SurfaceManager
{
    [TestFixture]
    public class TestDesignSurfaceManager
    {
        [Test]
        public void ContentView_HasContentProperty()
        {
            var cv = new ContentView();
            var page = new ContentPage { Content = cv };
            var mgr = new DefaultSurfaceManager();
            var root = mgr.SetInspectorSurface(page);

            var pair = mgr[cv.Id.ToString()];
            Assert.IsTrue(pair.UIWidget.HasContentProperty);
            Assert.IsTrue(pair.UIWidget.IsContentPropertyViewType);
            Assert.AreEqual(pair.UIWidget.ContentPropertyTypeName, typeof(View).FullName);
        }


        [Test]
        public void StackLayout_IsLayout_no_ContentProperty()
        {
            var stack = new StackLayout();
            var page = new ContentPage { Content = stack };
            var mgr = new DefaultSurfaceManager();
            var root = mgr.SetInspectorSurface(page);

            var pair = mgr[stack.Id.ToString()];
            Assert.AreEqual("System.Collections.IEnumerable<Xamarin.Forms.View>", pair.UIWidget.ContentPropertyTypeName);
            Assert.IsTrue(pair.UIWidget.IsLayout);
            Assert.IsTrue(pair.UIWidget.IsContentPropertyViewType);
            Assert.IsTrue(pair.UIWidget.HasContentProperty);
        }


        [Test]
        public void String_Content_isnt_Layout()
        {
            var label = new Label { Text = "value" };
            var page = new ContentPage { Content = label };
            var mgr = new DefaultSurfaceManager();
            var root = mgr.SetInspectorSurface(page);

            var pair = mgr[label.Id.ToString()];
            Assert.IsFalse(pair.UIWidget.IsLayout);
            Assert.IsTrue(pair.UIWidget.HasContentProperty);
            Assert.IsFalse(pair.UIWidget.IsContentPropertyViewType);
            Assert.AreEqual(typeof (string).FullName, pair.UIWidget.ContentPropertyTypeName);
        }


        [Test]
        public void Button_has_no_children_and_no_ContentProperty()
        {
            var btn = new Button { Text = "value" };
            var page = new ContentPage { Content = btn };
            var mgr = new DefaultSurfaceManager();
            var root = mgr.SetInspectorSurface(page);

            var pair = mgr[btn.Id.ToString()];
            Assert.IsFalse(pair.UIWidget.IsLayout);
            Assert.IsFalse(pair.UIWidget.HasContentProperty);
            Assert.IsFalse(pair.UIWidget.IsContentPropertyViewType);
            Assert.AreEqual(null, pair.UIWidget.ContentPropertyTypeName);
        }


        [Test]
        public void When_setting_design_surface_Root_is_set_and_returned()
        {
            var mgr = new DefaultSurfaceManager();
            var page = new ContentPage();

            var root = mgr.SetInspectorSurface(page);

            Assert.AreEqual(root, mgr.Root);
        }


        [Test]
        public void When_setting_design_surface_the_page_becomes_the_root_widget()
        {
            var mgr = new DefaultSurfaceManager();
            var page = new ContentPage();

            var root = mgr.SetInspectorSurface(page);

            Assert.AreEqual(page.Id.ToString(), root.Id, "Ids aren't equal.");
        }


        [Test]
        public void When_setting_design_surface_the_UIWidget_type_properties_are_set()
        {
            const string title = "Some title.";
            var mgr = new DefaultSurfaceManager();

            var page = new ContentPage
            {
                Title = title
            };

            var root = mgr.SetInspectorSurface(page);

            Assert.AreEqual(root.Id, page.Id.ToString(), "Ids aren't equal.");
            Assert.AreEqual(root.FullTypeName, page.GetType().FullName, "Full type names aren't equal.");
            Assert.AreEqual(root.Type, page.GetType().Name, "Short type names aren't equal.");
            Assert.AreEqual(root.Name, title, "The titles aren't equal.");
        }


        [Test]
        public void Can_lookup_UIWidget_by_id()
        {
            var label = new Label {Text = "textvalue"};
            var page = new ContentPage
            {
                Content = label
            };

            var mgr = new DefaultSurfaceManager();
            var root = mgr.SetInspectorSurface(page);

            var pagePair = mgr[page.Id.ToString()];
            var labelPair = mgr[label.Id.ToString()];

            Assert.AreEqual(root, pagePair.UIWidget, "The root and page aren't equal.");
            Assert.AreEqual(pagePair.VisualElement, page, "The page wasn't returned.");
            Assert.AreEqual(labelPair.VisualElement, label, "The label wasn't returned.");
            Assert.AreEqual(labelPair.UIWidget.Id, label.Id.ToString(), "The label Ids aren't equal.");
        }


        [Test]
        public void Can_lookup_UIWidget_by_guid()
        {
            var page = new ContentPage();
            var mgr = new DefaultSurfaceManager();

            mgr.SetInspectorSurface(page);
            var match = mgr[page.Id];

            Assert.IsNotNull(match, "Match was null.");
            Assert.AreEqual(match.UIWidget.Id, page.Id.ToString());
        }


        [Test]
        public void When_widget_isnt_found_during_lookup_return_null()
        {
            var page = new ContentPage();
            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var pair = mgr["12345"];
            Assert.IsNull(pair);
        }


        [Test]
        public void Should_return_null_on_invalid_input()
        {
            var traverser = new DefaultSurfaceManager();

            // Act
            var nullArg = traverser[null];
            Assert.IsNull(nullArg, "Null returns null.");

            var empty = traverser[""];
            Assert.IsNull(empty, "Empty returns null.");

            var whitespace = traverser[" "];
            Assert.IsNull(whitespace, "Whitespace returns null.");
        }


        [Test]
        [Description("The client should prevent this action from happening.")]
        public void Removing_root_UIWidget()
        {
            var page = new ContentPage();
            var mgr = new DefaultSurfaceManager();
            var root = mgr.SetInspectorSurface(page);

            Assert.Throws<InvalidOperationException>(() => mgr.Remove(root));
        }


        [Test]
        public void Should_remove_only_design_surface_child_by_using_its_UIWidget()
        {
            var label = new Label {Text = "value"};
            var page = new ContentPage {Content = label};
            var mgr = new DefaultSurfaceManager();
            var root = mgr.SetInspectorSurface(page);

            var labelPair = mgr[label.Id.ToString()];
            var removed = mgr.Remove(labelPair.UIWidget);

            Assert.IsTrue(removed, "Removed wasn't true.");
            Assert.IsEmpty(mgr.Root.Children, "mgr.Root.Children should be empty.");
            Assert.IsEmpty(root.Children, "root.Children should be empty.");
            Assert.IsNull(page.Content, "Page content should be null.");
        }


        [Test]
        public void Should_remove_child_of_stacklayout_by_using_its_UIWidget()
        {
            var child = new Entry {Placeholder = "text"};
            var layout = new StackLayout();
            layout.Children.Add(child);
            var page = new ContentPage {Content = layout};

            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var childPair = mgr[child.Id.ToString()];
            var layoutPair = mgr[layout.Id.ToString()];

            Assert.IsNotEmpty(layoutPair.UIWidget.Children, "The stacklayouts UIWidget should have children.");

            var removed = mgr.Remove(childPair.UIWidget);

            Assert.IsTrue(removed, "Removed wasn't true.");
            Assert.IsEmpty(layout.Children, "The stacklayout has children.");
            Assert.IsEmpty(layoutPair.UIWidget.Children, "The stacklayout has children.");
        }


        [Test]
        public void Calling_Contains_should_return_true_when_UIWidget_is_a_descendant_of_root()
        {
            var label = new Label();
            var child = new Entry { Placeholder = "text" };
            var layout = new StackLayout();
            layout.Children.Add(child);
            var page = new ContentPage { Content = layout };

            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var childPair = mgr[child.Id.ToString()];
            var containsWidget = mgr.Contains(childPair.UIWidget);
            var containsVisual = mgr.Contains(childPair.VisualElement);
            var containsLabel = mgr.Contains(label);

            Assert.IsTrue(containsWidget, "UIWidget not found.");
            Assert.IsTrue(containsVisual, "VisualElement not found.");
            Assert.IsFalse(containsLabel, "Label was not added to the page.");
        }


        [Test]
        public void SetParent_page_is_parent_and_label_is_child()
        {
            var label = new Label {Text = "value"};
            var page = new ContentPage();
            var mgr = new DefaultSurfaceManager();

            mgr.SetInspectorSurface(page);

            var parented = mgr.SetParent(label, page);
            Assert.IsTrue(parented, "SetParent returned false.");
            Assert.AreEqual(1, mgr.Root.Children.Count, "Expected one child.");
            Assert.AreEqual(label.Id.ToString(), mgr.Root.Children[0].Id, "The label widget wasn't added to the children collection.");
        }


        [Test]
        public void SetParent_stacklayout_is_parent_and_entry_is_child()
        {
            var child = new Entry();
            var layout = new StackLayout();
            var page = new ContentPage();
            var mgr = new DefaultSurfaceManager();

            page.Content = layout;
            mgr.SetInspectorSurface(page);

            var parented = mgr.SetParent(child, layout);

            Assert.IsTrue(parented, "Parented returned false.");

            var layoutWidget = mgr[layout.Id.ToString()];
            Assert.IsNotEmpty(layoutWidget.UIWidget.Children, "UIWidget did not have any children.");
            Assert.IsNotEmpty(layout.Children, "StackLayout view did not contain children.");
        }


        [Test]
        public void SetParent_entry_is_parent_and_label_is_child()
        {
            var child = new Label();
            var entry = new Entry();
            var page = new ContentPage();
            var mgr = new DefaultSurfaceManager();

            page.Content = entry;
            mgr.SetInspectorSurface(page);

            var parented = mgr.SetParent(child, entry);
            Assert.IsFalse(parented, "Parented should be false.");
            Assert.AreEqual(mgr.Root.Children[0].Id, entry.Id.ToString(), "The content view has changed.");
        }


        [Test]
        public void SetParent_ContentView_is_parent_and_label_is_child()
        {
            var child = new Label();
            var content = new ContentView();
            var page = new ContentPage {Content = content};
            var mgr = new DefaultSurfaceManager();

            mgr.SetInspectorSurface(page);
            var parented = mgr.SetParent(child, content);

            Assert.IsTrue(parented, "Parented should be true.");

            var contentPair = mgr[content.Id.ToString()];
            Assert.IsNotEmpty(contentPair.UIWidget.Children, "Content children should have an element.");
            Assert.AreEqual(contentPair.UIWidget.Children[0].Id, child.Id.ToString(), "The wrong child was attached.");
        }


        [Test]
        public void SetParent_ScrollView_is_parent_and_entry_is_child()
        {
            var child = new Label();
            var content = new ScrollView();
            var page = new ContentPage { Content = content };
            var mgr = new DefaultSurfaceManager();

            mgr.SetInspectorSurface(page);
            var parented = mgr.SetParent(child, content);

            Assert.IsTrue(parented, "Parented should be true.");

            var contentPair = mgr[content.Id.ToString()];
            Assert.IsNotEmpty(contentPair.UIWidget.Children, "Content children should have an element.");
            Assert.AreEqual(contentPair.UIWidget.Children[0].Id, child.Id.ToString(), "The wrong child was attached.");
        }


        [Test]
        public void ListView_should_have_three_children_returned()
        {
            var lv = new ListView
            {
                ItemsSource = new[]
                {
                    "1",
                    "2",
                    "3"
                }
            };

            var page = new ContentPage { Content = lv };

            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var pair = mgr[lv.Id];

            Assert.AreEqual(3, pair.UIWidget.Children.Count);
            Assert.AreEqual(typeof(TextCell).FullName, pair.UIWidget.Children[0].FullTypeName);
        }


        [Test]
        public void ListView_Cells_should_be_recognized()
        {
            var lv = new ListView
            {
                ItemsSource = new[]
                {
                    "1",
                    "2",
                    "3"
                }
            };

            var page = new ContentPage { Content = lv };

            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var pair = mgr[lv.Id];
            Assert.AreEqual(3, pair.UIWidget.Children.Count);

            Assert.IsFalse(pair.UIWidget.IsCell);
            Assert.IsTrue(pair.UIWidget.Children[0].IsCell);
            Assert.IsTrue(pair.UIWidget.Children[1].IsCell);
            Assert.IsTrue(pair.UIWidget.Children[2].IsCell);
        }

        [Test]
        public void TableView_ImageCells_should_be_recognized()
        {
            var page = new ImageCellTableViewPage();
            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var table = mgr[page.Table.Id];
            Assert.IsNotNull(table);

            var cell = table.UIWidget.Children[0];
            Assert.IsNotNull(cell);
            Assert.AreEqual(typeof(ImageCell).FullName, cell.FullTypeName);
        }

        [Test]
        public void ListView_Custom_cells_should_be_recognized()
        {
            var page = new ImageCellPage();
            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var listview = mgr[page.ListView.Id];
            Assert.IsNotNull(listview);

            var cell = listview.UIWidget.Children[0];
            Assert.IsNotNull(cell);
            Assert.AreEqual(typeof(CustomCell).FullName, cell.FullTypeName);
        }

        [Test]
        public void ListView_ViewCell_children_recognized()
        {
            var page = new ImageCellPage();
            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var listview = mgr[page.ListView.Id];
            Assert.IsNotNull(listview);

            var cell = listview.UIWidget.Children[0];

            Assert.IsNotNull(cell);
            Assert.IsTrue(cell.Children.Count > 0, "cell.Children.Count > 0");
            Assert.AreEqual("Xamarin.Forms.StackLayout", cell.Children[0].FullTypeName);
            Assert.IsTrue(cell.Children[0].Children.Count > 0, "cell.Children[0].Children.Count > 0");
        }

        [Test]
        public void ListView_ViewCells_in_StackLayout_recognized()
        {
            var page = new TestDataSelectorPage();
            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var listview = mgr[page.ListView.Id];
            Assert.IsNotNull(listview);

            var cell = listview.UIWidget.Children[0];
            Assert.IsNotNull(cell);
            Assert.IsTrue(cell.Children.Count > 0, "cell.Children.Count > 0");
        }

        [Test]
        public void Cannot_delete_cells()
        {
            var page = new ImageCellPage();
            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var listview = mgr[page.ListView.Id];
            Assert.IsNotNull(listview);

            var cell = listview.UIWidget.Children[0];
            Assert.IsNotNull(cell);
            Assert.AreEqual(typeof(CustomCell).FullName, cell.FullTypeName);

            foreach (var child in listview.UIWidget.Children)
            {
                Assert.IsFalse(child.CanDelete);
            }
        }

        [Test]
        public void Test_IsDatabound_True()
        {
            object ctx = new
            {
                Value1 = "123"
            };

            var page = new ContentPage
            {
                BindingContext = ctx
            };

            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var pair = mgr[page.Id];

            Assert.IsNotNull(pair);
            Assert.IsTrue(pair.UIWidget.IsDatabound);
        }

        [Test]
        public void Test_IsDatabound_False()
        {
            var page = new ContentPage
            {
            };

            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var pair = mgr[page.Id];

            Assert.IsNotNull(pair);
            Assert.IsFalse(pair.UIWidget.IsDatabound);
        }

        [Test]
        public void TableView_as_only_child_should_be_recognized()
        {
            var table = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot
                {
                    new TableSection
                    {
                        new ImageCell(),
                        new SwitchCell(),
                        new EntryCell()
                    }
                }
            };

            var page = new ContentPage
            {
                Content = table
            };

            var mgr = new DefaultSurfaceManager();
            mgr.SetInspectorSurface(page);

            var pair = mgr[table.Id];

            Assert.IsNotNull(pair);
            Assert.AreEqual(3, pair.UIWidget.Children.Count);

            Assert.AreEqual(typeof(ImageCell).FullName, pair.UIWidget.Children[0].FullTypeName);
            Assert.AreEqual(typeof(SwitchCell).FullName, pair.UIWidget.Children[1].FullTypeName);
            Assert.AreEqual(typeof(EntryCell).FullName, pair.UIWidget.Children[2].FullTypeName);
        }
    }
}
