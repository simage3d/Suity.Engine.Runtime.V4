using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suity.Engine;
using Suity.Helpers;
using Suity.Helpers.Conversion;
using Suity.Networking;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using Suity.Reflecting;
using System.Net;
using Suity.Modules;

namespace Suity.Networking.Server
{
    public class SsAppSession : AppSession<SsAppSession, SsRequestInfo>
    {
        public const int ChannelCount = 32;

        struct SendingPackage
        {
            public object Package;
            public NetworkDeliveryMethods Method;
            public int Channel;

            public override string ToString()
            {
                return string.Format("[{0}] {1}", Channel, Package);
            }
        }

        internal NetworkServer _parentServer;
        readonly internal ServerSocketSession _proxySession;
        readonly object _lock = new object();
        readonly BinaryDataWriter _buffer = new BinaryDataWriter();
        bool _commandExecuting;
        readonly List<SendingPackage> _sendings = new List<SendingPackage>();
        
        readonly HashSet<string> _roles = new HashSet<string>();
        readonly Dictionary<Type, object> _properties = new Dictionary<Type, object>();
        DateTime _lastIncomingPackageTime;
        readonly DateTime[] _lastIncomingPackageTimes = new DateTime[ChannelCount];
        private PackageSender _packageSender;
        private NetworkUser _user;

        public NetworkServer Server => _parentServer;
        public NetworkUser User
        {
            get { return _user; }
            set
            {
                _user = value;
                if (_user != null)
                {
                    _user.Token = SessionID;
                }
            }
        }


        public SsAppSession()
        {
            _proxySession = new ServerSocketSession(this);
        }


        public void AddRole(string role)
        {
            _roles.Add(role);
        }
        public void RemoveRold(string role)
        {
            _roles.Remove(role);
        }
        public bool GetRole(string role)
        {
            return _roles.Contains(role);
        }

        public T GetService<T>() where T : class
        {
            return ((SsAppServer)AppServer).GetService<T>();
        }
        public void SetProperty<T>(T property) where T : class
        {
            lock (_properties)
            {
                if (property != null)
                {
                    _properties[typeof(T)] = property;
                }
                else
                {
                    _properties.Remove(typeof(T));
                }
            }            
        }
        public T GetProperty<T>() where T : class
        {
            lock (_properties)
            {
                object property;
                if (_properties.TryGetValue(typeof(T), out property))
                {
                    return (T)property;
                }
                else
                {
                    return default(T);
                }
            }
        }

        protected override void OnSessionStarted()
        {
            DateTime now = DateTime.UtcNow;

            _lastIncomingPackageTime = now;
            for (int i = 0; i < _lastIncomingPackageTimes.Length; i++)
            {
                _lastIncomingPackageTimes[i] = now;
            }

            base.OnSessionStarted();
        }
        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
        }
        protected override void HandleException(Exception e)
        {
            base.HandleException(e);
        }
        protected override void HandleUnknownRequest(SsRequestInfo requestInfo)
        {
            //base.HandleUnknownRequest(requestInfo);
            Send(new ErrorResult { StatusCode = StatusCodes.UnknownRequest.ToString() }, requestInfo.Method, requestInfo.Channel);
        }
        
        public void Send(object obj, NetworkDeliveryMethods method, int channel)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (channel < 0 || channel >= ChannelCount)
            {
                throw new ArgumentException("Channel value invalid.", nameof(channel));
            }

            if (method == NetworkDeliveryMethods.Default)
            {
                method = NetworkDeliveryMethods.ReliableOrdered;
            }

            //Debug.WriteLine("Send : " + obj.GetType().Name);

            lock (_lock)
            {
                if (_commandExecuting)
                {
                    _sendings.Add(new SendingPackage { Package = obj, Method = method, Channel = channel });
                }
                else
                {
                    LogOutGoingPackage(obj, method, channel);

                    _buffer.Offset = 0;
                    WritePackage(_buffer, obj, method, channel);
                    var b = _buffer.ToBytes();
                    base.Send(b, 0, b.Length);
                }
            }
        }
        public bool TrySend(object obj, NetworkDeliveryMethods method, int channel)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }
            if (channel < 0 || channel >= ChannelCount)
            {
                throw new ArgumentException();
            }

            if (method == NetworkDeliveryMethods.Default)
            {
                method = NetworkDeliveryMethods.ReliableOrdered;
            }

            lock (_lock)
            {
                if (_commandExecuting)
                {
                    _sendings.Add(new SendingPackage { Package = obj, Method = method, Channel = channel });
                    return true;
                }
                else
                {
                    LogOutGoingPackage(obj, method, channel);

                    _buffer.Offset = 0;
                    WritePackage(_buffer, obj, method, channel);
                    var b = _buffer.ToBytes();
                    return base.TrySend(b, 0, b.Length);
                }
            }
        }

        public void Send(IEnumerable<object> objs, NetworkDeliveryMethods method, int channel)
        {
            if (objs == null)
            {
                throw new ArgumentNullException();
            }
            if (channel < 0 || channel >= ChannelCount)
            {
                throw new ArgumentException();
            }

            if (method == NetworkDeliveryMethods.Default)
            {
                method = NetworkDeliveryMethods.ReliableOrdered;
            }

            lock (_lock)
            {
                if (_commandExecuting)
                {
                    _sendings.AddRange(objs.Select(o => new SendingPackage { Package = o, Method = method, Channel = channel }));
                }
                else
                {
                    _buffer.Offset = 0;
                    foreach (var obj in objs)
                    {
                        LogOutGoingPackage(obj, method, channel);

                        //Debug.WriteLine("Send : " + obj.GetType().Name);
                        WritePackage(_buffer, obj, method, channel);
                    }
                    var b = _buffer.ToBytes();
                    base.Send(b, 0, b.Length);
                }
            }
        }
        public void TrySend(IEnumerable<object> objs, NetworkDeliveryMethods method, int channel)
        {
            if (objs == null)
            {
                throw new ArgumentNullException();
            }
            if (channel < 0 || channel >= ChannelCount)
            {
                throw new ArgumentException();
            }

            if (method == NetworkDeliveryMethods.Default)
            {
                method = NetworkDeliveryMethods.ReliableOrdered;
            }

            lock (_lock)
            {
                if (_commandExecuting)
                {
                    _sendings.AddRange(objs.Select(o => new SendingPackage { Package = o, Method = method, Channel = channel }));
                }
                else
                {
                    _buffer.Offset = 0;
                    foreach (var obj in objs)
                    {
                        LogOutGoingPackage(obj, method, channel);

                        //Debug.WriteLine("Send : " + obj.GetType().Name);
                        WritePackage(_buffer, obj, method, channel);
                    }
                    var b = _buffer.ToBytes();
                    base.TrySend(b, 0, b.Length);
                }
            }
        }

        public DateTime GetLastIncomingTime()
        {
            return _lastIncomingPackageTime;
        }
        public DateTime GetLastIncomingTime(int channel)
        {
            return _lastIncomingPackageTimes.GetArrayItemSafe(channel);
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

                _buffer.Offset = 0;
                var ary = _sendings.ToArray();
                _sendings.Clear();

                foreach (var sending in ary)
                {
                    LogOutGoingPackage(sending.Package, sending.Method, sending.Channel);

                    WritePackage(_buffer, sending.Package, sending.Method, sending.Channel);
                }

                var b = _buffer.ToBytes();
                base.Send(b, 0, b.Length);
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
                ((SsAppServer)AppServer).BehaviorLog.LogIncomingPackage(_proxySession, package, channel);
            }
        }
        internal void LogOutGoingPackage(object package, NetworkDeliveryMethods method, int channel)
        {
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Download, RemoteEndPoint.ToString(), $"{method}:{channel}", package);

            if (!package.GetType().HasAttributeCached<NonBehaviorLogAttribute>())
            {
                ((SsAppServer)AppServer).BehaviorLog.LogOutGoingPackage(_proxySession, package, channel);
            }
        }


        private void WritePackage(BinaryDataWriter writer, object obj, NetworkDeliveryMethods method, int channel)
        {
            PackageSender packageSender = _packageSender ?? SetupPackageSender();
            packageSender.WritePackage(writer, obj, (int)method * ChannelCount + channel);
        }
        private PackageSender SetupPackageSender()
        {
            SsAppServer server = AppServer as SsAppServer;
            if (server != null)
            {
                _packageSender = new H5PackageSender(server.PacketFormat, server.Compressed);
            }
            else
            {
                _packageSender = new H5PackageSender(PacketFormats.Binary, false);
            }
            
            return _packageSender;
        }

    }
}
