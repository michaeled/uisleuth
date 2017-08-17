using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UISleuth.Reflection;

namespace UISleuth.Widgets
{
    [DebuggerDisplay("{FullName}")]
    public class UIType
    {
        public string[] PossibleValues { get; set; }
        public string FullName { get; set; }

        [JsonIgnore]
        public string ShortName => ReflectionMethods.GetShortTypeName(FullName);

        public UIPropertyDescriptors Descriptor { get; set; }
        public UIConstructor[] Constructors { get; set; }

        [JsonIgnore]
        public bool IsNullable
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName))
                {
                    return false;
                }

                var name = ReflectionMethods.GetShortTypeName(FullName);

                return name.EndsWith("?");
            }
        }
    }


    [DebuggerDisplay(".ctor with {Parameters.Length} parameter(s)")]
    public class UIConstructor
    {
        public string TypeName { get; set; }
        public string ShortTypeName => ReflectionMethods.GetShortTypeName(TypeName);
        public UIParameter[] Parameters { get; set; }


        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TypeName))
                {
                    return null;
                }

                if (Parameters == null || !Parameters.Any())
                {
                    return $"{ShortTypeName}()";
                }

                var signature = new StringBuilder();
                signature.Append($"{ShortTypeName}(");

                var ps = Parameters.OrderBy(p => p.Position);

                foreach (var p in ps)
                {
                    signature.Append($"{p.ShortTypeName} {p.ParameterName}");

                    if (ps.Last() == p)
                    {
                        signature.Append(")");
                    }
                    else
                    {
                        signature.Append(", ");
                    }
                }

                return signature.ToString();
            }
        }
    }

    public class UITypeComparer : IEqualityComparer<UIType>
    {
        public bool Equals(UIType x, UIType y)
        {
            return ReferenceEquals(x, y) || x.FullName.Equals(y.FullName);
        }

        public int GetHashCode(UIType obj)
        {
            return obj.FullName.GetHashCode();
        }
    }


    [DebuggerDisplay("param at pos: {Position}; name: {ParameterName}")]
    public class UIParameter
    {
        public int Position { get; set; }
        public string ParameterName { get; set; }
        public string TypeName { get; set; }
        public string ShortTypeName => ReflectionMethods.GetShortTypeName(TypeName);
        public bool IsTypeEnum { get; set; }
        public object Value { get; set; }
        public UIType UIType { get; set; }
    }
}