using System;
using UIKit;

namespace UISleuth.iOS
{
    internal class DisplayDimensionsIOS : IDisplayDimensions
    {
        public DisplayDimensions GetDimensions()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;

			var scale = (int) UIScreen.MainScreen.Scale;

            return new DisplayDimensions
            {
                StatusBarHeight = 0,
                Density = scale,
				Height = (int) UIScreen.MainScreen.Bounds.Height * scale,
                Width = (int) UIScreen.MainScreen.Bounds.Width * scale
            };
        }
    }
}