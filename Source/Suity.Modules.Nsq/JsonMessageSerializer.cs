using Newtonsoft.Json;
using NsqSharp.Bus.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules.Nsq
{
    public class JsonMessageSerializer : IMessageSerializer
    {
        // Can also use NsqSharp.Bus.Configuration.BuiltIn.NewtonsoftJsonSerializer instead of writing your own
        // if you use Newtonsoft.Json. IMessageSerializer implementation shown to demonstrate how any serializer
        // could be used.

        public object Deserialize(Type type, byte[] value)
        {
            string json = Encoding.UTF8.GetString(value);
            return JsonConvert.DeserializeObject(json, type);
        }

        public byte[] Serialize(object value)
        {
            string json = JsonConvert.SerializeObject(value);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
