using System.IO;

namespace ProtoBufExample
{
    public class EventStreamer : IEventStreamer
    {
        private readonly EventSerializer _serializer;

        public EventStreamer(EventSerializer serializer)
        {
            _serializer = serializer;
        }

        public byte[] SerializeEvent(IEvent e)
        {
            byte[] content;

            using (var ms = new MemoryStream())
            {
                _serializer.Serialize(e, e.GetType(), ms);
                content = ms.ToArray();
            }

            byte[] messageContractBuffer;

            using (var ms = new MemoryStream())
            {
                var name = e.GetType().Name;
                var messageContract = new MessageContract(name, content.Length, 0);

                _serializer.Serialize(messageContract, typeof(MessageContract), ms);
                messageContractBuffer = ms.ToArray();
            }

            using (var ms = new MemoryStream())
            {
                var headerContract = new MessageHeaderContract(messageContractBuffer.Length);
                headerContract.WriteHeader(ms);
                ms.Write(messageContractBuffer, 0, messageContractBuffer.Length);
                ms.Write(content, 0, content.Length);

                return ms.ToArray();
            }
        }

        public IEvent DeserializeEvent(byte[] buffer)
        {
            using (var ms = new MemoryStream())
            {
                var header = MessageHeaderContract.ReadHeader(buffer);
                ms.Seek(MessageHeaderContract.FixedSize, SeekOrigin.Begin);

                var headerBuffer = new byte[header.HeaderBytes];
                ms.Read(headerBuffer, 0, (int) header.HeaderBytes);

                var contract = (MessageContract)_serializer.Deserialize(new MemoryStream(headerBuffer), typeof(MessageContract));

                var contentBuffer = new byte[contract.ContentSize];
                ms.Read(contentBuffer, 0, (int) contract.ContentSize);

                var contentType = _serializer.GetContentType(contract.ContractName);
                var @event = (IEvent) _serializer.Deserialize(new MemoryStream(contentBuffer), contentType);

                return @event;
            }
        }
    }
}
