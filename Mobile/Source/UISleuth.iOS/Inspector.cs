// ReSharper disable CheckNamespace

using UISleuth.iOS;
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

            InspectorContainer.Current.Register<IScreenShot, ScreenShotIOS>();
            InspectorContainer.Current.Register<IDisplayDimensions, DisplayDimensionsIOS>();
            InspectorContainer.Current.Register<ITouchEvent, TouchEventIOS>();
            InspectorContainer.Current.Register<IInspectorEnvironment, InspectorEnvironmentIOS>();
            InspectorContainer.Current.Register<IDeviceOrientation, DeviceOrientationIOS>();
        }
    }
}