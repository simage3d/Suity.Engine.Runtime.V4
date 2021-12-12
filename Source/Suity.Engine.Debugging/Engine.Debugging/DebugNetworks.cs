// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using Suity.Helpers;
using Suity.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Suity.Engine.Debugging
{
    #region DebugNetworkServer
    class DebugNetworkServer : NetworkServer
    {
        readonly Dictionary<string, DebugNetworkSession> _sessions = new Dictionary<string, DebugNetworkSession>();
        readonly Dictionary<string, NetworkCommand> _commands = new Dictionary<string, NetworkCommand>();
        int _idAlloc;
        bool _isStarted;

        public DebugNetworkServer()
        {
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
                    Logs.LogWarning($"Ingored command {command.Key} which defines a method : {command.Method}.");
                    continue;
                }

                if (_commands.ContainsKey(command.Name))
                {
                    throw new ArgumentException("Command name is already registered : " + command.Name);
                }
                _commands.Add(command.Name, command);
            }
        }

        #region INetworkServer

        public override bool IsStarted => _isStarted;
        public override void Start()
        {
            _isStarted = true;
        }
        public override void Stop()
        {
            _isStarted = false;
        } 

        #endregion


        public DebugNetworkSession CreateSession()
        {
            int id = Interlocked.Increment(ref _idAlloc);
            string sessionId = "DebugSession_" + id.ToString();
            DebugNetworkSession session = new DebugNetworkSession(this, sessionId);

            lock (_sessions)
            {
                _sessions.Add(session.SessionId, session);
            }

            OnSessionOpened(session, null);

            return session;
        }


        internal void _CloseSession(DebugNetworkSession session, string reason)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            if (session.Server != this)
            {
                throw new InvalidOperationException("Session server mismatch");
            }

            bool removed = false;
            lock (_sessions)
            {
                removed = _sessions.Remove(session.SessionId);
            }

            if (removed)
            {
                OnSessionClosed(session, reason);
            }
        }

        public object HandleRequest(DebugNetworkSession session, object request, NetworkDeliveryMethods method, int channel)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (request == null)
            {
                session.LogErrorMessage("Request is null.");
                return null;
            }

            var typeInfo = ObjectType.GetClassTypeInfo(request.GetType());
            if (typeInfo == null)
            {
                session.LogErrorMessage("Type info not found for : " + request.GetType().Name);
                return null;
            }

            NetworkCommand command = _commands.GetValueOrDefault(typeInfo.Key);
            if (command == null)
            {
                session.LogErrorMessage("Command not found for : " + request.GetType().Name);
                return null;
            }

            session.LogIncomingPackage(request, channel);

            DebugNetworkInfo info = new DebugNetworkInfo(typeInfo.Key, request, method, channel);
            var result = command.ExecuteCommand(session, info);

            if (result != null)
            {
                session.LogOutGoingPackage(result, channel);
            }

            return result;
        }

    }
    #endregion

    #region DebugNetworkSession
    class DebugNetworkSession : NetworkSession
    {
        static readonly IPEndPoint _remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1);
        static readonly IPEndPoint _localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2);

        readonly DebugNetworkServer _server;


        public event Action<object, int> DataSent;
        public event Action Disconnected;

        bool _connected;
        string _sessionId;

        public DebugNetworkSession(DebugNetworkServer server, string sessionId)
        {
            if (server == null)
            {
                throw new ArgumentNullException(nameof(server));
            }
            if (sessionId == null)
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            _server = server;
            _sessionId = sessionId;

            _connected = true;

            LogMessage(LogMessageType.Info, "Session connected.");
        }

        public object HandleRequest(object request, NetworkDeliveryMethods method, int channel)
        {
            if (!Connected)
            {
                return new ErrorResult { StatusCode = StatusCodes.ClientError.ToString() };
            }

            return _server.HandleRequest(this, request, method, channel);
        }


        #region NetworkSession
        public override NetworkServer Server => _server;

        public override IPEndPoint RemoteEndPoint => _remoteEndPoint;

        public override IPEndPoint LocalEndPoint => _localEndPoint;

        public override bool Connected => _connected;

        public override string SessionId => _sessionId;

        public override KeepAliveModes KeepAlive => KeepAliveModes.LongTerm;

        public override void Close()
        {
            if (Connected)
            {
                _connected = false;

                _server._CloseSession(this, "Force close");
                Disconnected?.Invoke();

                LogMessage(LogMessageType.Warning, "Session closed.");
            }
        }

        public override T GetService<T>()
        {
            return Suity.Environment.GetService<T>();
        }

        public override void Send(object data, NetworkDeliveryMethods method, int channel)
        {
            LogOutGoingPackage(data, channel);
            DataSent?.Invoke(data, channel);
        }

        #endregion

        internal void LogIncomingPackage(object package, int channel)
        {
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Upload, SessionId.ToString(), channel.ToString(), package);
        }
        internal void LogOutGoingPackage(object package, int channel)
        {
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Download, SessionId.ToString(), channel.ToString(), package);
        }
        internal void LogErrorMessage(string message)
        {
            Logs.AddNetworkLog(LogMessageType.Error, NetworkDirection.None, SessionId.ToString(), string.Empty, message);
        }
        internal void LogMessage(LogMessageType type, string message)
        {
            Logs.AddNetworkLog(type, NetworkDirection.None, SessionId.ToString(), string.Empty, message);
        }
    }
    #endregion

    #region DebugNetworkInfo

    class DebugNetworkInfo : INetworkInfo
    {
        public NetworkDeliveryMethods Method { get; private set; }

        public int Channel { get; private set; }

        public string Key { get; private set; }

        public object Body { get; private set; }

        public object GetArgs(string name)
        {
            return null;
        }

        public DebugNetworkInfo(string key, object body, NetworkDeliveryMethods method, int channel)
        {
            Key = key;
            Body = body;
            Method = method;
            Channel = channel;
        }
    }

    #endregion
}
