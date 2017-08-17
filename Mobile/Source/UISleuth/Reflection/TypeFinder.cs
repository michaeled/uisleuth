using System;
using System.Collections.Generic;

namespace UISleuth.Reflection
{
    internal class TypeFinder : ITypeFinder
    {
        private readonly Dictionary<string, Type> _lookup;


        public TypeFinder()
        {
            _lookup = new Dictionary<string, Type>();
        }


        public Type Find(string typeName)
        {
            if (_lookup.ContainsKey(typeName))
            {
                return _lookup[typeName];
            }

            var type = Type.GetType(typeName);
            if (type != null)
            {
                _lookup[typeName] = type;
                return type;
            }

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                {
                    _lookup[typeName] = type;
                    return type;
                }
            }

            return null;
        }
    }
}