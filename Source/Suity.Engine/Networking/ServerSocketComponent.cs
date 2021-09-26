// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Synchonizing;
using Suity.Helpers;
using Suity.Engine;
using Suity.Views;
using Suity.Crypto;

namespace Suity.Networking
{
    /// <summary>
    /// 服务端Socket组件
    /// </summary>
    public class ServerSocketComponent : NodeComponent, IViewObject
    {
        readonly List<NetworkCommandFamily> _commands = new List<NetworkCommandFamily>();
        readonly List<AssetRef<NetworkCommandFamily>> _commandRefs = new List<AssetRef<NetworkCommandFamily>>();

        NetworkServer _server;

        readonly IPConfig _ipConfig = new IPConfig();
        readonly PortConfig _portConfig = new PortConfig();
        readonly StringConfig _domainNameConfig = new StringConfig();
        readonly StringConfig _aesKeyConfig = new StringConfig();
        readonly StringConfig _aesIvConfig = new StringConfig();


        public override string Icon => "*CoreIcon|Network";

        public static SocketTypes SocketType { get; private set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public PacketFormats PacketFormat { get; private set; } = PacketFormats.Binary;
        public bool Compressed { get; private set; }
        public bool AesConfig { get; private set; }
        public string DomainName { get; private set; }
        public int MaxRequestLength { get; private set; } = 1048576;
        public int KeepAliveTime { get; private set; } = 600;
        public int MaxConnectionNumber { get; private set; } = 100;

        public IEnumerable<NetworkCommandFamily> CommandFamilies => _commands;

        protected override void OnStart()
        {
            base.OnStart();

            foreach (var assetKey in _commandRefs.Select(o => o.Key).Where(o => !string.IsNullOrEmpty(o)))
            {
                NetworkCommandFamily family = ObjectType.GetAssetImplement<NetworkCommandFamily>(assetKey);
                if (family == null)
                {
                    throw new ModuleException("Network command family not found : " + assetKey);
                }
                _commands.Add(family);
            }

            ModuleConfig config = new ModuleConfig(this);
            config.SetItem(NetworkConfigs.CommandFamilies, CommandFamilies);
            if (AesConfig)
            {
                config.SetItem(NetworkConfigs.AesKey, new AesKey(_aesKeyConfig.GetValue(), _aesIvConfig.GetValue()));
            }

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
                    moduleName = ModuleBindingNames.UdpServerSocket;
                    break;
                case SocketTypes.Tcp:
                default:
                    moduleName = ModuleBindingNames.TcpServerSocket;
                    break;
            }

            _server = BindModule<NetworkServer>(moduleName, config);
            if (_server == null)
            {
                throw new ModuleBindException($"Bind module failed : {moduleName}");
            }

            _server.SessionOpened += _server_SessionOpened;
            _server.SessionClosed += _server_SessionClosed;
            _server.Start();
        }
        protected override void OnStop()
        {
            base.OnStop();

            if (_server != null)
            {
                _server.Stop();
                _server.SessionOpened -= _server_SessionOpened;
                _server.SessionClosed -= _server_SessionClosed;
                _server = null;
            }
            _commands.Clear();
        }

        private void _server_SessionOpened(object sender, NetworkSessionEventArgs args)
        {
            SendMessage(NetworkConfigs.Event_SessionConnected, args.Session);
        }
        private void _server_SessionClosed(object sender, NetworkSessionEventArgs args)
        {
            FunctionContext context = new FunctionContext(NetworkConfigs.Event_SessionClosed, args.Session);
            context.SetArgument("Reason", args.Reason);
            SendMessage(context);
        }

        #region IViewObject
        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            setup.InspectorField(LabelValue.Empty, new ViewProperty("FormatLabel", "格式"));

            setup.InspectorField(SocketType, new ViewProperty("SocketType", "Socket类型"));
            setup.InspectorField(PacketFormat, new ViewProperty(NetworkConfigs.PacketFormat.Name, "包格式"));
            setup.InspectorField(Compressed, new ViewProperty(NetworkConfigs.Compressed.Name, "压缩"));
            setup.InspectorField(AesConfig, new ViewProperty(nameof(AesConfig), "加密设置"));
            if (AesConfig)
            {
                setup.InspectorField(_aesKeyConfig, new ViewProperty("AesKeyConfig", "Key"));
                setup.InspectorField(_aesIvConfig, new ViewProperty("AesIvConfig", "IV"));
            }

            setup.InspectorField(LabelValue.Empty, new ViewProperty("AddressLabel", "地址"));

            if (ParentObject != null)
            {
                setup.InspectorField(IP, new ViewProperty(NetworkConfigs.IP.Name, "IP"));
                setup.InspectorField(Port, new ViewProperty(NetworkConfigs.Port.Name, "端口"));
                setup.InspectorField(DomainName, new ViewProperty(NetworkConfigs.DomainName.Name, "域名"));
            }
            else
            {
                setup.InspectorField(_ipConfig, new ViewProperty("IPConfig", "IP"));
                setup.InspectorField(_portConfig, new ViewProperty("PortConfig", "端口"));
                setup.InspectorField(_domainNameConfig, new ViewProperty("DomainNameConfig", "域名"));
            }

            setup.InspectorField(LabelValue.Empty, new ViewProperty("CommandLabel", "指令"));

            setup.InspectorField(_commandRefs, new ViewProperty("Commands", "指令"));

            setup.InspectorField(LabelValue.Empty, new ViewProperty("MiscLabel", "杂项"));

            setup.InspectorField(MaxRequestLength, new ViewProperty(NetworkConfigs.MaxRequestLength.Name, "包最大长度"));
            setup.InspectorField(KeepAliveTime, new ViewProperty(NetworkConfigs.KeepAliveTime.Name, "保持连接时长(秒)"));
            setup.InspectorField(MaxConnectionNumber, new ViewProperty(NetworkConfigs.MaxConnectionNumber.Name, "最大连接数量"));
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

            AesConfig = sync.Sync(nameof(AesConfig), AesConfig, SyncFlag.AffectsOthers);
            sync.Sync("AesKeyConfig", _aesKeyConfig, SyncFlag.ReadOnly);
            sync.Sync("AesIvConfig", _aesIvConfig, SyncFlag.ReadOnly);

            sync.Sync("Commands", _commandRefs, SyncFlag.ReadOnly);
            MaxRequestLength = sync.Sync(NetworkConfigs.MaxRequestLength.Name, MaxRequestLength);
            KeepAliveTime = sync.Sync(NetworkConfigs.KeepAliveTime.Name, KeepAliveTime);
            MaxConnectionNumber = sync.Sync(NetworkConfigs.MaxConnectionNumber.Name, MaxConnectionNumber);
        }
        #endregion

    }
}
