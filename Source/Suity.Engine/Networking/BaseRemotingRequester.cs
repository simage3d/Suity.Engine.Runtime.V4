// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using Suity.AutoDiscovery;
using Suity.Collections;
using Suity.Engine;
using Suity.Helpers;

namespace Suity.Networking
{
    public abstract class BaseRemotingRequester<T> : NodeComponent where T : class
    {
        protected T Proxy { get; private set; }

        protected override void OnStart()
        {
            base.OnStart();

            ModuleConfig config = new ModuleConfig(this);

            IAutoDiscovery serviceDiscovery = BindModule<IAutoDiscovery>(ModuleBindingNames.AutoDiscovery, config);
            if (serviceDiscovery == null)
            {
                throw new ModuleBindException("Bind module failed : ServiceDiscovery");
            }

            var serviceCode = new ServiceCode("Remoting", typeof(T).FullName);
            serviceDiscovery.RequestAutoDiscovery(serviceCode).OnUpdate(o =>
            {
                if (o != null)
                {
                    CreateRemotingProxy(o.Url);
                    if (Proxy != null)
                    {
                        Logs.LogInfo($"Remoting {typeof(T).FullName} proxy updated.");
                    }
                    else
                    {
                        Logs.LogError($"Remoting {typeof(T).FullName} proxy failed.");
                    }
                }
                else
                {
                    DeleteRemotingProxy();
                    Logs.LogWarning($"Remoting {typeof(T).FullName} proxy lost.");
                }
            });
        }


        private void CreateRemotingProxy(string url)
        {
            lock (this)
            {
                DeleteRemotingProxy();

                try
                {
                    ClientTcpChannelStore.GetOrCreateChannel(typeof(T).Name);

                    Proxy = (T)Activator.GetObject(typeof(T), url);
                }
                catch (Exception err)
                {
                    Logs.LogError(err);
                    DeleteRemotingProxy();
                }
            }
        }
        private void DeleteRemotingProxy()
        {
            lock (this)
            {
                Proxy = null;

                try
                {
                    ClientTcpChannelStore.DeleteChannel(typeof(T).Name);
                }
                catch (Exception err)
                {
                    Logs.LogError(err);
                }
            }
        }
    }

    static class ClientTcpChannelStore
    {
        static readonly Dictionary<string, TcpChannel> _tcpChannels = new Dictionary<string, TcpChannel>();
        public static TcpChannel GetOrCreateChannel(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

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
                        ["secure"] = true
                    };

                    channel = new TcpChannel(props, clientProv, serverProv);
                    ChannelServices.RegisterChannel(channel, true);

                    _tcpChannels[name] = channel;
                }

                return channel;
            }
        }
        public static bool DeleteChannel(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            lock (_tcpChannels)
            {
                TcpChannel channel = _tcpChannels.RemoveAndGet(name);

                if (channel != null)
                {
                    ChannelServices.UnregisterChannel(channel);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
