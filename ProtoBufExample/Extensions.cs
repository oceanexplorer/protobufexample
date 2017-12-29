using System;
using System.Runtime.Serialization;

namespace ProtoBufExample
{
    public static class Extensions
    {
        public static string GetContractName(this Type self)
        {
            var attr = (DataContractAttribute) self.GetCustomAttributes(typeof(DataContractAttribute), false)[0];
            return $"{attr.Namespace}:{self.Name}";
        }
    }
}
