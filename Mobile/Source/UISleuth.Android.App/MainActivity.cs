using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace UISleuth.Android.App
{
    [Activity(Name = "com.uisleuth.app.MainActivity", Label = "UI Sleuth", Icon = "@drawable/icon", Theme = "@style/Theme.UISleuth", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.Init(this, bundle);

            LoadApplication(new App());
#if DEBUG
            UISleuth.Inspector.Init();
#endif
        }
    }
}