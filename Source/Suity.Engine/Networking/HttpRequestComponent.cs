// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Synchonizing;
using Suity.Views;
using Suity.Engine;

namespace Suity.Networking
{
    public class HttpRequestComponent : NodeComponent, IViewObject
    {
        AssetRef<NetworkUpdaterFamily> _updaterRef = new AssetRef<NetworkUpdaterFamily>();

        NetworkSender _sender;

        public override string Icon => "*CoreIcon|Network";

        public bool AutoDiscovery { get; private set; }
        public string Uri { get; private set; } = "http://localhost:8400";
        public string ApiPath { get; private set; } = string.Empty;

        public NetworkUpdaterFamily UpdaterFamily { get; private set; }
        public NetworkRequest Request { get; private set; }
        public bool RsaConfig { get; private set; }
        readonly StringConfig _publicKeyConfig = new StringConfig();

        readonly VersionConfig _versionConfig = new VersionConfig();



        protected override void OnStart()
        {
            UpdaterFamily = ObjectType.GetAssetImplement<NetworkUpdaterFamily>(_updaterRef.Key);
            if (UpdaterFamily == null)
            {
                throw new ModuleException("Network command family not found : " + _updaterRef.Key);
            }
            

            ModuleConfig config = new ModuleConfig(this);
            // 设置用于AutoDicovery
            config.SetItem(NetworkConfigs.UpdaterFamily, UpdaterFamily);

            Request = BindModule<NetworkRequest>(ModuleBindingNames.HttpRequest, config);
            if (Request == null)
            {
                throw new ModuleBindException("Bind module failed : HttpRequest");
            }

            _sender = ObjectType.GetAssetImplement<NetworkSender>(_updaterRef.Key);
            _sender?.SetRequester(Request);

            base.OnStart();
        }
        protected override void OnStop()
        {
            base.OnStop();

            UpdaterFamily = null;
            Request = null;
            _sender = null;
        }

        public T GetSender<T>() where T : NetworkSender
        {
            return _sender as T;
        }

        public override object GetService(Type contentType)
        {
            //需要类型精确相等
            if (_sender != null && _sender.GetType() == contentType)
            {
                return _sender;
            }

            return null;
        }

        #region IViewObject
        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            AutoDiscovery = sync.Sync(NetworkConfigs.AutoDiscovery.Name, AutoDiscovery, SyncFlag.AffectsOthers);
            Uri = sync.Sync(NetworkConfigs.Uri.Name, Uri);
            ApiPath = sync.Sync(NetworkConfigs.ApiPath.Name, ApiPath);

            if (ParentObject != null)
            {
                sync.Sync(NetworkConfigs.PublicKey.Name, _publicKeyConfig.GetValue(), SyncFlag.ReadOnly);
                sync.Sync(NetworkConfigs.Version.Name, _versionConfig.GetValue(), SyncFlag.ReadOnly);
            }

            RsaConfig = sync.Sync(nameof(RsaConfig), RsaConfig, SyncFlag.AffectsOthers);
            sync.Sync("PublicKeyConfig", _publicKeyConfig, SyncFlag.ReadOnly);

            _updaterRef = sync.Sync("Updater", _updaterRef, SyncFlag.NotNull);
            sync.Sync("UpdaterVersion", _versionConfig, SyncFlag.ReadOnly);
        }
        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            setup.InspectorField(AutoDiscovery, new ViewProperty(NetworkConfigs.AutoDiscovery.Name));
            if (!AutoDiscovery)
            {
                setup.InspectorField(Uri, new ViewProperty(NetworkConfigs.Uri.Name));
                setup.InspectorField(ApiPath, new ViewProperty(NetworkConfigs.ApiPath.Name));
            }

            if (ParentObject != null)
            {
                if (RsaConfig)
                {
                    setup.InspectorFieldOf<string>(new ViewProperty(NetworkConfigs.PublicKey.Name));
                }
            }
            else
            {
                setup.InspectorField(RsaConfig, new ViewProperty(nameof(RsaConfig)));
                if (RsaConfig)
                {
                    setup.InspectorField(_publicKeyConfig, new ViewProperty("PublicKeyConfig"));
                }
            }

            setup.InspectorField(_updaterRef, new ViewProperty("Updater"));
            setup.InspectorField(_versionConfig, new ViewProperty("UpdaterVersion"));
        }

        #endregion
    }
}
