// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Engine;
using Suity.Synchonizing;
using Suity.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Suity.Networking
{
    public class ClientSocketComponent : NodeComponent, IViewObject
    {
        readonly List<NetworkUpdaterFamily> _updaters = new List<NetworkUpdaterFamily>();
        readonly List<AssetRef<NetworkUpdaterFamily>> _updaterRefs = new List<AssetRef<NetworkUpdaterFamily>>();
        readonly List<NetworkSender> _senders = new List<NetworkSender>();

        NetworkClient _client;

        readonly IPConfig _ipConfig = new IPConfig();
        readonly PortConfig _portConfig = new PortConfig();
        readonly StringConfig _domainNameConfig = new StringConfig();

        public override string Icon => "*CoreIcon|Network";

        public static SocketTypes SocketType { get; private set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public PacketFormats PacketFormat { get; private set; } = PacketFormats.Binary;
        public bool Compressed { get; private set; }
        public string DomainName { get; private set; } = string.Empty;

        public IEnumerable<NetworkUpdaterFamily> UpdaterFamilies => _updaters;

        public bool AutoConnnect { get; set; }

        public NetworkClient Client => _client;


        protected override void OnStart()
        {
            foreach (var assetKey in _updaterRefs.Select(o => o.Key).Where(o => !string.IsNullOrEmpty(o)))
            {
                NetworkUpdaterFamily family = ObjectType.GetAssetImplement<NetworkUpdaterFamily>(assetKey);
                if (family == null)
                {
                    throw new ModuleException("Network updater family not found : " + assetKey);
                }
                _updaters.Add(family);
            }

            ModuleConfig config = new ModuleConfig(this);
            config.SetItem(NetworkConfigs.UpdaterFamilies, UpdaterFamilies);

            IP = _ipConfig.GetIPAddress();
            Port = _portConfig.GetTcpPort(NodeApplication.Current.MultipleLaunchIndex);
            if (Port < 0)
            {
                throw new ComponentStartException("Port allocation failed : " + _portConfig.ToString());
            }
            DomainName = _domainNameConfig.GetValue();

            string moduleName = null;
            switch (SocketType)
            {
                case SocketTypes.Udp:
                    moduleName = ModuleBindingNames.UdpClientSocket;
                    break;
                case SocketTypes.Tcp:
                default:
                    moduleName = ModuleBindingNames.TcpClientSocket;
                    break;
            }

            _client = BindModule<NetworkClient>(moduleName, config);
            if (_client == null)
            {
                throw new ModuleBindException($"Bind module failed : {moduleName}");
            }

            _client.Connected += _client_Connected;
            _client.Reconnected += _client_Reconnected;
            _client.Closed += _client_Closed;

            foreach (var assetRef in _updaterRefs)
            {
                var sender = ObjectType.GetAssetImplement<NetworkSender>(assetRef.Key);
                if (sender != null)
                {
                    sender.SetRequester(_client);
                    _senders.Add(sender);
                }
            }

            if (AutoConnnect)
            {
                IPAddress ipAddress = IPAddress.Parse(IP);
                _client.Connect(new IPEndPoint(ipAddress, Port));
            }

            base.OnStart();
        }
        protected override void OnStop()
        {
            base.OnStop();

            foreach (var sender in _senders)
            {
                sender.SetRequester(null);
            }
            _senders.Clear();

            if (_client != null)
            {
                _client.Connected -= _client_Connected;
                _client.Reconnected -= _client_Reconnected;
                _client.Closed -= _client_Closed;
                _client.Close("Component stop");
                _client.Dispose();
                _client = null;
            }
            _client = null;
            _updaters.Clear();
        }
        public override object GetService(Type contentType)
        {
            return _senders.Find(o => o.GetType() == contentType);
        }

        public T GetSender<T>() where T : NetworkSender
        {
            return _senders.OfType<T>().FirstOrDefault();
        }

        #region Client events
        private void _client_Connected(object sender, EventArgs e)
        {
            SendMessage(NetworkConfigs.Event_ClientConnected, this);
        }
        private void _client_Reconnected(object sender, EventArgs e)
        {
            SendMessage(NetworkConfigs.Event_ClientReconnected, this);
        }
        private void _client_Closed(object sender, EventArgs e)
        {
            SendMessage(NetworkConfigs.Event_ClientClosed, this);
        } 
        #endregion

        #region IViewObject
        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            if (ParentObject != null)
            {
                setup.AllInspectorField(this, s => s != "IPConfig" && s != "PortConfig" && s != "DomainNameConfig");
            }
            else
            {
                setup.AllInspectorField(this, s => s != "IP" && s != "Port" && s != "DomainName");
            }
        }
        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            SocketType = sync.Sync("SocketType", SocketType);
            PacketFormat = sync.Sync(NetworkConfigs.PacketFormat.Name, PacketFormat);
            Compressed = sync.Sync(NetworkConfigs.Compressed.Name, Compressed);

            IP = sync.Sync(NetworkConfigs.IP.Name, IP);
            Port = sync.Sync(NetworkConfigs.Port.Name, Port);
            DomainName = sync.Sync(NetworkConfigs.DomainName.Name, DomainName, SyncFlag.NotNull, string.Empty);
            sync.Sync("IPConfig", _ipConfig, SyncFlag.ReadOnly);
            sync.Sync("PortConfig", _portConfig, SyncFlag.ReadOnly);
            sync.Sync("DomainNameConfig", _domainNameConfig, SyncFlag.ReadOnly);

            sync.Sync("Updaters", _updaterRefs, SyncFlag.ReadOnly);
            AutoConnnect = sync.Sync("AutoConnnect", AutoConnnect);
        }
        #endregion
    }
}
