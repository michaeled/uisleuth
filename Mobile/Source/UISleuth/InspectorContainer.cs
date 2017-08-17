using TinyIoC;
using UISleuth.Networking;
using UISleuth.Platform;
using UISleuth.Reactions;
using UISleuth.Reflection;
using UISleuth.Workflow;

namespace UISleuth
{
    internal class InspectorContainer
    {
        private static TinyIoCContainer _container;
        internal static TinyIoCContainer Current
        {
            get
            {
                if (_container == null)
                {
                    Reset();
                }

                return _container;
            }
        }

        internal static void Init()
        {
            Current.Register<ITypeFinder, TypeFinder>();
            Current.Register<InspectorSocket, DefaultInspectorSocket>();
            Current.Register<IUIMessageFinder, UIMessageFinder>();
            Current.Register<InspectorWorkflow, DefaultInspectorWorkflow>();
            Current.Register<INetworkAdapter, NetworkAdapter>();
            Current.Register<IXamlLoader, XamlLoader>();

            Current.Register<ICodeLoader, CodeLoader>().AsMultiInstance();
            Current.Register<SurfaceManager, DefaultSurfaceManager>().AsMultiInstance();
            Current.Register<IInspectorThread, InspectorThread>().AsMultiInstance();
        }

        internal static void Reset()
        {
            _container = new TinyIoCContainer();
        }
    }
}