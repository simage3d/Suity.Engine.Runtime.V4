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
    public class ServerSocketSession : NetworkSession
    {
        readonly SsAppSession _inner;

        public ServerSocketSession(SsAppSession inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public override NetworkServer Server => _inner._parentServer;

        public override IPEndPoint RemoteEndPoint => _inner.RemoteEndPoint;

        public override IPEndPoint LocalEndPoint => _inner.LocalEndPoint;

        public override bool Connected => _inner.Connected;

        public override string SessionId => _inner.SessionID;

        public override DateTime StartTime => _inner.StartTime;

        public override NetworkUser User
        {
            get => _inner.User;
            set => _inner.User = value;
        }

        public override void Close()
        {
            _inner.Close();
        }

        public override DateTime GetLastIncomingTime()
        {
            return _inner.GetLastIncomingTime();
        }

        public override DateTime GetLastIncomingTime(int channel)
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
