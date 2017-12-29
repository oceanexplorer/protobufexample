using System.Runtime.Serialization;

namespace ProtoBufExample
{
    [DataContract(Namespace = "Test")]
    public class TestEvent : IEvent
    {
        public TestEvent(string name, string number)
        {
            Name = name;
            Number = number;
        }
        
        [DataMember(Order = 1)] public string Name { get; set; }
        [DataMember(Order = 2)] public string Number { get; set; }
    }
}
