using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Helpers;
using SuperSocket.ProtoBase;

namespace Suity.Networking.Client
{
    public class ObjectPackageInfo : IPackageInfo<string>, INetworkInfo
    {
        public string Key { get; private set; }

        public object Body { get; private set; }

        public NetworkDeliveryMethods Method { get; private set; }

        public int Channel { get; private set; }

        Dictionary<string, object> _customArgs;

        protected ObjectPackageInfo()
        {
        }

        public ObjectPackageInfo(string key, object body, NetworkDeliveryMethods method, int channel)
        {
            Key = key;
            Body = body;
            Method = method;
            Channel = channel;
        }

        public object GetArgs(string name)
        {
            return _customArgs?.GetValueOrDefault(name);
        }
        public void SetArgs(string name, object value)
        {
            if (_customArgs == null)
            {
                _customArgs = new Dictionary<string, object>();
            }
            _customArgs[name] = value;
        }
    }
}
