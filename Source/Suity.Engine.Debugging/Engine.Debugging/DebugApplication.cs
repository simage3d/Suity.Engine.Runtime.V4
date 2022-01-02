// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Suity.Clustering;
using Suity.Collections;
using Suity.Helpers;
using Suity.Networking;
using Suity.Reflecting;
using Suity.Synchonizing;
using Suity.Synchonizing.Core;
using Suity.Synchonizing.Preset;
using Suity.Views;


namespace Suity.Engine.Debugging
{
    #region DebugApplication
    class DebugApplication : NodeApplication
    {
        readonly IDebugHostService _hostService;
        readonly internal DebugNode _debugNode;
        readonly internal NodeStartInfo _startInfo;
        readonly internal IDebugInstanceService _instanceService;
        readonly internal DebugModuleProvider _moduleProvider;

        readonly Queue<Action> _actionQueue = new Queue<Action>();

        bool _exiting;

        public override string ServiceId => _startInfo.ApplicationName;
        public override int MultipleLaunchIndex => _startInfo.MultipleLaunchIndex;
        public override string GalaxyName => _startInfo.GalaxyName;
        public override string GalaxyId => _startInfo.GalaxyId;
        public override string GalaxyVersion => _startInfo.GalaxyVersion;
        public override string StellarId => "Debug";
        public override string ApplicationName => _startInfo.ApplicationName;
        public override string ApplicationId => _startInfo.ApplicationId;

        public override string DataId => _startInfo.DataId;
        public override string DataVersion => _startInfo.DataVersion ?? _startInfo.GalaxyVersion;


        public IDebugHostService DebugHost => _hostService;
        public IDebugInstanceService DebugInstance => _instanceService;


        public DebugApplication(IDebugHostService hostService, IDebugInstanceService instanceService, DebugNode debugNode)
        {
            _hostService = hostService ?? throw new ArgumentNullException(nameof(hostService));
            _debugNode = debugNode ?? throw new ArgumentNullException(nameof(debugNode));
            _instanceService = instanceService ?? throw new ArgumentNullException(nameof(instanceService));

            _startInfo = debugNode.StartInfo;
            DebugDevice.Instance._app = this;

            _moduleProvider = new DebugModuleProvider(_instanceService, debugNode, debugNode.StartInfo.ModuleDirectory);

            ConsoleCommand.AddCommand(new ExitCommand());
        }
        protected override void OnStarted()
        {
            base.OnStarted();
            _moduleProvider.StartNodeObject(this.Node);
            new Thread(ActionQueueThread).Start();
        }
        protected override void OnStopped()
        {
            _exiting = true;
            _moduleProvider.Stop();
            base.OnStopped();
        }

        internal void QueueAction(Action action)
        {
            if (action == null)
            {
                return;
            }
            
            lock (_actionQueue)
            {
                _actionQueue.Enqueue(action);
            }
        }
        private void ActionQueueThread()
        {
            while (!_exiting)
            {
                if (_actionQueue.Count > 0)
                {
                    lock (_actionQueue)
                    {
                        while (_actionQueue.Count > 0)
                        {
                            Action action = _actionQueue.Dequeue();
                            action();
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }



        protected override NodeObject CreateNodeObject()
        {
            NodeObject obj = new NodeObject();
            foreach (var compStartInfo in _startInfo.Components)
            {
                Type type = DefaultSyncTypeResolver.Instance.ResolveType(compStartInfo.FullTypeName, null);
                if (type != null && typeof(NodeComponent).IsAssignableFrom(type))
                {
                    try
                    {
                        NodeComponent component = obj.AddComponent(type);
                        if (component != null)
                        {
                            component.Name = compStartInfo.ComponentName;
                            ISyncObject syncObj = component as ISyncObject;
                            if (syncObj != null && compStartInfo.ComponentData != null)
                            {
                                Serializer.Deserialize(syncObj, compStartInfo.ComponentData, DeserializeSyncTypeResolver.Instance);
                            }
                        }
                        else
                        {
                            Logs.LogError("创建组件失败:" + compStartInfo.FullTypeName);
                            return null;
                        }
                    }
                    catch (Exception)
                    {
                        Logs.LogError("无法创建组件:" + compStartInfo.FullTypeName);
                        return null;
                    }
                }
                else
                {
                    Logs.LogError("找不到组件类型 : " + compStartInfo.FullTypeName);
                }
            }

            return obj;
        }


        internal void AliveCheck()
        {
            _moduleProvider.AliveCheck();
        }

        internal object GetService(Type serviceType)
        {
            var result = Node?.GetService(serviceType);
            if (result != null)
            {
                return result;
            }

            if (serviceType == typeof(IModuleProvider))
            {
                return _moduleProvider;
            }
            if (serviceType == typeof(NodeStartInfo))
            {
                return _startInfo;
            }
            if (serviceType == typeof(IGlobalClusterInfo))
            {
                return _debugNode;
            }
            if (serviceType == typeof(ConsoleCommandService))
            {
                return ConsoleCommand;
            }

            try
            {
                result = _instanceService.GetService(serviceType);
                if (result != null)
                {
                    Logs.LogInfo($"Get service from editor : {serviceType.FullName} - {result.GetType().FullName}");
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    #endregion

    #region ConsoleRuntimeLog

    class ConsoleRuntimeLog : IRuntimeLog
    {
        public static readonly ConsoleRuntimeLog Instance = new ConsoleRuntimeLog();

        public void AddLog(LogMessageType type, object message)
        {
            Console.WriteLine(message?.ToString() ?? string.Empty);
        }
    }

    #endregion

    #region ConsoleNetworkLog

    class ConsoleNetworkLog : INetworkLog
    {
        public static readonly ConsoleNetworkLog Instance = new ConsoleNetworkLog();

        public void AddNetworkLog(LogMessageType type, NetworkDirection direction, string sessionId, string channelId, object message)
        {
            Console.WriteLine(message?.ToString() ?? string.Empty);
        }
    }

    #endregion

    #region Commands
    class ExitCommand : ConsoleCommand
    {
        public ExitCommand()
            : base("exit", null, "Stop and exit application.")
        {
        }

        public override void ExecuteComand(string[] args)
        {
            NodeApplication.Current?.Stop();
            Thread.Sleep(500);
        }
    } 
    #endregion
}
