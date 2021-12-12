// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Controlling;
using Suity.Networking;
using Suity.Synchonizing;
using Suity.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Suity.Engine.Debugging
{
    public class ControllerDebugComponent : NodeComponent, IViewObject
    {
        public const string EventDefault_DataReceived = "DataReceived";
        public const string EventDefault_Disconnected = "Disconnected";
        public const string Key_DebugRunner = "DebugRunner";

        AssetRef<Controller> _controllerRef = new AssetRef<Controller>();
        readonly List<AssetRef<NetworkCommandFamily>> _commandRefs = new List<AssetRef<NetworkCommandFamily>>();

        int _runnerCount = 1;
        int _frameRate = 30;
        string _event_DataReceived = EventDefault_DataReceived;
        string _event_Disconnected = EventDefault_Disconnected;
        DebugNetworkServer _server;

        readonly List<ControllerDebugRunner> _runners = new List<ControllerDebugRunner>();

        public override string Icon => "*CoreIcon|Debug";

        protected override void OnStart()
        {
            base.OnStart();

            _server = new DebugNetworkServer();
            foreach (var commandFamily in _commandRefs.Select(o => o.GetInstance()).OfType<NetworkCommandFamily>())
            {
                _server.RegisterCommandFamily(commandFamily);
            }

            _runners.Clear();

            string name = Name;
            if (string.IsNullOrEmpty(name))
            {
                name = "DebugRunner";
            }

            if (_runnerCount > 0)
            {
                for (int i = 0; i < _runnerCount; i++)
                {
                    var controller = _controllerRef.CreateInstance();
                    if (controller == null)
                    {
                        throw new NullReferenceException("Controller create failed : " + _controllerRef.Key);
                    }

                    string instanceName = $"{name}_{i}";

                    ControllerDebugRunner runner = new ControllerDebugRunner(_server, controller, _frameRate, i, instanceName);

                    runner.Event_DataReceived = _event_DataReceived;
                    runner.Event_Disconnected = _event_Disconnected;
                    _runners.Add(runner);
                }
            }
            //Logs.LogDebug("Controller created : " + _controller.GetType().Name);
            //NodeApplication.Current.AddNetworkLog


        }
        protected override void OnStop()
        {
            base.OnStop();

            foreach (var runner in _runners)
            {
                runner.Dispose();
            }
            _runners.Clear();
        }

        #region IViewObject
        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            setup.AllInspectorField(this);
        }

        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            _controllerRef = sync.Sync("Controller", _controllerRef, SyncFlag.NotNull);
            sync.Sync("Commands", _commandRefs, SyncFlag.ReadOnly);
            _runnerCount = sync.Sync("RunnerCount", _runnerCount, SyncFlag.None, 1);
            _frameRate = sync.Sync("FrameRate", _frameRate, SyncFlag.None, 30);
            _event_DataReceived = sync.Sync("EventDataReceived", _event_DataReceived, SyncFlag.NotNull, EventDefault_DataReceived);
            _event_Disconnected = sync.Sync("EventDisconnected", _event_Disconnected, SyncFlag.NotNull, EventDefault_Disconnected);
        }
        #endregion
    }

    public static class DebugComponentExtensions
    {
        public static IDebugRunner GetDebugRunner(this FunctionContext context)
        {
            return context.GetArgument(ControllerDebugComponent.Key_DebugRunner) as IDebugRunner;
        }
    }

    public interface IDebugRunner
    {
        int Index { get; }

        string Name { get; }

        NetworkSession Session { get; }

        object Send(object request, NetworkDeliveryMethods method, int channel);

        void Connect();

        void Close();
    }

    class ControllerDebugRunner : IDebugRunner, IDisposable
    {
        Timer _timer;
        readonly DebugNetworkServer _server;
        readonly Controller _controller;
        readonly FunctionContext _context;

        DebugNetworkSession _session;


        public string Event_DataReceived { get; set; } = ControllerDebugComponent.EventDefault_DataReceived;
        public string Event_Disconnected { get; set; } = ControllerDebugComponent.EventDefault_Disconnected;



        public ControllerDebugRunner(DebugNetworkServer server, Controller controller, int framerate, int index, string name)
        {
            if (server == null)
            {
                throw new ArgumentNullException(nameof(server));
            }
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            Index = index;
            Name = name;


            _server = server;
            _controller = controller;
            _context = new FunctionContext();
            _context.SetArgument(ControllerDebugComponent.Key_DebugRunner, this);

            _session = _server.CreateSession();
            _session.DataSent += _session_DataSent;
            _session.Disconnected += _session_Disconnected;

            _controller.Start(_context);
            _controller.Enter(_context);

            TimeSpan timeSpan = TimeSpan.FromSeconds((double)1 / (double)framerate);
            _timer = new Timer(Update, null, timeSpan, timeSpan);
        }

        #region IDebugRunner
        public int Index { get; private set; }
        public string Name { get; private set; }
        public NetworkSession Session => _session;
        public void Connect()
        {
            if (_session == null)
            {
                CreateSession();
            }
        }
        public void Close()
        {
            try
            {
                _session?.Close();
            }
            catch (Exception err)
            {
                Logs.LogError(err);
            }            
        }
        public object Send(object request, NetworkDeliveryMethods method, int channel)
        {
            try
            {
                return _Send(request, method, channel);
            }
            catch (Exception err)
            {
                Logs.LogError(err);
                return null;
            }
        } 

        private object _Send(object request, NetworkDeliveryMethods method, int channel)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (_session == null)
            {
                return new ErrorResult { StatusCode = StatusCodes.ClientError.ToString() };
            }

            return _session.HandleRequest(request, method, channel);
        }
        #endregion

        private void CreateSession()
        {
            if (_session != null)
            {
                _session.Close();
            }

            _session = _server.CreateSession();
            _session.DataSent += _session_DataSent;
            _session.Disconnected += _session_Disconnected;
        }

        private void _session_DataSent(object data, int channel)
        {
            var context = new FunctionContext(_context, Event_DataReceived, data);
            _controller.DoAction(context);
        }
        private void _session_Disconnected()
        {
            var context = new FunctionContext(_context, Event_Disconnected);
            _controller.DoAction(context);

            _session.DataSent -= _session_DataSent;
            _session.Disconnected -= _session_Disconnected;
            _session = null;
        }

        private void Update(object state)
        {
            try
            {
                _controller.Update(_context);
            }
            catch (Exception err)
            {
                Logs.LogError(err);
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;

            _controller.Exit(_context);
            _controller.Stop(_context);
        }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }


}
