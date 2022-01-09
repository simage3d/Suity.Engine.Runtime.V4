using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Suity.Collections;
using Suity.Crypto;
using Suity.Engine;
using Suity.Helpers;
using Suity.Networking;
using Suity.Networking.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Suity.Modules
{
    public class NettyBinding : NetworkServer, IModuleBinding
    {
        readonly ModuleConfig _config;
        readonly List<NetworkCommandFamily> _commandFamilies = new List<NetworkCommandFamily>();
        readonly Dictionary<string, NetworkCommand> _commands = new Dictionary<string, NetworkCommand>();
        readonly ConcurrentDictionary<Type, object> _services = new ConcurrentDictionary<Type, object>();
        IBehaviorLog _behaviorLog;

        public NettyBinding(ModuleConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _services[typeof(NetworkServer)] = this;

            foreach (NetworkCommandFamily family in _config.GetItem(NetworkConfigs.CommandFamilies).OfType<NetworkCommandFamily>())
            {
                RegisterCommandFamily(family);
                _commandFamilies.Add(family);
                //Logs.LogInfo("SuperSocket server added command family : " + family.Name);
            }
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
                    Logs.LogWarning($"Ingored command {command.Name} which defines a method : {command.Method}.");
                    continue;
                }

                if (_commands.ContainsKey(command.Name))
                {
                    throw new ArgumentException("Command name is already registered : " + command.Name);
                }
                _commands.Add(command.Name, command);
            }
        }
        public void AddService<T>(T service)
        {
            _services[typeof(T)] = service;
        }

        /// <summary>
        /// 服务器是否已运行
        /// </summary>
        private bool _isServerRunning = false;
        /// <summary>
        /// 关闭侦听器事件
        /// </summary>
        private ManualResetEvent ClosingArrivedEvent = new ManualResetEvent(false);



        public override bool IsStarted => _isServerRunning;


        public override void Start()
        {
            try
            {
                if (_isServerRunning)
                {
                    ClosingArrivedEvent.Set();  // 停止侦听
                }
                else
                {
                    string ip = _config.GetItem(NetworkConfigs.IP);
                    int port = _config.GetItem(NetworkConfigs.Port);
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = "127.0.0.1";
                    }

                    IPAddress serverIP = IPAddress.Parse(ip); // 服务器地址
                    int serverPort = port; // 服务器端口
                    int backlog = 100; // 最大连接等待数


                    //线程池任务
                    ThreadPool.QueueUserWorkItem(ThreadPoolCallback,
                        new TcpServerParams()
                        {
                            ServerIP = serverIP,
                            ServerPort = serverPort,
                            Backlog = backlog
                        });
                    //var args= (new TcpServerParams()
                    //{
                    //    ServerIP = ServerIP,
                    //    ServerPort = ServerPort,
                    //    Backlog = Backlog
                    //});
                    //RunServerAsync(args).ConfigureAwait(false);
                }
            }
            catch (Exception exp)
            {
            }
        }
        public override void Stop()
        {
        }


        private void ThreadPoolCallback(object state)
        {
            TcpServerParams Args = state as TcpServerParams;
            RunServerAsync(Args).Wait();
        }
        public async Task RunServerAsync(TcpServerParams args)
        {
            PacketFormats packetFormat = _config.GetItem(NetworkConfigs.PacketFormat, PacketFormats.Binary);
            bool compressed = _config.GetItem(NetworkConfigs.Compressed, false);
            AesKey aesKey = _config.GetItem(NetworkConfigs.AesKey);
            int maxLen = _config.GetItem(NetworkConfigs.MaxRequestLength);


            IEventLoopGroup bossGroup;
            IEventLoopGroup workerGroup;

            bossGroup = new MultithreadEventLoopGroup(1);
            workerGroup = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, args.Backlog) //设置网络IO参数等
                .Option(ChannelOption.SoKeepalive, true)//保持连接
                .Handler(new LoggingHandler("SRV-LSTN"))//在主线程组上设置一个打印日志的处理器
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    //工作线程连接器 是设置了一个管道，服务端主线程所有接收到的信息都会通过这个管道一层层往下传输
                    //同时所有出栈的消息 也要这个管道的所有处理器进行一步步处理
                    IChannelPipeline pipeline = channel.Pipeline;

                    //IdleStateHandler 心跳
                    //pipeline.AddLast(new IdleStateHandler(150, 0, 0));//第一个参数为读，第二个为写，第三个为读写全部

                    //出栈消息，通过这个handler 在消息顶部加上消息的长度
                    pipeline.AddLast("framing-enc", new H5PackageSender(packetFormat, compressed, aesKey));
                    //入栈消息通过该Handler,解析消息的包长信息，并将正确的消息体发送给下一个处理Handler
                    pipeline.AddLast("framing-dec", new H5PackageFilter(packetFormat, compressed, aesKey, maxLen));

                    pipeline.AddLast("NettyServer", new ServerHandler(this));
                }));

                IChannel boundChannel = await bootstrap.BindAsync(args.ServerIP, args.ServerPort);

                //运行至此处，服务启动成功
                _isServerRunning = true;

                Logs.LogInfo($"DotNetty is listening on {args.ServerIP}:{args.ServerPort}");

                ClosingArrivedEvent.Reset();
                ClosingArrivedEvent.WaitOne();
                await boundChannel.CloseAsync();
            }
            finally
            {
                await Task.WhenAll(
                    bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }


        public void Dispose()
        {
        }
        public T GetServiceObject<T>() where T : class
        {
            object service;
            if (_services.TryGetValue(typeof(T), out service) && service is T t)
            {
                return t;
            }

            service = _config.GetService(typeof(T));
            if (service != null && typeof(T).IsAssignableFrom(service.GetType()))
            {
                _services[typeof(T)] = service;
                return (T)service;
            }

            return default(T);
        }

        public IBehaviorLog BehaviorLog
        {
            get
            {
                if (_behaviorLog == null)
                {
                    _behaviorLog = GetServiceObject<IBehaviorLog>();
                }
                return _behaviorLog ?? EmptyBehaviorLog.Empty;
            }
        }

        internal void ExecuteCommand(NettySession session, NettyRequestInfo requestInfo)
        {
            try
            {
                session.LogIncomingPackage(requestInfo.Body, requestInfo.Method, requestInfo.Channel);
                session.EnterExecute();
                var command = _commands.GetValueOrDefault(requestInfo.Key);
                if (command != null)
                {
                    var result = command.ExecuteCommand(session, requestInfo);
                    if (result is IFuture future)
                    {
                        future
                            .OnResult(o => session.Send(o, requestInfo.Method, requestInfo.Channel))
                            .OnError(err => session.Send(err, requestInfo.Method, requestInfo.Channel));
                    }
                    else if (result != null)
                    {
                        session.Send(result, requestInfo.Method, requestInfo.Channel);
                    }
                }
                else
                {
                }
            }
            finally
            {
                session.ExitExecute();
            }
        }
    }

    public class TcpServerParams
    {
        public IPAddress ServerIP { get; set; }

        public int ServerPort { get; set; }

        public int Backlog { get; set; } = 100;
    }
}
