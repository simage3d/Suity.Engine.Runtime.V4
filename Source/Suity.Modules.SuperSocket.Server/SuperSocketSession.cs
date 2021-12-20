using Suity.Networking;
using Suity.Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules
{
    public class SuperSocketSession : NetworkSession
    {
        readonly SsAppSession _inner;

        public SuperSocketSession(SsAppSession inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public override NetworkServer Server => _inner._parentServer;

        public override IPEndPoint RemoteEndPoint => _inner.RemoteEndPoint;

        public override IPEndPoint LocalEndPoint => _inner.LocalEndPoint;

        public override bool Connected => _inner.Connected;

        public override string SessionId => _inner.SessionID;

        public override KeepAliveModes KeepAlive => KeepAliveModes.KeepAlive;

        protected override NetworkUser ResolveNetworkUser()
        {
            return _inner.User;
        }
        protected override void OnSetNetworkUser(NetworkUser user)
        {
            _inner.User = user;
        }


        public override void Close()
        {
            _inner.Close();
        }

        public override DateTime GetLastActiveTime(int channel)
        {
            return _inner.GetLastIncomingTime(channel);
        }

        public override T GetService<T>()
        {
            return _inner.GetService<T>();
        }

        public override void Send(object obj, NetworkDeliveryMethods method, int channel)
        {
            _inner.Send(obj, method, channel);
        }
    }
}
