using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suity.Helpers;
using SuperSocket.SocketBase.Protocol;

namespace Suity.Networking.Server
{
    public class SsRequestInfo : RequestInfo<object>, INetworkInfo
    {
        public NetworkDeliveryMethods Method { get; private set; }

        public int Channel { get; private set; }

        public new object Body { get { return base.Body; } }



        public SsRequestInfo(string key, object body, NetworkDeliveryMethods method, int channel)
            : base(key, body)
        {
            Method = method;
            Channel = channel;
        }

        public object GetArgs(string name)
        {
            return null;
        }
    }
}
