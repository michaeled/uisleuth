using System.IO;
using Android.App;
using Android.Graphics;

namespace UISleuth.Android
{
    internal class ScreenShotAndroid : IScreenShot
    {
        public byte[] Capture()
        {
            byte[] result;

            var activity = (Activity)Xamarin.Forms.Forms.Context;
            var view = activity.Window.DecorView.RootView;

            using (var bitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888))
            {
                var canvas = new Canvas(bitmap);
                view.Draw(canvas);

                using (var stream = new MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, stream);
                    result = stream.ToArray();
                }
            }

            return result;
        }
    }
}