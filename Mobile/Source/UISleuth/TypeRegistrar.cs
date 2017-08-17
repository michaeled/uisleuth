using System;
using System.Collections.Generic;
using System.Linq;
using UISleuth.Reflection;
using UISleuth.Widgets;

namespace UISleuth
{
    internal class TypeRegistrar
    {
        private TypeRegistrar()
        {
            Types = new HashSet<UIType>(new UITypeComparer());
        }


        private static TypeRegistrar _instance;
        public static TypeRegistrar Instance => _instance ?? (_instance = new TypeRegistrar());

        public HashSet<UIType> Types { get; internal set; }


        public bool IsRegistered(UIType type)
        {
            return Types.Contains(type);
        }
        

        public bool IsRegistered(UIReflectionProperty prop, RegistrarMatches matches)
        {
            var nameMatch = Types
                .Any(t => t.FullName != null && t.FullName.Equals(prop.TargetType.FullName, StringComparison.CurrentCultureIgnoreCase));

            if (matches.HasFlag(RegistrarMatches.TypeName | RegistrarMatches.Enum))
            {
                return nameMatch || prop.IsTargetEnum;
            }

            if (matches.HasFlag(RegistrarMatches.TypeName))
            {
                return nameMatch;
            }

            if (matches.HasFlag(RegistrarMatches.Enum))
            {
                return prop.IsTargetEnum;
            }

            return false;
        }


        public bool IsRegistered(Type type)
        {
            return Types.Any(t => t.FullName.Equals(type.FullName, StringComparison.CurrentCultureIgnoreCase));
        }


        public void SetTypes(params Type[] types)
        {
            Types = new HashSet<UIType>(types.Select(type => new UIType {FullName = type.FullName}));
        }


        public bool AddType(UIType type)
        {
            if (string.IsNullOrWhiteSpace(type?.FullName)) return false;

            return _instance.Types.Add(type);
        }
    }
}