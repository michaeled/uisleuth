using System.Diagnostics;
using Xamarin.Forms;

namespace UISleuth.Tests.SurfaceManager
{
    public class CustomModel
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public string image { get; set; }
    }

    public class CustomCell : ViewCell
    {
        public CustomCell()
        {
            //instantiate each of our views
            var image = new Image();
            StackLayout cellWrapper = new StackLayout();
            StackLayout horizontalLayout = new StackLayout();
            Label left = new Label();
            Label right = new Label();

            //set bindings
            left.SetBinding(Label.TextProperty, "title");
            right.SetBinding(Label.TextProperty, "subtitle");
            image.SetBinding(Image.SourceProperty, "image");

            //Set properties for desired design
            cellWrapper.BackgroundColor = Color.FromHex("#eee");
            horizontalLayout.Orientation = StackOrientation.Horizontal;
            right.HorizontalOptions = LayoutOptions.EndAndExpand;
            left.TextColor = Color.FromHex("#f35e20");
            right.TextColor = Color.FromHex("503026");

            //add views to the view hierarchy
            horizontalLayout.Children.Add(image);
            horizontalLayout.Children.Add(left);
            horizontalLayout.Children.Add(right);
            cellWrapper.Children.Add(horizontalLayout);
            View = cellWrapper;

            var moreAction = new MenuItem { Text = "More" };
            moreAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            moreAction.Clicked += (sender, e) => {
                var mi = ((MenuItem)sender);
                Debug.WriteLine("More Context Action clicked: " + mi.CommandParameter);
            };

            var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
            deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            deleteAction.Clicked += (sender, e) => {
                var mi = ((MenuItem)sender);
                Debug.WriteLine("Delete Context Action clicked: " + mi.CommandParameter);
            };
            // add to the ViewCell's ContextActions property
            ContextActions.Add(moreAction);
            ContextActions.Add(deleteAction);
        }
    }

    public class ImageCellPage : ContentPage
    {
        public ListView ListView;

        public ImageCellPage()
        {
            ListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(CustomCell)),
                ItemsSource = new[]
                {
                    new CustomModel
                    {
                        subtitle = "subtitle 123...",
                        title = "title 234.."
                    }
                }
            };


            Content = ListView;
        }
    }
}
