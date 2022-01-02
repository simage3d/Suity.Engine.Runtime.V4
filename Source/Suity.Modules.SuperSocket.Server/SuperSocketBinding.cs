using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking.Server;
using Suity.Networking;
using Suity.Networking.Server;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using Suity.Engine;
using Suity.Synchonizing;
using Suity.Views;
using Suity.NodeQuery;
using Suity.Crypto;

namespace Suity.Modules
{
    public class SuperSocketBinding : NetworkServer, IModuleBinding, IViewObject, IInfoNode
    {
        const bool Debug = false;

        public const int MaxUser = 4;

        readonly ModuleConfig _config;
        readonly List<NetworkCommandFamily> _commandFamilies = new List<NetworkCommandFamily>();

        readonly SsAppServer _server;

        public SuperSocketBinding(ModuleConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            PacketFormats packetFormat = config.GetItem(NetworkConfigs.PacketFormat, PacketFormats.Binary);
            bool compressed = config.GetItem(NetworkConfigs.Compressed, false);

            AesKey aesKey = config.GetItem(NetworkConfigs.AesKey);

            _server = new SsAppServer(packetFormat, compressed, aesKey);
            _server.ResolveService = type => _config.GetService(type);
            _server.NewSessionConnected += _server_NewSessionConnected;
            _server.SessionClosed += _server_SessionClosed;

            foreach (NetworkCommandFamily family in _config.GetItem(NetworkConfigs.CommandFamilies).OfType<NetworkCommandFamily>())
            {
                _server.RegisterCommandFamily(family);
                _commandFamilies.Add(family);
                //Logs.LogInfo("SuperSocket server added command family : " + family.Name);
            }
        }
        protected override void Destroy()
        {
            base.Destroy();
            Dispose();
        }

        private void _server_NewSessionConnected(SsAppSession session)
        {
            session._parentServer = this;
            OnSessionOpened(session._proxySession, null);
        }
        private void _server_SessionClosed(SsAppSession session, CloseReason value)
        {
            OnSessionClosed(session._proxySession, value.ToString());
        }

        #region IModuleBinding
        T IModuleBinding.GetServiceObject<T>()
        {
            return this as T;
        }

        public void Dispose()
        {
            _server.Dispose();
        }
        #endregion

        #region NetworkServer

        public override bool IsStarted => _server.State == ServerState.Running;
        public override void Start()
        {
            //Logs.LogInfo("Starting SuperSocket ...");

            ServerConfig serverConfig = null;

            string ip = _config.GetItem(NetworkConfigs.IP);
            int port = _config.GetItem(NetworkConfigs.Port);
            if (string.IsNullOrEmpty(ip))
            {
                ip = "127.0.0.1";
            }

            if (serverConfig == null)
            {
                serverConfig = new ServerConfig
                {
                    Ip = ip,
                    Port = port,
                };
                serverConfig.MaxConnectionNumber = _config.GetItem(NetworkConfigs.MaxConnectionNumber, Debug ? 10 : 100);
                serverConfig.MaxRequestLength = _config.GetItem(NetworkConfigs.MaxRequestLength);
                serverConfig.KeepAliveTime = _config.GetItem(NetworkConfigs.KeepAliveTime);
            }

            //Setup the appServer
            if (!_server.Setup(serverConfig))
            {
                Logs.LogError("Failed to setup EditorSocketServer.");
                return;
            }

            if (_commandFamilies.Count == 0)
            {
                Logs.LogWarning("No command installed : " + _config.Name);
            }

            //Try to start the appServer
            if (!_server.Start())
            {
                Logs.LogError("Failed to start EditorSocketServer.");
                return;
            }

            Logs.LogInfo($"SuperSocket is listening on {ip}:{port}");
        }

        public override void Stop()
        {
            //Logs.LogInfo("Stopping SuperSocket server...");
            _server.Stop();
            //Logs.LogInfo("SuperSocket server is stopped.");
        }


        #endregion

        #region IViewObject
        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            setup.AllTreeViewField(this);
        }

        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            sync.Sync("Commands", _commandFamilies, SyncFlag.ReadOnly);
        }
        #endregion

        #region IInfoNode
        void IInfoNode.WriteInfo(INodeWriter writer)
        {
            foreach (var item in _commandFamilies.OfType<IInfoNode>())
            {
                item.WriteInfo(writer);
            }
        }

        #endregion
    }
}
