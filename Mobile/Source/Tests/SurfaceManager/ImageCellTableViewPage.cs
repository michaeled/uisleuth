using System;
using Xamarin.Forms;

namespace UISleuth.Tests.SurfaceManager
{
    public class ImageCellTableViewPage : ContentPage
    {
        public TableView Table;

        public ImageCellTableViewPage()
        {
            Label header = new Label
            {
                Text = "ImageCell",
                HorizontalOptions = LayoutOptions.Center
            };

            Table = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot
                {
                    new TableSection
                    {
                        new ImageCell
                        {
                            // Some differences with loading images in initial release.
                            ImageSource = null,
                            Text = "This is an ImageCell",
                            Detail = "This is some detail text",
                        }
                    }
                }
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children =
                {
                    header,
                    Table
                }
            };
        }
    }
}
