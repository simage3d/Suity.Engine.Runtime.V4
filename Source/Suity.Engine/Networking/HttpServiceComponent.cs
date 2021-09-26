// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Suity.Collections;
using Suity.Crypto;
using Suity.Engine;
using Suity.Synchonizing;
using Suity.Views;
using System.Collections.Generic;
using System.Linq;

namespace Suity.Networking
{
    /// <summary>
    /// Http服务组件
    /// </summary>
    public sealed class HttpServiceComponent : NodeComponent, IViewObject
    {
        private readonly List<AssetRef<NetworkCommandFamily>> _familyRefs = new List<AssetRef<NetworkCommandFamily>>();
        private readonly List<NetworkCommandFamily> _families = new List<NetworkCommandFamily>();
        private readonly List<string> _handlerComponents = new List<string>();
        private readonly StringConfig _contentRealPathConfig = new StringConfig();
        private readonly StringConfig _contentRequestPathConfig = new StringConfig();
        private readonly IPConfig _ipConfig = new IPConfig();
        private readonly PortConfig _portConfig = new PortConfig();
        private readonly StringConfig _publicKeyConfig = new StringConfig();
        private readonly StringConfig _privateKeyConfig = new StringConfig();
        private NetworkServer _server;
        private bool _useContentConfig = true;
        private bool _usePortConfig = true;

        public string ApiPath { get; private set; } = string.Empty;
        public IEnumerable<NetworkCommandFamily> CommandFamilies => _families;
        public string ContentRealPath => _contentRealPathConfig.GetValue();
        public string ContentRequestPath => _contentRequestPathConfig.GetValue();
        public override string Icon => "*CoreIcon|Network";

        public int Port { get; private set; }
        public bool RsaConfig { get; private set; }
        public bool ServiceProxy { get; private set; }
        public string Uri { get; private set; } = "http://localhost";

        protected override void OnStart()
        {
            foreach (var assetKey in _familyRefs.Select(o => o.Key).Where(o => !string.IsNullOrEmpty(o)))
            {
                NetworkCommandFamily family = ObjectType.GetAssetImplement<NetworkCommandFamily>(assetKey);
                if (family == null)
                {
                    throw new ModuleException("Network command family not found : " + assetKey);
                }
                _families.Add(family);
            }
            foreach (var handler in _handlerComponents.Select(name => GetComponent<HttpHandlerComponent>(name)).SkipNull())
            {
                _families.Add(handler.Family);
            }

            ModuleConfig config = new ModuleConfig(this);
            config.SetItem(NetworkConfigs.CommandFamilies, CommandFamilies);

            if (RsaConfig)
            {
                RsaKey rsaKey = new RsaKey { Public = _publicKeyConfig.GetValue(), Private = _privateKeyConfig.GetValue() };
                config.SetItem(NetworkConfigs.RsaKey, rsaKey);
            }

            string host = _ipConfig.GetIPAddress();
            if (host == IPConfig.LocalHost)
            {
                host = "localhost";
            }
            Uri = "http://" + host;

            if (_usePortConfig)
            {
                Port = _portConfig.GetTcpPort(NodeApplication.Current.MultipleLaunchIndex);
                if (Port < 0)
                {
                    throw new ComponentStartException("Port allocation failed : " + _portConfig.ToString());
                }
                Uri += ":" + Port;
            }

            _server = BindModule<NetworkServer>(ModuleBindingNames.HttpService, config);
            if (_server == null)
            {
                throw new ModuleBindException("Bind module failed : HttpService");
            }

            _server.Start();
            base.OnStart();
        }

        protected override void OnStop()
        {
            base.OnStop();
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }
            _families.Clear();
        }

        #region IViewObject

        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            if (ParentObject != null)
            {
                setup.InspectorField(Uri, new ViewProperty(NetworkConfigs.Uri.Name));
                if (_useContentConfig)
                {
                    setup.InspectorFieldOf<string>(new ViewProperty(NetworkConfigs.ContentRequestPath.Name));
                    setup.InspectorFieldOf<string>(new ViewProperty(NetworkConfigs.ContentRealPath.Name));
                }
                if (RsaConfig)
                {
                    setup.InspectorFieldOf<string>(new ViewProperty(NetworkConfigs.PublicKey.Name));
                    setup.InspectorFieldOf<string>(new ViewProperty(NetworkConfigs.PrivateKey.Name));
                }
            }
            else
            {
                setup.Label("AddressLabel", "地址");

                setup.InspectorField(_ipConfig, new ViewProperty("IPConfig", "IP"));

                setup.InspectorField(_usePortConfig, new ViewProperty("UsePortConfig", "使用端口配置"));
                if (_usePortConfig)
                {
                    setup.InspectorField(_portConfig, new ViewProperty("PortConfig", "端口"));
                }

                setup.Label("ContentLabel", "内容");

                setup.InspectorField(_useContentConfig, new ViewProperty("UseContentConfig", "使用内容配置"));
                if (_useContentConfig)
                {
                    setup.InspectorField(_contentRequestPathConfig, new ViewProperty("RequestPathConfig", "请求路径"));
                    setup.InspectorField(_contentRealPathConfig, new ViewProperty("RealPathConfig", "物理路径"));
                }

                setup.Label("RsaLabel", "加密");

                setup.InspectorField(RsaConfig, new ViewProperty(nameof(RsaConfig), "Rsa加密配置"));
                if (RsaConfig)
                {
                    setup.InspectorField(_publicKeyConfig, new ViewProperty("PublicKeyConfig", "公钥"));
                    setup.InspectorField(_privateKeyConfig, new ViewProperty("PrivateKeyConfig", "密钥"));
                }
            }

            setup.Label("CommandLabel", "指令");

            setup.InspectorField(ApiPath, new ViewProperty(NetworkConfigs.ApiPath.Name, "Api路径"));
            setup.InspectorField(ServiceProxy, new ViewProperty(NetworkConfigs.ServiceProxy.Name, "使用代理"));

            setup.InspectorField(_familyRefs, new ViewProperty("Commands", "指令"));
            setup.InspectorField(_handlerComponents, new ViewProperty("HandlerComponents", "自定义响应组件"));
        }

        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            _usePortConfig = sync.Sync("UsePortConfig", _usePortConfig, SyncFlag.AffectsOthers);
            _useContentConfig = sync.Sync("UseContentConfig", _useContentConfig, SyncFlag.AffectsOthers);

            if (ParentObject != null)
            {
                sync.Sync(NetworkConfigs.Uri.Name, Uri, SyncFlag.ReadOnly);
                sync.Sync(NetworkConfigs.ContentRequestPath.Name, _contentRequestPathConfig.GetValue(), SyncFlag.ReadOnly);
                sync.Sync(NetworkConfigs.ContentRealPath.Name, _contentRealPathConfig.GetValue(), SyncFlag.ReadOnly);
                sync.Sync(NetworkConfigs.PublicKey.Name, _publicKeyConfig.GetValue(), SyncFlag.ReadOnly);
                sync.Sync(NetworkConfigs.PrivateKey.Name, _privateKeyConfig.GetValue(), SyncFlag.ReadOnly);
            }

            sync.Sync("IPConfig", _ipConfig, SyncFlag.ReadOnly);
            sync.Sync("PortConfig", _portConfig, SyncFlag.ReadOnly);

            sync.Sync("RequestPathConfig", _contentRequestPathConfig, SyncFlag.ReadOnly);
            sync.Sync("RealPathConfig", _contentRealPathConfig, SyncFlag.ReadOnly);

            ApiPath = sync.Sync(NetworkConfigs.ApiPath.Name, ApiPath, SyncFlag.NotNull);
            ServiceProxy = sync.Sync(NetworkConfigs.ServiceProxy.Name, ServiceProxy);

            RsaConfig = sync.Sync(nameof(RsaConfig), RsaConfig, SyncFlag.AffectsOthers);
            sync.Sync("PublicKeyConfig", _publicKeyConfig, SyncFlag.ReadOnly);
            sync.Sync("PrivateKeyConfig", _privateKeyConfig, SyncFlag.ReadOnly);

            sync.Sync("Commands", _familyRefs, SyncFlag.ReadOnly);
            sync.Sync("HandlerComponents", _handlerComponents, SyncFlag.ReadOnly);
        }

        #endregion IInspectorObject
    }
}