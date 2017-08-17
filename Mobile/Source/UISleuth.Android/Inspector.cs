// ReSharper disable CheckNamespace

using UISleuth.Android;
using UISleuth.Networking;

namespace UISleuth
{
    public static class Inspector
    {
        public static void Init(short mobilePort = ServiceEndpoint.MobilePort, bool loggingEnabled = true)
        {
            InitPlatform();
            InspectorBootstrapper.Init(mobilePort, loggingEnabled);
        }


        public static void ShowAcceptingConnections()
        {
            InspectorBootstrapper.ShowAcceptingConnections();
        }


        private static void InitPlatform()
        {
            InspectorContainer.Init();

            InspectorContainer.Current.Register<IScreenShot, ScreenShotAndroid>();
            InspectorContainer.Current.Register<IDisplayDimensions, DisplayDimensionsAndroid>();
            InspectorContainer.Current.Register<ITouchEvent, TouchEventAndroid>();
            InspectorContainer.Current.Register<IInspectorEnvironment, InspectorEnvironmentAndroid>();
            InspectorContainer.Current.Register<IDeviceOrientation, DeviceOrientationAndroid>();
        }
    }
}