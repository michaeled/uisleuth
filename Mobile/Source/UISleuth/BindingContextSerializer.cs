using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UISleuth
{
    internal class CustomJsonTextWriter : JsonTextWriter
    {
        public int CurrentDepth { get; private set; }

        public CustomJsonTextWriter(TextWriter textWriter) : base(textWriter)
        {
            // ignored
        }

        public override void WriteStartObject()
        {
            CurrentDepth++;
            base.WriteStartObject();
        }

        public override void WriteEndObject()
        {
            CurrentDepth--;
            base.WriteEndObject();
        }
    }

    internal class CustomContractResolver : DefaultContractResolver
    {
        private readonly Func<bool> _includeProperty;

        public CustomContractResolver(Func<bool> includeProperty)
        {
            _includeProperty = includeProperty;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var shouldSerialize = property.ShouldSerialize;

            property.ShouldSerialize = obj => _includeProperty() 
            && (shouldSerialize == null || shouldSerialize(obj));

            return property;
        }
    }

    internal class BindingContextSerializer
    {
        public static string SerializeObject(object obj, int maxDepth)
        {
            using (var strWriter = new StringWriter())
            {
                using (var jsonWriter = new CustomJsonTextWriter(strWriter))
                {
                    Func<bool> include = () => jsonWriter.CurrentDepth <= maxDepth;
                    var resolver = new CustomContractResolver(include);

                    var serializer = new JsonSerializer
                    {
                        ContractResolver = resolver,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    };

                    serializer.Serialize(jsonWriter, obj);
                }

                return strWriter.ToString();
            }
        }
    }
}
