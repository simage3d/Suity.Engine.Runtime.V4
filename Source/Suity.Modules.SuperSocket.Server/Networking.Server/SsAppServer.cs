using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using Suity.Collections;
using System.Net;
using Suity.Helpers;
using Suity.Engine;
using Suity.Crypto;
using System.Security.Cryptography;

namespace Suity.Networking.Server
{
    public class SuityPackageFilterFactory : IReceiveFilterFactory<SsRequestInfo>
    {
        readonly PacketFormats _format;
        readonly bool _compressed;
        readonly AesKey _aesKey;

        public SuityPackageFilterFactory(PacketFormats format, bool compressed, AesKey aesKey = null)
        {
            _format = format;
            _compressed = compressed;
            _aesKey = aesKey;
        }

        public IReceiveFilter<SsRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new H5PackageFilter(_format, _compressed, _aesKey);
        }
    }


    public class SsAppServer : AppServer<SsAppSession, SsRequestInfo>
    {
        public delegate object ServiceResolve(Type type);


        public ServiceResolve ResolveService;
        public PacketFormats PacketFormat { get; }
        public bool Compressed { get; }
        public AesKey AesKey { get; }

        readonly ConcurrentDictionary<Type, object> _services = new ConcurrentDictionary<Type, object>();
        readonly Dictionary<string, NetworkCommand> _commands = new Dictionary<string, NetworkCommand>();
        IBehaviorLog _behaviorLog;


        public SsAppServer(PacketFormats packetFormat, bool compressed, AesKey aesKey = null)
            : base(new SuityPackageFilterFactory(packetFormat, compressed, aesKey))
        {
            PacketFormat = packetFormat;
            Compressed = compressed;
            AesKey = aesKey;

            SuityFormatter.Initialize();
        }

        public void RegisterCommandFamily(NetworkCommandFamily family)
        {
            if (family == null)
            {
                throw new ArgumentNullException(nameof(family));
            }
            foreach (var command in family.Commands)
            {
                if (command == null || string.IsNullOrEmpty(command.Name))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(command.Method))
                {
                    Logs.LogWarning($"Ingored command {command.Name} which defines a method : {command.Method}.");
                    continue;
                }

                if (_commands.ContainsKey(command.Name))
                {
                    throw new ArgumentException("Command name is already registered : " + command.Name);
                }
                _commands.Add(command.Name, command);
            }
        }
        public void AddService<T>(T service)
        {
            _services[typeof(T)] = service;
        }
        public T GetService<T>()
        {
            object service;
            if (_services.TryGetValue(typeof(T), out service))
            {
                return (T)service;
            }

            if (ResolveService != null)
            {
                service = ResolveService(typeof(T));
                if (service != null && typeof(T).IsAssignableFrom(service.GetType()))
                {
                    _services[typeof(T)] = service;
                    return (T)service;
                }
            }
            return default(T);
        }

        public IBehaviorLog BehaviorLog
        {
            get
            {
                if (_behaviorLog == null)
                {
                    _behaviorLog = GetService<IBehaviorLog>();
                }
                return _behaviorLog ?? EmptyBehaviorLog.Empty;
            }
        }

        protected override void OnNewSessionConnected(SsAppSession session)
        {
            BehaviorLog.LogSessionConnected(session._proxySession);
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.None, session.RemoteEndPoint.ToString(), null, $"New session connected : {session.RemoteEndPoint}");
            base.OnNewSessionConnected(session);
        }
        protected override void OnSessionClosed(SsAppSession session, CloseReason reason)
        {
            //反编译查看过Session的StartTime是DateTime.Now
            session._proxySession?.ReportUserLogout();

            BehaviorLog.LogSessionClosed(session._proxySession, reason.ToString());
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.None, session.RemoteEndPoint.ToString(), null, $"Session closed : {session.RemoteEndPoint} ({reason})");
            base.OnSessionClosed(session, reason);
        }

        protected override void ExecuteCommand(SsAppSession session, SsRequestInfo requestInfo)
        {
            try
            {
                session.LogIncomingPackage(requestInfo.Body, requestInfo.Method, requestInfo.Channel);
                session.EnterExecute();
                var command = _commands.GetValueOrDefault(requestInfo.Key);
                if (command != null)
                {
                    var result = command.ExecuteCommand(session._proxySession, requestInfo);
                    if (result is IFuture future)
                    {
                        future
                            .OnResult(o => session.Send(o, requestInfo.Method, requestInfo.Channel))
                            .OnError(err => session.Send(err, requestInfo.Method, requestInfo.Channel));
                    }
                    else if (result != null)
                    {
                        session.Send(result, requestInfo.Method, requestInfo.Channel);
                    }
                }
                else
                {
                    base.ExecuteCommand(session, requestInfo);
                }
            }
            finally
            {
                session.ExitExecute();
            }
        }
    }
}
