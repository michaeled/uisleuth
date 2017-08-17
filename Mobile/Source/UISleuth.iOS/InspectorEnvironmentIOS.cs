using Foundation;

namespace UISleuth.iOS
{
    internal class InspectorEnvironmentIOS : IInspectorEnvironment
    {
        public string GetAppName()
        {
            var bundleName = (NSString) NSBundle.MainBundle.InfoDictionary["CFBundleName"];
            return bundleName;
        }
    }
}
