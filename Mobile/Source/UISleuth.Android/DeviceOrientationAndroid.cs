using Android.App;
using Android.Content.PM;

namespace UISleuth.Android
{
    internal class DeviceOrientationAndroid : IDeviceOrientation
    {
        public void SetOrientation(UIDeviceOrientation orientation)
        {
            var activity = (Activity) Xamarin.Forms.Forms.Context;
            if (activity == null) return;

            switch (orientation)
            {
                case UIDeviceOrientation.Landscape:
                    activity.RequestedOrientation = ScreenOrientation.Landscape;
                    break;
                case UIDeviceOrientation.ReverseLandscape:
                    activity.RequestedOrientation = ScreenOrientation.ReverseLandscape;
                    break;
                case UIDeviceOrientation.Portrait:
                    activity.RequestedOrientation = ScreenOrientation.Portrait;
                    break;
                case UIDeviceOrientation.ReversePortrait:
                    activity.RequestedOrientation = ScreenOrientation.ReversePortrait;
                    break;
            }
        }

        public UIDeviceOrientation GetOrientation()
        {
            var activity = (Activity) Xamarin.Forms.Forms.Context;
            if (activity == null) return UIDeviceOrientation.Unknown;

            switch (activity.RequestedOrientation)
            {
                case ScreenOrientation.Landscape:
                    return UIDeviceOrientation.Landscape;
                case ScreenOrientation.Portrait:
                    return UIDeviceOrientation.Portrait;
                case ScreenOrientation.ReverseLandscape:
                    return UIDeviceOrientation.ReverseLandscape;
                case ScreenOrientation.ReversePortrait:
                    return UIDeviceOrientation.ReversePortrait;
            }

            return UIDeviceOrientation.Unknown;
        }
    }
}