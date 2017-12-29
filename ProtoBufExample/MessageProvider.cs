using System;
using System.Linq;
using System.Reflection;

namespace ProtoBufExample
{
    public static class MessageProvider
    {
        public static Type[] GetKnownEventTypes()
        {
            var types = Assembly
                .GetExecutingAssembly()
                .GetExportedTypes()
                .Where(t => typeof(IEvent).IsAssignableFrom(t) && t.IsAbstract == false)
                .Union(new[] {typeof(MessageContract)})
                .ToArray();

            return types;
        }
    }
}
