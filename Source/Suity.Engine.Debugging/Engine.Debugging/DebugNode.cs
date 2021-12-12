// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Suity.AutoDiscovery;
using Suity.Clustering;
using Suity.Collections;
using Suity.Helpers;

namespace Suity.Engine.Debugging
{
    public class DebugNode : MarshalByRefObject, IDebugNode, IServiceDiscoveryHost, IGlobalClusterInfo
    {
        public static readonly TimeSpan AliveCheckStartTimeSpan = TimeSpan.FromSeconds(3);
        public static readonly TimeSpan AliveCheckTimeSpan = TimeSpan.FromSeconds(10);


        readonly NodeStartInfo _info;
        DebugApplication _application;

        internal NodeStartInfo StartInfo => _info;
        internal DebugApplication Application => _application;

        // 默认只处理单个client (指代 ServiceDiscoveryModule)
        IServiceDiscoveryClient _client;
        Timer _timer;

        public DebugNode(NodeStartInfo info)
        {
            _info = info ?? throw new ArgumentNullException(nameof(info));
        }
        public override object InitializeLifetimeService() => null;

        internal void Start(IDebugHostService debugHost, IDebugInstanceService debugInstance)
        {
            if (_application != null)
            {
                throw new InvalidOperationException("Application is already started.");
            }

            _application = new DebugApplication(debugHost, debugInstance, this);
            NodeApplication.Current = _application;

            _timer = new Timer(CheckLifeTime, null, AliveCheckStartTimeSpan, AliveCheckTimeSpan);
        }

        #region IDebugNode
        public string NodeId => _info.NodeId;

        public DebugServiceLocation[] GetServiceLocations()
        {
            var locations = _client?.GetServiceLocations() ?? EmptyArray<ServiceLocation>.Empty;
            return locations.Select(o => o.ToDebugLocation()).ToArray();
        }
        public void AddServiceDiscovery(DebugServiceLocation location)
        {
            _client?.AddServiceDiscovery(location.ToLocation());
        }
        public void RemoveServiceDiscovery(DebugServiceLocation location)
        {
            _client?.RemoveServiceDiscovery(location.ToLocation());
        } 
        #endregion

        #region IServiceDiscoveryHost
        public void AddServiceLocation(IServiceDiscoveryClient client, ServiceLocation location)
        {
            if (client == null || location == null)
            {
                return;
            }

            _client = client;
            Debugger._debugInstance?.AddServiceLocation(location.ToDebugLocation());
        }

        public void RemoveServiceLocation(IServiceDiscoveryClient client, ServiceLocation location)
        {
            if (client == null || location == null)
            {
                return;
            }

            _client = client;
            Debugger._debugInstance?.RemoveServiceLocation(location.ToDebugLocation());
        }

        public void RequestAutoDiscovery(IServiceDiscoveryClient client, ServiceCode serviceCode)
        {
            if (client == null || string.IsNullOrEmpty(serviceCode))
            {
                return;
            }

            _client = client;
            Debugger._debugInstance?.RequestAutoDiscovery(serviceCode);
        }

        public void CancelAutoDiscovery(IServiceDiscoveryClient client, ServiceCode serviceCode)
        {
            if (client == null || string.IsNullOrEmpty(serviceCode))
            {
                return;
            }

            _client = client;
            Debugger._debugInstance?.CancelAutoDiscovery(serviceCode);
        }
        #endregion

        #region Alive
        private void CheckLifeTime(object state)
        {
            try
            {
                _application?.AliveCheck();
                Debugger._debugInstance?.NodeAlive(_info.GalaxyName);
            }
            catch (Exception err)
            {
                Logs.LogError(err);
            }
        }

        #endregion

        #region IGlobalServiceInfo

        ServiceLocation[] IGlobalClusterInfo.GetAllServiceLocations()
        {
            return Debugger._debugInstance?.GetGlobalServiceLocations()?.Select(o => o.ToLocation()).ToArray() ?? EmptyArray<ServiceLocation>.Empty;
        }

        #endregion
    }
}
