// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using Suity.AutoDiscovery;
using Suity.Collections;
using Suity.Engine;
using Suity.Helpers;
using Suity.Synchonizing;
using Suity.Views;

namespace Suity.Networking
{
    public abstract class BaseRemotingProvider<T, TImplement> : NodeComponent , IViewObject
        where T : class
        where TImplement : MarshalByRefObject, T
    {
        readonly IPConfig _ipConfig = new IPConfig();
        readonly PortConfig _portConfig = new PortConfig();

        public string IPAddress { get; private set; }
        public int Port { get; private set; }

        protected override void OnStart()
        {
            base.OnStart();

            if (NodeApplication.Current == null)
            {
                throw new InvalidOperationException("NodeApplication not started.");
            }

            ModuleConfig config = new ModuleConfig(this);

            IPAddress = _ipConfig.GetIPAddress();
            Port = _portConfig.GetTcpPort(NodeApplication.Current.MultipleLaunchIndex);
            if (Port < 0)
            {
                throw new ComponentStartException("Port allocation failed : " + _portConfig.ToString());
            }

            IAutoDiscovery serviceDiscovery = BindModule<IAutoDiscovery>(ModuleBindingNames.AutoDiscovery, config);
            if (serviceDiscovery == null)
            {
                throw new ModuleBindException("Bind module failed : ServiceDiscovery");
            }

            ServerTcpChannelStore.GetOrCreateChannel(IPAddress, Port);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(TImplement), typeof(T).FullName, WellKnownObjectMode.Singleton);

            string url = $"tcp://{IPAddress}:{Port}/{typeof(T).FullName}";
            var serviceCode = new ServiceCode("Remoting", typeof(T).FullName);
            serviceDiscovery.AddServiceLocation(new ServiceLocation(serviceCode, url));
        }


        #region IViewObject
        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            if (ParentObject == null)
            {
                setup.AllInspectorField(this, s => s != "IP" && s != "Port");
            }
            else
            {
                setup.AllInspectorField(this, s => s != "IPConfig" && s != "PortConfig");
            }
        }
        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            IPAddress = sync.Sync(NetworkConfigs.IP.Name, IPAddress);
            Port = sync.Sync(NetworkConfigs.Port.Name, Port);
            sync.Sync("IPConfig", _ipConfig, SyncFlag.ReadOnly);
            sync.Sync("PortConfig", _portConfig, SyncFlag.ReadOnly);
        }
        #endregion
    }

    static class ServerTcpChannelStore
    {
        static readonly Dictionary<string, TcpChannel> _tcpChannels = new Dictionary<string, TcpChannel>();
        public static TcpChannel GetOrCreateChannel(string ip, int port)
        {
            string name = $"TcpChannel#{port}";
            lock (_tcpChannels)
            {
                TcpChannel channel = _tcpChannels.GetValueOrDefault(name);

                if (channel == null)
                {
                    BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider
                    {
                        TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full
                    };
                    BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
                    System.Collections.IDictionary props = new System.Collections.Hashtable
                    {
                        ["name"] = name,
                        ["secure"] = true,
                        ["bindTo"] = ip,
                        ["port"] = port
                    };

                    channel = new TcpChannel(props, clientProv, serverProv);
                    ChannelServices.RegisterChannel(channel, true);

                    _tcpChannels[name] = channel;
                }

                return channel;
            }
        }
    }
}
