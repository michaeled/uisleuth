using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace UISleuth.Android.App.Tests.CanvasSelecting
{
    public class StackLayoutPage : ContentPage
    {
        public StackLayoutPage()
        {
            var stack = new StackLayout();
            var b1 = new Button();
            var b2 = new Button();
            var b3 = new Button();
            var b4 = new Button();

            stack.Children.Add(b1);
            stack.Children.Add(b2);
            stack.Children.Add(b3);
            stack.Children.Add(b4);

            Content = stack;
        }
    }
}