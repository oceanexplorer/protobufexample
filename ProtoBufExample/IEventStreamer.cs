namespace ProtoBufExample
{
    public interface IEventStreamer
    {
        byte[] SerializeEvent(IEvent e);
        IEvent DeserializeEvent(byte[] buffer);
    }
}
