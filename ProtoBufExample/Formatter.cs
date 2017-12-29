using System;
using System.IO;

namespace ProtoBufExample
{
    public sealed class Formatter
    {
        public Action<object, Stream> SerializeDelegate;
        public Func<Stream, object> DeserializerDelegate;
        public string ContractName;

        public Formatter(string name, Func<Stream, object> deserializerDelegate, Action<object, Stream> serializeDelegate)
        {
            ContractName = name;
            DeserializerDelegate = deserializerDelegate;
            SerializeDelegate = serializeDelegate;
        }
    }
}
