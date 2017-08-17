using System;
using System.Reflection;
using UISleuth.Reflection;

namespace UISleuth.Tests.Reactions.EmptyFakes
{
    internal class FakeCodeLoader : ICodeLoader
    {
        public object CreateInstance(Type type)
        {
            throw new NotImplementedException();
        }

        public InspectorAssembly GetAssemblyInformation(byte[] raw)
        {
            throw new NotImplementedException();
        }

        public Assembly GetAssembly(string fullName)
        {
            throw new NotImplementedException();
        }

        public InspectorAssembly Load(byte[] raw, out Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public Type[] GetExportedTypes(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public MethodInfo[] GetExportedMethods(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public bool AttachEventHandler(object source, string eventName, object handler, MethodInfo method)
        {
            throw new NotImplementedException();
        }
    }
}
