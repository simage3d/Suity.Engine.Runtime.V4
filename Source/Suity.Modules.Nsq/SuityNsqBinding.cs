using System;
using System.Collections.Generic;
using System.Linq;
using NsqSharp;
using NsqSharp.Bus;
using NsqSharp.Bus.Configuration;
using StructureMap;
using StructureMap.Pipeline;
using Suity.Engine;
using Suity.Networking;
using Suity.Helpers;
using Suity.NodeQuery;
using Suity.Views;
using Suity.Collections;
using NsqSharp.Bus.Configuration.BuiltIn;

namespace Suity.Modules.Nsq
{
    class SuityNsqBinding : MessageQueue, IModuleBinding, IInfoNode
    {
        readonly ModuleConfig _config;
        readonly Dictionary<Type, NetworkCommand> _commands = new Dictionary<Type, NetworkCommand>();
        IBus _bus;

        public string Domain { get; private set; }

        public SuityNsqBinding(ModuleConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));


            Domain = config.GetItem(NetworkConfigs.DomainName);
            if (string.IsNullOrEmpty(Domain))
            {
                Logs.LogWarning("Message domain name is not set, using 'default'.");
                Domain = "default";
            }


            var container = SetupDependencyInjectionContainer();

            IEnumerable<NetworkCommandFamily> families = config.GetItem(NetworkConfigs.CommandFamilies);

            IEnumerable<NetworkCommand> handlers = config.GetItem(NetworkConfigs.CommandHandlers);
            if (handlers != null)
            {
                foreach (NetworkCommand handler in handlers.OfType<NetworkCommand>())
                {
                    Logs.LogInfo($"Add nsq command handler : {handler.GetType().FullName} ({handler.GetType().Assembly.FullName})");
                    _commands[handler.RequestType] = handler;
                }
            }

            try
            {
                string[] nsqLookupdAddrs;
                string configNsqLookupd = Environment.GetConfig("NsqLookupd");
                if (!string.IsNullOrEmpty(configNsqLookupd))
                {
                    nsqLookupdAddrs = configNsqLookupd.Split(',').Select(s => s.Trim()).ToArray();
                }
                else
                {
                    nsqLookupdAddrs = new[] { "127.0.0.1:4161" };
                }

                string nsqdTcpAddr = Environment.GetConfig("NsqdTcp");
                if (string.IsNullOrEmpty(nsqdTcpAddr))
                {
                    nsqdTcpAddr = "127.0.0.1:4150";
                }

                foreach (var addr in nsqLookupdAddrs)
                {
                    Logs.LogInfo($"NsqLookupd address : {addr}");
                }
                Logs.LogInfo($"Nsqd TCP address : {nsqdTcpAddr}");

                Config nsqConfig = new Config
                {
                    // optional override of default config values
                    MaxRequeueDelay = TimeSpan.FromSeconds(15),
                    MaxBackoffDuration = TimeSpan.FromSeconds(2),
                    MaxAttempts = 2
                };
                SuityNsqLogger nsqLogger = new SuityNsqLogger();

                // start the bus
                BusService.Start(new BusConfiguration(
                    new ObjectBuilder(container), // dependency injection container
                    new JsonMessageSerializer(), // message serializer
                    new MessageAuditor(), // receives received, started, and failed notifications
                    new MessageTypeToTopicProvider(Domain, families), // mapping between .NET message types and topics
                    new HandlerTypeToChannelProvider(Domain, handlers), // mapping between IHandleMessages<T> implementations and channels
                    busStateChangedHandler: new BusStateChangedHandler(), // bus starting/started/stopping/stopped
                    defaultNsqLookupdHttpEndpoints: nsqLookupdAddrs, // nsqlookupd address
                    defaultThreadsPerHandler: 1, // threads per handler. tweak based on use case, see handlers in this project.
                    nsqConfig: nsqConfig,
                    nsqLogger: nsqLogger, // logger for NSQ events (see also ConsoleLogger, or implement your own)
                    nsqdPublisher : new NsqdTcpPublisher(nsqdTcpAddr, nsqLogger, nsqConfig)
                // preCreateTopicsAndChannels: true // pre-create topics so we dont have to wait for an nsqlookupd cycle
                ));
            }
            catch (Exception err)
            {
                throw;
            }


            _bus = container.GetInstance<IBus>();
        }
        protected override void Destroy()
        {
            Dispose();
        }

        public NetworkCommand GetCommand<T>()
        {
            return _commands.GetValueOrDefault(typeof(T));
        }

        public void Dispose()
        {
            BusService.Stop();
            _bus = null;
        }

        public T GetServiceObject<T>() where T : class
        {
            return this as T;
        }

        private Container SetupDependencyInjectionContainer()
        {
            return new Container(x =>
            {
                //x.Scan(scan =>
                //{
                //    scan.TheCallingAssembly();
                //    scan.WithDefaultConventions();
                //});

                x.For<ICounter>(Lifecycles.Singleton).Use<Counter>();
                x.For<SuityNsqBinding>().Add(this);
            });
        }

        public override void Send<T>(T message)
        {
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Upload, ModuleBindingNames.MessageQueue, string.Empty, message);

            try
            {
                _bus.Send(message);
            }
            catch (Exception err)
            {
                Logs.LogError(err);
            }
        }


        #region IInfoNode
        void IInfoNode.WriteInfo(INodeWriter writer)
        {
            foreach (var item in _commands.OfType<IInfoNode>())
            {
                item.WriteInfo(writer);
            }
        }
        #endregion
    }
}
