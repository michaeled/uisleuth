using System;
using System.Reflection;

namespace UISleuth.Reflection
{
    internal interface ICodeLoader
    {
        object CreateInstance(Type type);
        bool AttachEventHandler(object source, string eventName, object handler, MethodInfo method);
        InspectorAssembly Load(byte[] raw, out Assembly assembly);
        InspectorAssembly GetAssemblyInformation(byte[] raw);
        Assembly GetAssembly(string fullName);
    }
}