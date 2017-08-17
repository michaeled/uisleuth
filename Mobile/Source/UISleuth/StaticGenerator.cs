using System;
using System.Linq;
using UISleuth.Reflection;

namespace UISleuth
{
    internal class StaticGenerator : IGenerateValues
    {
        public string[] Get(Type type)
        {
            return type?.GetStaticFields().Select(p => p.Name).ToArray();
        }
    }
}
