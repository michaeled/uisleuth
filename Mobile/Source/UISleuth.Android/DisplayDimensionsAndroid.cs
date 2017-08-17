using System;
using Android.App;
using Android.Graphics;

namespace UISleuth.Android
{
    [Activity(Label = "DisplayDimensionsAndroid")]
    internal class DisplayDimensionsAndroid : IDisplayDimensions
    {
        public DisplayDimensions GetDimensions()
        {
            var result = new DisplayDimensions();
            var activity = (Activity) Xamarin.Forms.Forms.Context;
            if (activity == null) return null;

            var size = new Point();
            activity.WindowManager.DefaultDisplay.GetRealSize(size);

            result.Width = size.X;
            result.Height = size.Y;
            result.Density = Math.Round(activity.Resources.DisplayMetrics.Density, 2);

            var statusBarId = activity.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (statusBarId > 0)
            {
                result.StatusBarHeight = activity.Resources.GetDimensionPixelSize(statusBarId);
            }

            var navbarId = activity.Resources.GetIdentifier("navigation_bar_height", "dimen", "android");
            if (navbarId > 0)
            {
                result.NavigationBarHeight = activity.Resources.GetDimensionPixelSize(navbarId);
            }

            return result;
        }
    }
}