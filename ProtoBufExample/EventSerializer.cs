using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProtoBuf.Meta;

namespace ProtoBufExample
{
    public class EventSerializer
    {
        private readonly IDictionary<Type, Formatter> _type2Contract = new Dictionary<Type, Formatter>();
        private readonly IDictionary<string, Type> _contract2Type = new Dictionary<string, Type>();

        public EventSerializer(IEnumerable<Type> knownEventTypes)
        {
            _type2Contract = knownEventTypes.ToDictionary(t => t, t =>
            {
                var formatter = RuntimeTypeModel.Default.CreateFormatter(t);

                return new Formatter(t.GetContractName(), formatter.Deserialize, (o, stream) => formatter.Serialize(stream, o));
            });

            _contract2Type = knownEventTypes
                .ToDictionary(
                    t => t.GetContractName(),
                    t => t
                );
        }

        public void Serialize(object instance, Type type, Stream destinationStream)
        {
            Formatter formatter;
            if (!_type2Contract.TryGetValue(type, out formatter))
            {
                var s = $"Cannot find serializer for unknown object type '{instance.GetType()}'. " +
                        "Have you passes all known types to the constructor?";

                throw new InvalidOperationException(s);
            }

            formatter.SerializeDelegate(instance, destinationStream);
        }

        public Type GetContentType(string contractName)
        {
            return _contract2Type[contractName];
        }

        public object Deserialize(Stream sourceStream, Type type)
        {
            Formatter value;
            if (!_type2Contract.TryGetValue(type, out value))
            {
                var s = $"Cannot find formatter for unknown object type '{type}'. " +
                        "Have you passes all known types to the constructor?";

                throw new InvalidOperationException(s);
            }

            return value.DeserializerDelegate(sourceStream);
        }
    }
}
