using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UISleuth.Reflection
{
    internal static class ReflectionExtensions
    {
        public static object GetDefaultValue(this Type t)
        {
            if (t.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(t);
            }

            return null;
        }
        

        public static bool HasFlags(this Enum e)
        {
            if (e.GetType().GetCustomAttributes(typeof(FlagsAttribute), true).Any())
            {
                return true;
            }

            return false;
        }


        public static IEnumerable<Type> GetTypes(this Assembly assembly)
        {
            return assembly.DefinedTypes.Select(t => t.AsType());
        }


        public static EventInfo GetEvent(this Type type, string name)
        {
            return type.GetRuntimeEvent(name);
        }


        public static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces;
        }


        public static bool IsAssignableFrom(this Type type, Type otherType)
        {
            return type.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
        }


        public static Attribute[] GetCustomAttributes(this Type type, Type attributeType, bool inherit)
        {
            return (Attribute[]) type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
        }


        public static IEnumerable<ConstructorInfo> GetConstructors(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors.Where(c => c.IsPublic);
        }


        public static bool IsInstanceOfType(this Type type, object obj)
        {
            return type.IsAssignableFrom(obj.GetType());
        }


        public static IEnumerable<EventInfo> GetPublicEvents(this Type type)
        {
            var events = type.GetRuntimeEvents();
            
            var pub = events
                .Where(e => e.AddMethod.IsPublic || e.AddMethod.IsFamily || e.AddMethod.IsFamilyOrAssembly || e.AddMethod.IsStatic)
                .ToArray();

            return pub;
        }


        public static Delegate[] GetEventSubscribers(this object obj, string eventName)
        {
            var info = obj?.GetType().GetTypeInfo().GetDeclaredField(eventName);
            var field = info?.GetValue(obj);

            var del = (MulticastDelegate) field;
            return del?.GetInvocationList();
        }


        public static MethodInfo GetAddMethod(this EventInfo eventInfo, bool nonPublic = false)
        {
            if (eventInfo.AddMethod == null || (!nonPublic && !eventInfo.AddMethod.IsPublic))
            {
                return null;
            }

            return eventInfo.AddMethod;
        }


        public static MethodInfo GetRemoveMethod(this EventInfo eventInfo, bool nonPublic = false)
        {
            if (eventInfo.RemoveMethod == null || (!nonPublic && !eventInfo.RemoveMethod.IsPublic))
            {
                return null;
            }

            return eventInfo.RemoveMethod;
        }


        public static MethodInfo GetGetMethod(this PropertyInfo property, bool nonPublic = false)
        {
            if (property.GetMethod == null || (!nonPublic && !property.GetMethod.IsPublic))
            {
                return null;
            }

            return property.GetMethod;
        }


        public static MethodInfo GetSetMethod(this PropertyInfo property, bool nonPublic = false)
        {
            if (property.SetMethod == null || (!nonPublic && !property.SetMethod.IsPublic))
            {
                return null;
            }

            return property.SetMethod;
        }



        public static IEnumerable<FieldInfo> GetStaticFields(this Type type)
        {
            return type
                .GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == type)
                .ToArray();
        }


        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type, bool includeStatics = false)
        {
            var flags = BindingFlags.FlattenHierarchy | BindingFlags.Public;

            if (includeStatics)
            {
                flags |= BindingFlags.Static;
            }

            return GetProperties(type, flags);
        }


        public static IEnumerable<PropertyInfo> GetProperties(this Type type, BindingFlags flags)
        {
            var properties = type.GetTypeInfo().DeclaredProperties;

            if ((flags & BindingFlags.FlattenHierarchy) == BindingFlags.FlattenHierarchy)
            {
                properties = type.GetRuntimeProperties();
            }

            var universe = from property in properties
                   let getMethod = property.GetMethod
                   where getMethod != null
                   where (flags & BindingFlags.Public) != BindingFlags.Public || getMethod.IsPublic
                   where (flags & BindingFlags.Instance) != BindingFlags.Instance || !getMethod.IsStatic
                   select property;

            // static filter
            var result = universe.Where(f =>
            {
                if (flags.HasFlag(BindingFlags.Static))
                {
                    return true;
                }

                return !f.GetMethod.IsStatic;
            });

            return result;
        }


        public static PropertyInfo GetProperty(this Type type, string name, BindingFlags flags)
        {
            return GetProperties(type, flags).FirstOrDefault(p => p.Name == name);
        }


        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return GetProperties(type, BindingFlags.Public | BindingFlags.FlattenHierarchy).FirstOrDefault(p => p.Name == name);
        }


        public static IEnumerable<MethodInfo> GetMethods(this Type type)
        {
            return GetMethods(type, BindingFlags.FlattenHierarchy | BindingFlags.Public);
        }


        public static IEnumerable<MethodInfo> GetMethods(this Type type, BindingFlags flags)
        {
            var properties = type.GetTypeInfo().DeclaredMethods;
            if ((flags & BindingFlags.FlattenHierarchy) == BindingFlags.FlattenHierarchy)
            {
                properties = type.GetRuntimeMethods();
            }

            return properties
                .Where(m => (flags & BindingFlags.Public) != BindingFlags.Public || m.IsPublic)
                .Where(m => (flags & BindingFlags.Instance) != BindingFlags.Instance || !m.IsStatic)
                .Where(m => (flags & BindingFlags.Static) != BindingFlags.Static || m.IsStatic);
        }


        public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags)
        {
            return GetMethods(type, flags).FirstOrDefault(m => m.Name == name);
        }


        public static MethodInfo GetMethod(this Type type, string name)
        {
            return GetMethods(type, BindingFlags.Public | BindingFlags.FlattenHierarchy)
                   .FirstOrDefault(m => m.Name == name);
        }


        public static IEnumerable<ConstructorInfo> GetConstructors(this Type type, BindingFlags flags)
        {
            return type.GetConstructors()
                .Where(m => (flags & BindingFlags.Public) != BindingFlags.Public || m.IsPublic)
                .Where(m => (flags & BindingFlags.Instance) != BindingFlags.Instance || !m.IsStatic)
                .Where(m => (flags & BindingFlags.Static) != BindingFlags.Static || m.IsStatic);
        }


        public static IEnumerable<FieldInfo> GetFields(this Type type)
        {
            return GetFields(type, BindingFlags.Public | BindingFlags.FlattenHierarchy);
        }


        public static IEnumerable<FieldInfo> GetFields(this Type type, BindingFlags flags)
        {
            var fields = type.GetTypeInfo().DeclaredFields;
            if ((flags & BindingFlags.FlattenHierarchy) == BindingFlags.FlattenHierarchy)
            {
                fields = type.GetRuntimeFields();
            }

            return fields
                .Where(f => (flags & BindingFlags.Public) != BindingFlags.Public || f.IsPublic)
                .Where(f => (flags & BindingFlags.Instance) != BindingFlags.Instance || !f.IsStatic)
                .Where(f => (flags & BindingFlags.Static) != BindingFlags.Static || f.IsStatic);
        }


        public static FieldInfo GetField(this Type type, string name, BindingFlags flags)
        {
            return GetFields(type, flags).FirstOrDefault(p => p.Name == name);
        }


        public static FieldInfo GetField(this Type type, string name)
        {
            return GetFields(type, BindingFlags.Public | BindingFlags.FlattenHierarchy).FirstOrDefault(p => p.Name == name);
        }

        
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GenericTypeArguments;
        }
    }
}
