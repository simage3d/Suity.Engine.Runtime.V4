using DotNetty.Transport.Channels;
using Suity.Helpers;
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
    public class NettySession : NetworkSession
    {
        public const int ChannelCount = 32;


        readonly NettyBinding _server;
        readonly IChannelHandlerContext _inner;

        readonly object _lock = new object();
        bool _commandExecuting;
        readonly List<NettyRequestInfo> _sendings = new List<NettyRequestInfo>();

        DateTime _lastIncomingPackageTime;
        readonly DateTime[] _lastIncomingPackageTimes = new DateTime[ChannelCount];



        public NettySession(NettyBinding server, IChannelHandlerContext inner)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public override NetworkServer Server => _server;

        public override IPEndPoint RemoteEndPoint => _inner.Channel.RemoteAddress as IPEndPoint;

        public override IPEndPoint LocalEndPoint => _inner.Channel.LocalAddress as IPEndPoint;

        public override bool Connected => _inner.Channel.Active;

        public override string SessionId => _inner.Name;

        public override KeepAliveModes KeepAlive => KeepAliveModes.KeepAlive;

        public override void Close()
        {
            _inner.CloseAsync();
        }

        public override T GetService<T>()
        {
            return _server.GetServiceObject<T>();
        }

        public override void Send(object obj, NetworkDeliveryMethods method, int channel)
        {
            NettyRequestInfo info = new NettyRequestInfo(null, obj, method, channel);

            _inner.WriteAndFlushAsync(info);
        }




        internal void EnterExecute()
        {
            lock (_lock)
            {
                if (_commandExecuting)
                {
                    throw new InvalidOperationException("Command is Executing");
                }
                _commandExecuting = true;
            }
        }
        internal void ExitExecute()
        {
            lock (_lock)
            {
                if (!_commandExecuting)
                {
                    throw new InvalidOperationException("Command is NOT Executing");
                }
                _commandExecuting = false;

                if (_sendings.Count == 0)
                {
                    return;
                }

                var ary = _sendings.ToArray();
                _sendings.Clear();

                foreach (var sending in ary)
                {
                    LogOutGoingPackage(sending.Body, sending.Method, sending.Channel);

                    _inner.WriteAsync(sending);
                }

                _inner.Flush();
            }
        }

        internal void LogIncomingPackage(object package, NetworkDeliveryMethods method, int channel)
        {
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Upload, RemoteEndPoint.ToString(), $"{method}:{channel}", package);

            DateTime now = DateTime.UtcNow;

            _lastIncomingPackageTime = now;
            if (channel >= 0 && channel < ChannelCount)
            {
                _lastIncomingPackageTimes[channel] = now;
            }

            if (!package.GetType().HasAttributeCached<NonBehaviorLogAttribute>())
            {
                _server.BehaviorLog.LogIncomingPackage(this, package, channel);
            }
        }
        internal void LogOutGoingPackage(object package, NetworkDeliveryMethods method, int channel)
        {
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Download, RemoteEndPoint.ToString(), $"{method}:{channel}", package);

            if (!package.GetType().HasAttributeCached<NonBehaviorLogAttribute>())
            {
                _server.BehaviorLog.LogOutGoingPackage(this, package, channel);
            }
        }

    }
}
