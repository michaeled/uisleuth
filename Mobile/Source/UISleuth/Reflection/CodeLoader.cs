using System;
using System.Linq;
using System.Reflection;

namespace UISleuth.Reflection
{
    internal class CodeLoader : ICodeLoader
    {
        public object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }


        public Assembly GetAssembly(string fullName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == fullName);
            return assembly;
        }


        public bool AttachEventHandler(object source, string eventName, object handler, MethodInfo method)
        {
            var eventSource = source.GetType().GetEvent(eventName);
            if (eventSource == null) return false;

            var methodDelegate = Delegate.CreateDelegate(eventSource.EventHandlerType, handler, method);
            eventSource.AddEventHandler(source, methodDelegate);

            return true;
        }

        public InspectorAssembly Load(byte[] raw, out Assembly assembly)
        {
            var loaded = Assembly.Load(raw);
            assembly = loaded;

            var name = loaded.GetName();

            var result = new InspectorAssembly
            {
                FullName = name.FullName,
                ShortName = name.Name
            };

            return result;
        }


        public InspectorAssembly GetAssemblyInformation(byte[] raw)
        {
            var assembly = Assembly.ReflectionOnlyLoad(raw);
            var name = assembly.GetName();

            var result = new InspectorAssembly
            {
                FullName = name.FullName,
                ShortName = name.Name
            };

            return result;
        }
    }
}
