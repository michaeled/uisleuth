using System.Runtime.InteropServices;
using UIKit;

namespace UISleuth.iOS
{
    internal class ScreenShotIOS : IScreenShot
    {
        public byte[] Capture()
        {
            var view = UIApplication.SharedApplication.KeyWindow;
            UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, false, 2);
            UIApplication.SharedApplication.KeyWindow.DrawViewHierarchy(view.Bounds, true);
            var img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return ToByteArrayFromJPEG(img);
        }

        private byte[] ToByteArrayFromJPEG(UIImage image)
        {
            if (image == null) return null;

            using (image)
            {
                using (var data = image.AsJPEG())
                {
                    byte[] bytes = new byte[data.Length];
                    Marshal.Copy(data.Bytes, bytes, 0, (int)data.Length);
                    return bytes;
                }
            }
        }
    }
}