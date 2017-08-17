using UISleuth.Android.App.Tests.CanvasSelecting;
using UISleuth.Android.App.Tests.ListView;
using Xamarin.Forms;

namespace UISleuth.Android.App
{
    internal class App : Application
    {
        internal App()
        {
            MainPage = new DataSelectorPage();
            //MainPage = new StackLayoutPage();
        }
    }
}