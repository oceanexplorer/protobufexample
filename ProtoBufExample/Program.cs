using System.Linq;

namespace ProtoBufExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var knownTypes = new[] {typeof(TestEvent), typeof(MessageContract)};
            var eventStreamer = new EventStreamer(new EventSerializer(knownTypes));
            var fileTapeSteam = new FileTapeStream("test");
            var @event = new TestEvent("Geoff", "01524345456");

            var data = eventStreamer.SerializeEvent(@event);
            fileTapeSteam.Append(data);

            fileTapeSteam = new FileTapeStream("test");
            var tapeRecords = fileTapeSteam.ReadRecords();
            var events = eventStreamer.DeserializeEvent(tapeRecords.First().Data);
        }
    }
}
