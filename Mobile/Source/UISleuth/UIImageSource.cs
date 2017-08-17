using Xamarin.Forms;

namespace UISleuth
{
    internal sealed class UIImageSource : StreamImageSource
    {
        public string FileName { get; set; }


        public UIImageSource(ImageSource source)
        {
            var sis = source as StreamImageSource;

            if (sis != null)
            {
                Stream = sis.Stream;
            }
        }
    }
}