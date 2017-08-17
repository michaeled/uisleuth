using Foundation;
using UIKit;

namespace UISleuth.iOS
{
    internal class DeviceOrientationIOS : IDeviceOrientation
    {
        public void SetOrientation(UIDeviceOrientation orientation)
        {
            switch (orientation)
            {
                case UIDeviceOrientation.Landscape:
                    UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.LandscapeLeft), new NSString("orientation"));
                    break;
                case UIDeviceOrientation.ReverseLandscape:
                    UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.LandscapeRight), new NSString("orientation"));
                    break;
                case UIDeviceOrientation.Portrait:
                    UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.Portrait), new NSString("orientation"));
                    break;
                case UIDeviceOrientation.ReversePortrait:
                    UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.PortraitUpsideDown), new NSString("orientation"));
                    break;
            }
        }

        public UIDeviceOrientation GetOrientation()
        {
            switch (UIDevice.CurrentDevice.Orientation)
            {
                case UIKit.UIDeviceOrientation.PortraitUpsideDown:
                    return UIDeviceOrientation.ReversePortrait;
                case UIKit.UIDeviceOrientation.Portrait:
                    return UIDeviceOrientation.Portrait;
                case UIKit.UIDeviceOrientation.LandscapeLeft:
                    return UIDeviceOrientation.Landscape;
                case UIKit.UIDeviceOrientation.LandscapeRight:
                    return UIDeviceOrientation.ReverseLandscape;
                default:
                    return UIDeviceOrientation.Unknown;
            }
        }
    }
}