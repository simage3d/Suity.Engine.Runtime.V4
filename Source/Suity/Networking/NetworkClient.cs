// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Suity.Networking
{
    [MultiThreadSecurity(MultiThreadSecurityMethods.LockedSecure)]
    public abstract class NetworkClient : NetworkRequest
    {
        protected readonly int _channelSize;
        protected readonly int _resultChannelSize;
        protected readonly BinaryDataWriter _buffer = new BinaryDataWriter();
        protected readonly object _lock = new object();

        readonly Dictionary<string, NetworkUpdater> _updaters = new Dictionary<string, NetworkUpdater>();
        readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        PacketFormatter _formatter;

        ConnectFuture _connectFuture;
        readonly ISendFuture[] _sendFutures;
        readonly DateTime[] _sendTimes;

        string _clientName;
        bool _logEvent = true;
        bool _logErrorEvent = true;
        bool _logUpdaterInit;

        private IPEndPoint _endPoint;
        private IPEndPoint _lastEndPoint;

        //连接断开时是否重新连接
        bool _disconnectTriggerReconnect;
        //连接成功后时候作为一个重连接事件
        bool _reconnecting;
        int _retryTimes;
        bool _isWeak;

        long _numBytesSent;
        long _numBytesReceived;
        long _numBytesSentTotal;
        long _numBytesReceivedTotal;

        double _numBytesSentSec;
        double _numBytesReceiveSec;

        DateTime _lastHealthCheck;


        public NetworkClient()
        {
            _lastHealthCheck = DateTime.UtcNow;
        }

        public event EventHandler Connected;
        public event EventHandler Reconnected;
        public event EventHandler Closed;
        public event EventHandler<PackageEventArgs> NotifyPackageReceived;
        public event EventHandler<ChannelPackageEventArgs> ChannelSending;
        public event EventHandler<ChannelPackageEventArgs> ChannelReceived;

        public TimeSpan ChannelTimeOutSpan { get; set; } = TimeSpan.FromSeconds(7);
        public TimeSpan TimeSpanWeak { get; set; } = TimeSpan.FromSeconds(3);
        public bool DisableUpdater { get; set; }
        public PacketFormatter Formatter
        {
            get { return _formatter; }
            set
            {
                _formatter = value;
                OnPacketFormatterUpdated();
            }
        }
        public bool IsStarted { get; private set; }
        public IPEndPoint EndPoint => _endPoint;
        public IPEndPoint LastEndPoint => _lastEndPoint;

        public bool TryReconnect { get; set; } = true;
        public abstract bool IsConnected { get; }
        public bool IsSignalWeak
        {
            get
            {
                if (TryReconnect && !IsConnected)
                {
                    return true;
                }

                return _isWeak;
            }
        }

        public long BytesSentTotal => _numBytesSentTotal;
        public long BytesReceivedTotal => _numBytesReceivedTotal;
        public double BytesSentPerSecond => _numBytesSentSec;
        public double BytesReceivedPerSecond => _numBytesReceiveSec;


        public bool LogEvent
        {
            get { return _logEvent; }
            set { _logEvent = value; }
        }
        public bool LogErrorEvent
        {
            get { return _logErrorEvent; }
            set { _logErrorEvent = value; }
        }




        public NetworkClient(int channelSize, int resultChannelSize)
        {
            if (channelSize <= 0)
            {
                throw new ArgumentException("channelSize value invalid", nameof(channelSize));
            }
            if (resultChannelSize < 0 || resultChannelSize > channelSize)
            {
                throw new ArgumentException("sendOnlyIndexStart value invalid", nameof(channelSize));
            }

            _channelSize = channelSize;
            _resultChannelSize = resultChannelSize;

            _sendFutures = new ISendFuture[_resultChannelSize];
            _sendTimes = new DateTime[_resultChannelSize];

            _endPoint = new IPEndPoint(IPAddress.Any, 0);
        }
        public void Dispose()
        {
            lock (_lock)
            {
                Close("Client shutdown");

                if (IsStarted)
                {
                    IsStarted = false;
                    ClientShutdown("Client shutdown");
                }
            }
        }
        protected internal override void Destroy()
        {
            Dispose();
        }


        protected override string GetName()
        {
            return _clientName;
        }
        protected override void SetName(string name)
        {
            _clientName = name;
        }


        public void RegisterUpdaterFamily(NetworkUpdaterFamily family)
        {
            if (family == null)
            {
                throw new ArgumentNullException(nameof(family));
            }

            lock (_lock)
            {
                foreach (var updater in family.Updaters)
                {
                    if (updater == null)
                    {
                        continue;
                    }

                    AddUpdater(updater);
                }
            }
        }
        public void AddUpdater(NetworkUpdater updater)
        {
            if (updater == null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            lock (_lock)
            {
                if (!string.IsNullOrEmpty(updater.Name))
                {
                    _updaters[updater.Name] = updater;
                }
                if (updater.ResultType != null)
                {
                    _updaters[updater.ResultType.FullName] = updater;
                }

                //if (_logUpdaterInit)
                //{
                //    AddLog(LogMessageType.Debug, $"[{Name}] add package updater : {updater.Name}");
                //}
            }
        }
        private void Reconnect()
        {
            lock (_lock)
            {
                if (_endPoint == null)
                {
                    return;
                }

                if (!IsStarted)
                {
                    IsStarted = true;
                    ClientStart();
                }

                if (_logEvent)
                {
                    AddLog(LogMessageType.Info, $"[{Name}] reconnecting to {_endPoint}...({_retryTimes})");
                }

                CleanUp();

                _disconnectTriggerReconnect = false;
                ClientDisconnect("Reconnect");

                _disconnectTriggerReconnect = true;
                _reconnecting = true;
                _retryTimes++;
                ClientConnect(_endPoint, true);
            }
        }



        public bool IsChannelBusy(int channel)
        {
            if (channel >= 0 && channel < _sendFutures.Length)
            {
                return _sendFutures[channel] != null;
            }
            else
            {
                return false;
            }
        }
        public void HealthCheck()
        {
            lock (_lock)
            {
                DateTime now = DateTime.UtcNow;

                bool weak = false;

                for (int i = 0; i < _sendFutures.Length; i++)
                {
                    var send = _sendFutures[i];
                    if (send != null)
                    {
                        var span = now - send.SendTime;
                        if (span > TimeSpanWeak)
                        {
                            weak = true;
                        }

                        if (span > ChannelTimeOutSpan)
                        {
                            if (_logEvent)
                            {
                                AddLog(LogMessageType.Warning, $"[{Name}] channel {i} time out, reconnecting...");
                            }

                            Reconnect();
                            break;
                        }
                    }
                }
                _isWeak = weak;

                double secconds = (now - _lastHealthCheck).TotalSeconds;
                _lastHealthCheck = now;

                if (secconds > 0)
                {
                    _numBytesSentSec = _numBytesSent / secconds;
                    _numBytesReceiveSec = _numBytesReceived / secconds;
                }

                _numBytesSent = 0;
                _numBytesReceived = 0;
            }
        }
        public IFuture Connect(string ip, int port)
        {
            IPAddress ipAddr = IPAddress.Parse(ip);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

            return Connect(endPoint);
        }
        public IFuture Connect(IPEndPoint endPoint)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            lock (_lock)
            {
                if (!IsStarted)
                {
                    IsStarted = true;
                    ClientStart();
                }

                _endPoint = _lastEndPoint = endPoint;

                if (IsConnected)
                {
                    if (GetClientRemoteEndPoint() == endPoint)
                    {
                        return new ResultFuture<object>(null);
                    }
                    else
                    {
                        ClientDisconnect("Client connect to the another end point");
                    }
                }

                if (_logEvent)
                {
                    AddLog(LogMessageType.Info, $"[{Name}] connecting to {endPoint}...");
                }

                if (_connectFuture == null)
                {
                    _connectFuture = new ConnectFuture();
                }
                _disconnectTriggerReconnect = true;

                ClientConnect(endPoint, false);
                return _connectFuture;
            }
        }

        public void Close(string reason)
        {
            lock (_lock)
            {
                if (!IsStarted)
                {
                    return;
                }

                CleanUp();

                IsStarted = false;

                _disconnectTriggerReconnect = false;
                _reconnecting = false;
                _retryTimes = 0;

                ClientDisconnect(reason);
            }
        }
        public override bool Push(object obj, NetworkDeliveryMethods method, int channel)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }

            if (method == NetworkDeliveryMethods.Default)
            {
                method = NetworkDeliveryMethods.ReliableOrdered;
            }

            lock (_lock)
            {
                if (!IsConnected)
                {
                    if (_logEvent)
                    {
                        AddLog(LogMessageType.Warning, $"[{Name}] send {obj.GetType().Name} Channel={channel}, but connection is closed.");
                    }
                    return false;
                }

                if (_logEvent)
                {
                    AddNetworkLog(LogMessageType.Info, NetworkDirection.Upload, method, channel, obj);
                }

                _buffer.Reset();
                ClientSendMessage(obj, method, channel);

                return true;
            }
        }
        public override IFuture<TResult> Send<TRequest, TResult>(TRequest obj, NetworkDeliveryMethods method, int channel)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }
            if (channel < 0 || channel >= _channelSize)
            {
                throw new ArgumentException("Channel number out of range : " + channel);
            }

            if (method == NetworkDeliveryMethods.Default)
            {
                method = NetworkDeliveryMethods.ReliableOrdered;
            }

            lock (_lock)
            {
                if (!IsConnected)
                {
                    if (_logEvent)
                    {
                        AddNetworkLog(LogMessageType.Warning, NetworkDirection.None, method, channel, $"[{Name}] send {obj.GetType().Name} Channel={channel}, but connection is closed.");
                    }
                    return new ErrorFuture<TResult>(new ErrorResult
                    {
                        StatusCode = nameof(StatusCodes.ConnectionClosed),
                        Location = Suity.Environment.Location,
                    });
                }
                if (method == NetworkDeliveryMethods.ReliableOrdered && channel < _sendFutures.Length && _sendFutures[channel] != null)
                {
                    if (_logErrorEvent)
                    {
                        AddNetworkLog(LogMessageType.Error, NetworkDirection.None, method, channel, $"[{Name}] send {obj.GetType().Name} Channel={channel}, but channel is not idle.");
                    }
                    TimeSpan sendTimeSpan = DateTime.UtcNow - _sendFutures[channel].SendTime;
                    if (sendTimeSpan > ChannelTimeOutSpan)
                    {
                        if (_logEvent)
                        {
                            AddNetworkLog(LogMessageType.Info, NetworkDirection.None, method, channel, $"[{Name}] channel {channel} time out, reconnecting...");
                        }
                        Close($"Channel {channel} time out");
                        Reconnect();
                    }
                    return new ErrorFuture<TResult>(new ErrorResult
                    {
                        StatusCode = nameof(StatusCodes.ChannelNotIdle),
                        Location = Suity.Environment.Location,
                    });
                }

                if (_logEvent)
                {
                    AddNetworkLog(LogMessageType.Info, NetworkDirection.Upload, method, channel, obj);
                }

                _buffer.Reset();
                ClientSendMessage(obj, method, channel);

                if (method == NetworkDeliveryMethods.ReliableOrdered && channel < _sendFutures.Length)
                {
                    SendFuture<TResult> future = new SendFuture<TResult>(obj, Suity.Environment.Location);

                    _sendFutures[channel] = future;
                    _sendTimes[channel] = DateTime.UtcNow;
                    ChannelSending?.Invoke(this, new ChannelPackageEventArgs(obj, channel));
                    return future;
                }
                else
                {
                    return EmptyFuture<TResult>.Empty;
                }
            }
        }

        protected void ReportDataTraffic(NetworkDirection direction, int numBytes)
        {
            if (numBytes <= 0)
            {
                return;
            }

            switch (direction)
            {
                case NetworkDirection.Upload:
                    _numBytesSent += numBytes;
                    _numBytesSentTotal += numBytes;
                    break;
                case NetworkDirection.Download:
                    _numBytesReceived += numBytes;
                    _numBytesReceivedTotal += numBytes;
                    break;
                default:
                    break;
            }
        }


        #region Services

        public void AddService<T>(T service)
        {
            lock (_lock)
            {
                _services[typeof(T)] = service;
            }
        }
        public virtual T GetService<T>() where T : class
        {
            lock (_lock)
            {
                if (_services.TryGetValue(typeof(T), out object service))
                {
                    return (T)service;
                }

                return default(T);
            }
        }

        #endregion

        #region To override

        protected void AddLog(LogMessageType type, object message)
        {
            Logs.AddLog(type, message);
            Logs.AddNetworkLog(type, NetworkDirection.None, Name, null, message);
        }
        protected void AddNetworkLog(LogMessageType type, NetworkDirection direction, NetworkDeliveryMethods method, int channel, object message)
        {
            Logs.AddNetworkLog(type, direction, Name, $"{method}:{channel}", message);
        }
        protected void AddNetworkLog(LogMessageType type, NetworkDirection direction, string channelId, object message)
        {
            Logs.AddNetworkLog(type, direction, Name, channelId, message);
        }

        protected abstract IPEndPoint GetClientRemoteEndPoint();

        protected abstract void ClientStart();

        protected abstract void ClientConnect(IPEndPoint endPoint, bool reconnecting);

        protected abstract void ClientSendMessage(object obj, NetworkDeliveryMethods method, int channel);

        protected abstract void ClientDisconnect(string reason);

        protected abstract void ClientShutdown(string reason);

        protected virtual void OnPacketFormatterUpdated()
        {
        }

        protected virtual void OnLatencyUpdated(TimeSpan latency)
        {

        }

        #endregion

        #region To notify

        protected void NotifyClientConnected()
        {
            if (_logEvent)
            {
                AddLog(LogMessageType.Info, $"[{Name}] connected.");
            }

            lock (_lock)
            {
                ConnectFuture future = _connectFuture;
                _connectFuture = null;
                _retryTimes = 0;
                future?._onResult?.Invoke(null);
                if (_reconnecting)
                {
                    Reconnected?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    _reconnecting = true;
                    Connected?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        protected void NotifyClientDisconnected()
        {
            if (_logEvent)
            {
                AddLog(LogMessageType.Info, $"[{Name}] connection closed.");
            }

            lock (_lock)
            {
                CleanUp();
                Closed?.Invoke(this, EventArgs.Empty);
            }
            if (_disconnectTriggerReconnect && TryReconnect)
            {
                Reconnect();
            }
        }

        protected void NotifyClientError(Exception exception)
        {
            if (_logErrorEvent)
            {
                AddLog(LogMessageType.Error, $"[{Name}] encounter {exception.GetType().Name}");
                AddLog(LogMessageType.Error, exception);
            }

            lock (_lock)
            {
                if (_disconnectTriggerReconnect && TryReconnect)
                {
                    if (_logErrorEvent)
                    {
                        AddLog(LogMessageType.Error, $"[{Name}] Try reconnect....");
                    }
                    Close("Client error");
                    Reconnect();
                }
                else
                {
                    if (_connectFuture != null)
                    {
                        if (_logErrorEvent)
                        {
                            AddLog(LogMessageType.Error, $"[{Name}] Report to ConnectFuture.");
                            AddLog(LogMessageType.Error, $"[{Name}] Has error callback : {_connectFuture._onError != null}");
                        }

                        ConnectFuture future = _connectFuture;
                        _connectFuture = null;

                        future._onError?.Invoke(new ErrorResult
                        {
                            StatusCode = nameof(StatusCodes.ClientError),
                            Location = Suity.Environment.Location,
                        });
                    }
                    else
                    {
                        if (_logErrorEvent)
                        {
                            AddLog(LogMessageType.Error, $"[{Name}] Can not report to ConnectFuture(= null).");
                        }
                    }
                }
            }
        }

        protected void NotifyPacket(byte[] data, int offset, int length, NetworkDeliveryMethods method, int channel)
        {
            string typeName = null;
            object obj = null;
            if (_formatter.Decode(data, offset, length, ref typeName, ref obj))
            {
                NotifyPacket(new NetworkInfo(typeName, obj, method, channel));
            }
        }

        protected void NotifyPacket(string key, object obj, NetworkDeliveryMethods method, int channel)
        {
            NotifyPacket(new NetworkInfo(key, obj, method, channel));
        }

        protected void NotifyPacket(INetworkInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            var key = info.Key;
            var obj = info.Body;
            var method = info.Method;
            var channel = info.Channel;

            if (method == NetworkDeliveryMethods.Default)
            {
                method = NetworkDeliveryMethods.ReliableOrdered;
            }

            if (obj != null)
            {
                if (_logEvent)
                {
                    AddNetworkLog(LogMessageType.Info, NetworkDirection.Download, method, channel, obj);
                }
            }
            else
            {
                if (_logErrorEvent)
                {
                    AddNetworkLog(LogMessageType.Error, NetworkDirection.Download, method, channel, $"[{Name}] receive empty package.");
                }
            }

            lock (_lock)
            {
                NetworkUpdater updater;

                if (method == NetworkDeliveryMethods.ReliableOrdered && channel >= 0 && channel < _sendFutures.Length)
                {
                    // 带有Result的情况

                    ISendFuture future = _sendFutures[channel];
                    _sendFutures[channel] = null;
                    ChannelReceived?.Invoke(this, new ChannelPackageEventArgs(obj, channel));

                    if (obj is ErrorResult errorResult)
                    {
                        if (future != null)
                        {
                            OnLatencyUpdated(DateTime.UtcNow - _sendTimes[channel]);
                            future.SetSendError(errorResult);
                        }
                    }
                    else
                    {
                        if (!DisableUpdater && _updaters.TryGetValue(key, out updater))
                        {
                            updater.ExecuteUpdater(this, future?.Request, info);
                        }

                        if (future != null)
                        {
                            OnLatencyUpdated(DateTime.UtcNow - _sendTimes[channel]);
                            future.SetSendResult(obj);
                        }

                        OnPackageReceived(obj);
                    }
                }
                else
                {
                    // 只发送，没有Result的情况

                    if (!DisableUpdater && _updaters.TryGetValue(key, out updater))
                    {
                        updater.ExecuteUpdater(this, null, info);
                    }

                    if (!(obj is ErrorResult))
                    {
                        OnPackageReceived(obj);
                        NotifyPackageReceived?.Invoke(this, new PackageEventArgs(obj));
                    }
                }
            }
        }

        #endregion

        private void CleanUp()
        {
            lock (_lock)
            {
                _connectFuture = null;

                ISendFuture[] futures = _sendFutures.ToArray();
                Array.Clear(_sendFutures, 0, _sendFutures.Length);
                Array.Clear(_sendTimes, 0, _sendTimes.Length);

                foreach (var future in futures)
                {
                    future?.SetSendError(new ErrorResult
                        {
                            StatusCode = nameof(StatusCodes.ConnectionClosed),
                            Location = Suity.Environment.Location,
                        });
                }
            }
        }



        #region class ConnectFuture
        class ConnectFuture : IFuture
        {
            internal Action<object> _onResult;
            internal Action<ErrorResult> _onError;

            #region IFuture 成员

            public IFuture OnResult(Action<object> onComplete)
            {
                if (onComplete != null)
                {
                    _onResult += onComplete;
                }
                return this;
            }

            public IFuture OnError(Action<ErrorResult> onError)
            {
                if (onError != null)
                {
                    _onError += onError;
                }
                return this;
            }

            public IFuture OnProgress(Action<object> onProgress)
            {
                return this;
            }

            #endregion
        }
        #endregion

        #region class SendFuture

        interface ISendFuture : IFuture
        {
            object Request { get; }
            DateTime SendTime { get; }
            void SetSendResult(object obj);
            void SetSendError(ErrorResult error);
        }

        class SendFuture<T> : Future<T>, ISendFuture
        {
            readonly string _appLocation;

            internal Action<object> _onResult;
            internal Action<ErrorResult> _onError;

            public object Request { get; }
            public DateTime SendTime { get; private set; }

            public SendFuture(object request, string appLocation)
            {
                Request = request;
                SendTime = DateTime.UtcNow;
                _appLocation = appLocation;
            }

            public void SetSendResult(object obj)
            {
                if (obj is T t)
                {
                    base.SetResult(t);
                }
                else
                {
                    base.SetError(new ErrorResult
                    {
                        StatusCode = nameof(StatusCodes.InvalidCast),
                        //Location = NodeApplication.Current?.ServiceId,
                    });
                }
            }

            public void SetSendError(ErrorResult error)
            {
                base.SetError(error);
            }
        }
        #endregion

        #region class NetworkInfo

        class NetworkInfo : INetworkInfo
        {
            public NetworkDeliveryMethods Method { get; }

            public int Channel { get; }

            public string Key { get; }

            public object Body { get; }

            public object GetArgs(string name)
            {
                return null;
            }

            public NetworkInfo(string key, object body, NetworkDeliveryMethods method, int channel)
            {
                Key = key;
                Body = body;
                Method = method;
                Channel = channel;
            }
        }

        #endregion
    }
}
