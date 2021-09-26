// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Synchonizing;
using Suity.Views;
using Suity.Helpers;
using Suity.Engine;
using Suity.Collections;

namespace Suity.Networking
{
    [NodeService(typeof(INetworkCommandHandler))]
    public class CommandHandlerComponent : NodeComponent, INetworkCommandHandler, IViewObject
    {
        public const string TargetTypeArgName = "type";

        readonly List<AssetRef<NetworkCommandFamily>> _commandRefs = new List<AssetRef<NetworkCommandFamily>>();
        readonly Dictionary<Type, NetworkCommand> _commands = new Dictionary<Type, NetworkCommand>();

        public override string Icon => "*CoreIcon|Command";

        protected override void OnStart()
        {
            base.OnStart();

            foreach (var famly in _commandRefs.Select(o => o.GetInstance()).SkipNull())
            {
                foreach (var command in famly.Commands.Where(o => o.RequestType != null))
                {
                    if (_commands.ContainsKey(command.RequestType))
                    {
                        Logs.LogWarning("Command requested type exists : " + command.RequestType.Name);
                        continue;
                    }

                    _commands.Add(command.RequestType, command);
                }
            }
        }
        protected override void OnStop()
        {
            base.OnStop();

            _commands.Clear();
        }

        #region INetworkCommandHandler

        public object ExecuteCommand(NetworkSession session, INetworkInfo requestInfo, object request)
        {
            NetworkCommand command = _commands.GetValueOrDefault(request.GetType());
            if (command == null)
            {
                throw new ExecuteException(StatusCodes.ServiceUnavailable);
            }

            return command.ExecuteCommand(session, new InnerNetworkInfo(requestInfo, request));
        }
        public object ExecuteCommandTarget<TTarget>(NetworkSession session, INetworkInfo requestInfo, TTarget target, object request)
        {
            NetworkCommand command = _commands.GetValueOrDefault(request.GetType());
            if (command == null)
            {
                throw new ExecuteException(StatusCodes.ServiceUnavailable);
            }

            return command.ExecuteCommandTarget<TTarget>(session, new InnerNetworkInfo(requestInfo, request), target);
        }

        #endregion

        #region IViewObject
        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            sync.Sync("Commands", _commandRefs, SyncFlag.ReadOnly);
        }
        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            setup.InspectorField(_commandRefs, new ViewProperty("Commands"));
        }
        #endregion

        class InnerNetworkInfo : INetworkInfo
        {
            readonly INetworkInfo _inner;
            readonly object _request;

            public InnerNetworkInfo(INetworkInfo inner, object request)
            {
                _inner = inner ?? throw new ArgumentNullException(nameof(inner));
                _request = request;
            }

            public NetworkDeliveryMethods Method => _inner.Method;

            public int Channel => _inner.Channel;

            public string Key => _inner.Key;

            public object Body => _request;


            public object GetArgs(string name) => _inner.GetArgs(name);
        }
    }
}
