using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suity;
using Suity.Engine;
using Suity.Networking;
using Suity.Networking.Server;
using SuperSocket.SocketBase.Config;

namespace Networking.Server
{
    public class SuitySocketComponent : NodeComponent
    {
        SsAppServer _appServer;
        public SsAppServer Server { get { return _appServer; } }

        public SuitySocketComponent(PacketFormats packetFormat, bool compressed)
        {
            _appServer = new SsAppServer(packetFormat, compressed);
            _appServer.ResolveService = type => GetComponent(type);
            _appServer.NewSessionConnected += _appServer_NewSessionConnected;
            _appServer.SessionClosed += _appServer_SessionClosed;
        }
        protected override void OnStart()
        {
            ISocketConfigProvider provider = GetComponent<ISocketConfigProvider>();
            ServerConfig serverConfig = null;
            if (provider != null)
            {
                serverConfig = provider.GetConfig();
            }
            if (serverConfig == null)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                int port;
                if (!int.TryParse(config.AppSettings.Settings["Socket.Port"].Value, out port))
                {
                    port = 2012;
                }
                serverConfig = new ServerConfig
                {
                    Port = port,
                };
                serverConfig.CommandAssemblies = new ICommandAssemblyConfig[]
                {
                    new CommandAssemblyConfig
                    {
                        Assembly = NodeApplication.Current.GetType().Assembly.FullName
                    }
                };
                serverConfig.MaxRequestLength = 65536;
            }

            //Setup the appServer
            if (!_appServer.Setup(serverConfig))
            {
                Console.WriteLine("Failed to setup!");
                return;
            }

            Console.WriteLine();

            //Try to start the appServer
            if (!_appServer.Start())
            {
                Console.WriteLine("Failed to start!");
                return;
            }
        }
        protected override void OnStop()
        {
            _appServer.Stop();
        }


        private void _appServer_NewSessionConnected(SsAppSession session)
        {
            SendMessage(NetworkConfigs.Event_SessionConnected, session);
        }
        private void _appServer_SessionClosed(SsAppSession session, SuperSocket.SocketBase.CloseReason value)
        {
            SendMessage(NetworkConfigs.Event_SessionClosed, session);
        }
    }
}
