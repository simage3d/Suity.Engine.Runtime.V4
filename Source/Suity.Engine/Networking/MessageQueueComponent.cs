// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Engine;
using Suity.Synchonizing;
using Suity.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    /// <summary>
    /// 消息队列组件
    /// </summary>
    public class MessageQueueComponent : NodeComponent, IViewObject
    {
        readonly StringConfig _domainNameConfig = new StringConfig();
        readonly StringConfig _domainPrivateKeyConfig = new StringConfig();

        string _domainName;
        string _domainPrivateKey;

        readonly List<AssetRef<NetworkCommandFamily>> _messageTypeRefs = new List<AssetRef<NetworkCommandFamily>>();
        readonly List<AssetRef<NetworkCommand>> _handlerRefs = new List<AssetRef<NetworkCommand>>();

        readonly List<NetworkCommandFamily> _families = new List<NetworkCommandFamily>();
        readonly List<NetworkCommand> _handlers = new List<NetworkCommand>();

        MessageQueue _queue;

        public override string Icon => "*CoreIcon|Network";

        public string DomainName => _domainName;
        public string DomainPrivateKey => _domainPrivateKey;

        protected override void OnStart()
        {
            foreach (var assetKey in _messageTypeRefs.Select(o => o.Key).Where(o => !string.IsNullOrEmpty(o)))
            {
                NetworkCommandFamily family = ObjectType.GetAssetImplement<NetworkCommandFamily>(assetKey);
                if (family == null)
                {
                    throw new ModuleException("Network command family not found : " + assetKey);
                }
                _families.Add(family);
            }

            foreach (var assetKey in _handlerRefs.Select(o => o.Key).Where(o => !string.IsNullOrEmpty(o)))
            {
                NetworkCommand family = ObjectType.GetAssetImplement<NetworkCommand>(assetKey);
                if (family == null)
                {
                    throw new ModuleException("Network command not found : " + assetKey);
                }
                _handlers.Add(family);
            }

            _domainName = _domainNameConfig.GetValue();
            _domainPrivateKey = _domainPrivateKeyConfig.GetValue();

            ModuleConfig config = new ModuleConfig(this);
            config.SetItem(NetworkConfigs.CommandFamilies, _families);
            config.SetItem(NetworkConfigs.CommandHandlers, _handlers);

            _queue = BindModule<MessageQueue>(ModuleBindingNames.MessageQueue, config);

            base.OnStart();
        }
        protected override void OnStop()
        {
            base.OnStop();

            _families.Clear();
            _handlers.Clear();
        }

        public void Send<T>(T message)
        {
            _queue?.Send(message);
        }

        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            if (ParentObject != null)
            {
                setup.InspectorField(_domainName, new ViewProperty(NetworkConfigs.DomainName.Name, "域名"));
                setup.InspectorField(_domainPrivateKey, new ViewProperty(NetworkConfigs.DomainPrivateKey.Name, "密钥"));
            }
            else
            {
                setup.InspectorField(_domainNameConfig, new ViewProperty("DomainNameConfig", "域名"));
                setup.InspectorField(_domainPrivateKeyConfig, new ViewProperty("DomainPrivateKeyConfig", "密钥"));
            }
            setup.InspectorField(_messageTypeRefs, new ViewProperty("MessageTypes", "指令集"));
            setup.InspectorField(_handlerRefs, new ViewProperty("Handlers", "指令"));
        }
        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            sync.Sync("DomainNameConfig", _domainNameConfig, SyncFlag.ReadOnly);
            sync.Sync("DomainPrivateKeyConfig", _domainPrivateKeyConfig, SyncFlag.ReadOnly);
            sync.Sync(NetworkConfigs.DomainName.Name, _domainName, SyncFlag.ReadOnly);
            sync.Sync(NetworkConfigs.DomainPrivateKey.Name, _domainPrivateKey, SyncFlag.ReadOnly);
            sync.Sync("MessageTypes", _messageTypeRefs, SyncFlag.ReadOnly);
            sync.Sync("Handlers", _handlerRefs, SyncFlag.ReadOnly);
        }
    }
}
