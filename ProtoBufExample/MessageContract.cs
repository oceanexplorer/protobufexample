using System.Runtime.Serialization;

namespace ProtoBufExample
{
    [DataContract(Namespace = "Test")]
    public sealed class MessageContract
    {
        [DataMember(Order = 1)] public readonly string ContractName;
        [DataMember(Order = 2)] public readonly long ContentSize;
        [DataMember(Order = 3)] public readonly long ContentPosition;

        MessageContract()
        {
        }

        public MessageContract(string contractName, long contentSize, long contentPosition)
        {
            ContractName = contractName;
            ContentSize = contentSize;
            ContentPosition = contentPosition;
        }
    }
}