// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Suity.Engine;

namespace Suity.AutoDiscovery
{
    public abstract class BaseAutoDiscoveryModuleProvider : BaseModuleProvider
    {
        readonly IServiceDiscoveryHost _serviceDiscoveryHost;

        public BaseAutoDiscoveryModuleProvider(IServiceDiscoveryHost serviceDiscoveryHost)
        {
            _serviceDiscoveryHost = serviceDiscoveryHost ?? throw new ArgumentNullException(nameof(serviceDiscoveryHost));
        }

        public void StartNodeObject(NodeObject obj)
        {
            //自动服务发现
            foreach (var component in obj.GetAllComponents())
            {
                if (component is IServiceLocationProvider serviceLocationProvider)
                {
                    var moduleProvider = Suity.Environment.GetService<IModuleProvider>();
                    ModuleConfig config = new ModuleConfig(component);
                    IAutoDiscovery serviceDiscovery = moduleProvider.Bind<IAutoDiscovery>(ModuleBindingNames.AutoDiscovery, config);
                    if (serviceDiscovery != null)
                    {
                        foreach (var location in serviceLocationProvider.ServiceLocations)
                        {
                            if (location != null)
                            {
                                //Logs.LogInfo($"Add service discovery : {location.ServiceCode}");
                                serviceDiscovery.AddServiceLocation(location);
                            }
                        }
                    }
                }
            }
        }

        protected override void OnBound(IModuleBinding binding, ModuleConfig config)
        {
            IServiceLocationProvider serviceProducer = binding.GetServiceObject<IServiceLocationProvider>();
            if (serviceProducer != null)
            {
                IAutoDiscovery serviceDiscovery = this.Bind<IAutoDiscovery>(ModuleBindingNames.AutoDiscovery, config);
                if (serviceDiscovery != null)
                {
                    foreach (var location in serviceProducer.ServiceLocations)
                    {
                        if (location != null)
                        {
                            //Logs.LogInfo($"Add service discovery : {location.ServiceCode}");
                            serviceDiscovery.AddServiceLocation(location);
                        }
                    }
                }
            }
        }
        protected override Module ResolveInternalDefaultModule(string moduleName)
        {
            switch (moduleName)
            {
                case ModuleBindingNames.AutoDiscovery:
                    return new ServiceDiscoveryModule(_serviceDiscoveryHost);
                default:
                    break;
            }

            return null;
        }

    }
}
