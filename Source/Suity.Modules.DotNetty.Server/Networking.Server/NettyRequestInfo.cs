using Suity.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Networking.Server
{
    public class NettyRequestInfo : INetworkInfo
    {
        public NetworkDeliveryMethods Method { get; private set; }

        public string Key { get; }

        public int Channel { get; }

        public object Body { get; }

        public NettyRequestInfo(string key, object body, NetworkDeliveryMethods method, int channel)
        {
            Key = key;
            Body = body;
            Method = method;
            Channel = channel;
        }

        public object GetArgs(string name)
        {
            return null;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", Channel, Body);
        }
    }
}
