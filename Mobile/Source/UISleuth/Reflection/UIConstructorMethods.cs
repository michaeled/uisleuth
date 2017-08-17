using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UISleuth.Widgets;

namespace UISleuth.Reflection
{
    internal class UIConstructorMethods
    {
        public static object Construct(ITypeFinder finder, UIConstructor ctor)
        {
            var cType = finder.Find(ctor.TypeName);
            if (cType == null) return null;
            object[] ps = null;

            if (ctor.Parameters.Any())
            {
                ps = ctor
                    .Parameters
                    .OrderBy(c => c.Position)
                    .Select(p =>
                    {
                        if (p == null) return null;

                        var pType = finder.Find(p.TypeName);
                        if (pType == null) return null;
                        object val;

                        try
                        {
                            if (pType.GetTypeInfo().IsEnum)
                            {
                                val = ReflectionMethods.CreateEnum(pType, p.Value);
                            }
                            else
                            {
                                val = Convert.ChangeType(p.Value, pType);
                            }
                        }
                        catch (Exception)
                        {
                            val = p.Value;
                        }

                        return val;
                    }).ToArray();
            }

            return Activator.CreateInstance(cType, ps);
        }


        public static UIConstructor[] GetConstructors(Type type)
        {
            if (type == null) return new UIConstructor[] {};

            var xctors = new List<UIConstructor>();
            var tctors = type.GetConstructors();

            foreach (var tctor in tctors)
            {
                if (tctor.IsAbstract) continue;

                var xctor = new UIConstructor();
                var xparams = new List<UIParameter>();

                xctor.TypeName = type.AssemblyQualifiedName;

                foreach (var pi in tctor.GetParameters())
                {
                    var xparam = new UIParameter
                    {
                        ParameterName = pi.Name,
                        Position = pi.Position,
                        TypeName = pi.ParameterType.FullName,
                        IsTypeEnum = pi.ParameterType.GetTypeInfo().IsEnum
                    };

                    xparams.Add(xparam);
                }

                xctor.Parameters = xparams.ToArray();
                xctors.Add(xctor);
            }

            return xctors.ToArray();
        }
    }
}
