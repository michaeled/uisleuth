using Xamarin.Forms;

namespace UISleuth.iOS.App
{
    public class CustomModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Image { get; set; }
    }

    internal class App : Xamarin.Forms.Application
    {
        internal App()
        {
            var p1btn1 = new Button
            {
                Text = "push 1"
            };

            var p2btn1 = new Button
            {
                Text = "pop 2"
            };

            var p2btn2 = new Button
            {
                Text = "push 3"
            };

            var p3btn1 = new Button
            {
                Text = "pop 3"
            };

            var page1 = new ContentPage
            {
                Content = p1btn1
            };

            var model = new CustomModel
            {
                Subtitle = "subtitle 12323",
                Title = "title 922",
                Image = "zzz"
            };

            var lbl = new Label
            {
                Text = "Label...",
                BindingContext = model
            };

            var page2 = new ContentPage
            {
                Content = new StackLayout
                {
                    Children =
                    {
                        lbl,
                        p2btn1,
                        p2btn2
                    }
                }
            };

            var page3 = new ContentPage
            {
                Content = new StackLayout
                {
                    Children =
                    {
                        p3btn1
                    }
                }
            };

            p1btn1.Clicked += (sender, args) =>
            {
                Xamarin.Forms.Application.Current.MainPage.Navigation.PushModalAsync(page2);
            };

            p2btn1.Clicked += (sender, args) =>
            {
                Xamarin.Forms.Application.Current.MainPage.Navigation.PopModalAsync();
            };

            p2btn2.Clicked += (sender, args) =>
            {
                Xamarin.Forms.Application.Current.MainPage.Navigation.PushModalAsync(page3);
            };

            p3btn1.Clicked += (sender, args) =>
            {
                Xamarin.Forms.Application.Current.MainPage.Navigation.PopModalAsync();
            };

            MainPage = page1;

            //MainPage = new ContentPage
            //{
            //    Content = new WebView
            //    {
            //        Source = "http://xamarin.com"
            //    }
            //};
        }
    }
}