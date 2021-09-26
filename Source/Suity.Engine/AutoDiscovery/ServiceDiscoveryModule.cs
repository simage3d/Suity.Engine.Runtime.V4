// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using Suity.Collections;
using Suity.Engine;
using Suity.Helpers;

namespace Suity.AutoDiscovery
{
    public class ServiceDiscoveryModule : Module, IServiceDiscoveryClient
    {
        readonly IServiceDiscoveryHost _host;
        readonly LocalServiceDiscoveryCollection _services = new LocalServiceDiscoveryCollection();

        public event EventHandler<ServiceLocationEventArgs> ServiceLocationAdded;
        public event EventHandler<ServiceLocationEventArgs> ServiceDiscovered;
        public event EventHandler<ServiceLocationEventArgs> ServiceLost;


        internal LocalServiceDiscoveryCollection Services => _services;

        public ServiceDiscoveryModule(IServiceDiscoveryHost host)
            : base(ModuleBindingNames.AutoDiscovery, "Suity Native Service Discovery")
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));

            _services.ServiceDiscovered += (s, e) => OnServiceDiscovered(e.Location);
            _services.ServiceLost += (s, e) => OnServiceLost(e.Location);
        }

        protected override IModuleBinding Bind(ModuleConfig input)
        {
            return new ServiceDiscoveryBinding(this, input);
        }

        internal IEnumerable<ServiceDiscoveryBinding> ServiceDiscoveryBindings => Bindings.Select(o => o.Binding).OfType<ServiceDiscoveryBinding>();

        #region Service Discovery

        public void AddServiceDiscovery(ServiceLocation location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }
            if (string.IsNullOrEmpty(location.ServiceCode))
            {
                throw new ArgumentException(nameof(location.ServiceCode) + " is emtpy.");
            }

            _services.AddLocalService(location);

            try
            {
                //对外发送
                _host.AddServiceLocation(this, location);
            }
            catch (Exception)
            {
            }

            OnServiceLocationAdded(location);
        }
        public IServiceResolveFuture RequestAutoDiscovery(ServiceCode serviceCode)
        {
            if (string.IsNullOrEmpty(serviceCode))
            {
                return EmptyServiceResolveFuture.Empty;
            }

            var future = _services.AddAutoDiscovery(serviceCode);
            _host.RequestAutoDiscovery(this, serviceCode);

            return future;
        }

        #endregion

        #region IServiceDiscoveryClient
        void IServiceDiscoveryClient.AddServiceDiscovery(ServiceLocation location)
        {
            _services.AddGlobalService(location);
        }
        void IServiceDiscoveryClient.RemoveServiceDiscovery(ServiceLocation location)
        {
            _services.RemoveGlobalService(location);
        }

        ServiceLocation[] IServiceDiscoveryClient.GetServiceLocations()
        {
            return _services.GetLocalServices();
        }
        #endregion

        protected virtual void OnServiceLocationAdded(ServiceLocation location)
        {
            if (string.IsNullOrEmpty(location.ServiceCode))
            {
                Logs.LogWarning($"Service discovery meet empty service code.");
                return;
            }
            
            Logs.LogInfo($"Service location added : [{location.ServiceCode}].");
            ServiceLocationAdded?.Invoke(this, new ServiceLocationEventArgs(location));
        }
        protected virtual void OnServiceDiscovered(ServiceLocation location)
        {
            //Logs.LogInfo($"Service discovered : [{location.ServiceCode}].");
            ServiceDiscovered?.Invoke(this, new ServiceLocationEventArgs(location));
        }
        protected virtual void OnServiceLost(ServiceLocation location)
        {
            //Logs.LogWarning($"Service lost : [{location.ServiceCode}].");
            ServiceLost?.Invoke(this, new ServiceLocationEventArgs(location));
        }
    }

    public class ServiceDiscoveryBinding : IModuleBinding, IAutoDiscovery
    {
        readonly ServiceDiscoveryModule _module;
        readonly ModuleConfig _config;

        readonly Dictionary<string, ServiceLocation> _localServices = new Dictionary<string, ServiceLocation>();

        internal ModuleConfig Config => _config;

        public ServiceDiscoveryBinding(ServiceDiscoveryModule module, ModuleConfig config)
        {
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }


        #region IModuleBinding
        public T GetServiceObject<T>() where T : class
        {
            return this as T;
        }

        public void Dispose()
        {
        }
        #endregion

        #region IServiceDiscovery

        public void AddServiceLocation(ServiceLocation location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            var code = location.ServiceCode;

            lock (_localServices)
            {
                if (_localServices.ContainsKey(code))
                {
                    throw new InvalidOperationException($"{code} is already registered.");
                }

                _localServices.Add(code, location);
            }

            _module.AddServiceDiscovery(location);
        }

        public IServiceResolveFuture RequestAutoDiscovery(ServiceCode serviceCode)
        {
            if (string.IsNullOrEmpty(serviceCode))
            {
                return EmptyServiceResolveFuture.Empty;
            }

            Logs.LogInfo($"Request auto discovery : {serviceCode}");

            ServiceLocation localService;
            lock (_localServices)
            {
                //本实例内发现服务，直接忽略向外传递请求
                localService = _localServices.GetValueOrDefault(serviceCode);
            }
            if (localService != null)
            {
                Logs.LogInfo($"Service discovered in local services : {localService}");

                //本实例解算，但需要保证添加服务要先于发起解算
                return new ServiceResolveResultFuture(localService);
            }

            //外部解算
            return _module.RequestAutoDiscovery(serviceCode);
        }

        public ServiceLocation[] GetLocalServiceLocations()
        {
            return _module.Services.GetLocalServices();
        }

        public ServiceLocation[] GetGlobalServiceLocations()
        {
            return _module.Services.GetGlobalServices();
        }

        public ServiceResolve[] GetServiceResolves()
        {
            return _module.Services.GetServiceResolves();
        }



        #endregion

    }
}
