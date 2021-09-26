// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Collections;
using Suity.Helpers;

namespace Suity.AutoDiscovery
{
    public class LocalServiceDiscoveryCollection
    {
        //本地可以注册多个服务
        readonly UniqueMultiDictionary<string, ServiceLocation> _localServices = new UniqueMultiDictionary<string, ServiceLocation>();
        //远程只接受单个
        readonly Dictionary<string, ServiceLocation> _globalServices = new Dictionary<string, ServiceLocation>();
        readonly Dictionary<string, ServiceLocation> _activeServices = new Dictionary<string, ServiceLocation>();

        readonly UniqueMultiDictionary<string, ServiceResolveFuture> _clients = new UniqueMultiDictionary<string, ServiceResolveFuture>();


        public event EventHandler<ServiceLocationEventArgs> ServiceDiscovered;
        public event EventHandler<ServiceLocationEventArgs> ServiceLost;

        private readonly object _sync = new object();

        public LocalServiceDiscoveryCollection()
        {
        }

        public bool AddLocalService(ServiceLocation location)
        {
            if (location == null)
            {
                return false;
            }

            bool added;
            lock (_sync)
            {
                added = _localServices.Add(location.ServiceCode, location);
            }
            if (added)
            {
                ResolveServiceAdded(location);
            }

            return added;
        }
        public bool RemoveLocalService(ServiceLocation location)
        {
            if (location == null)
            {
                return false;
            }

            bool removed;
            lock (_sync)
            {
                removed = _localServices.Remove(location.ServiceCode, location);
            }
            if (removed)
            {
                ResolveServiceLost(location);
            }

            return removed;
        }
        public bool AddGlobalService(ServiceLocation location)
        {
            if (location == null)
            {
                return false;
            }

            bool added = false;
            lock (_sync)
            {
                if (_globalServices.GetValueOrDefault(location.ServiceCode) != location)
                {
                    _globalServices[location.ServiceCode] = location;
                    added = true;
                }
            }
            if (added)
            {
                ResolveServiceAdded(location);
            }

            return added;
        }
        public bool RemoveGlobalService(ServiceLocation location)
        {
            if (location == null)
            {
                return false;
            }

            bool removed = false;
            lock (_sync)
            {
                if (_globalServices.GetValueOrDefault(location.ServiceCode) == location)
                {
                    _globalServices.Remove(location.ServiceCode);
                    removed = true;
                }
            }
            if (removed)
            {
                ResolveServiceLost(location);
            }

            return removed;
        }

        public IServiceResolveFuture AddAutoDiscovery(ServiceCode serviceCode)
        {
            if (serviceCode == null)
            {
                return EmptyServiceResolveFuture.Empty;
            }

            ServiceResolveFuture future = new ServiceResolveFuture(this, serviceCode);
            ServiceLocation location = null;

            lock (_sync)
            {
                _clients.Add(serviceCode, future);
                location = _activeServices.GetValueOrDefault(serviceCode);
            }

            if (location != null)
            {
                future.Location = location;
            }

            return future;
        }


        private void RemoveFuture(ServiceResolveFuture future)
        {
            if (future == null)
            {
                return;
            }

            lock (_sync)
            {
                _clients.Remove(future.ServiceCode, future);
            }
        }


        public ServiceLocation[] GetLocalServices()
        {
            lock (_sync)
            {
                return _localServices.Values.ToArray();
            }
        }
        public ServiceLocation[] GetGlobalServices()
        {
            lock (_sync)
            {
                return _globalServices.Values.ToArray();
            }
        }
        public ServiceResolve[] GetServiceResolves()
        {
            lock (_sync)
            {
                return _clients.Values.Select(o => new ServiceResolve(o.ServiceCode, o.Location)).ToArray();
            }
        }


        private void ResolveServiceAdded(ServiceLocation location)
        {
            if (location == null)
            {
                return;
            }

            var code = location.ServiceCode;
            ServiceResolveFuture[] clients = null;

            lock (_sync)
            {
                if (!_activeServices.ContainsKey(code))
                {
                    _activeServices.Add(code, location);
                    clients = _clients[code].ToArray();
                }
            }

            if (location != null)
            {
                Logs.LogInfo($"Service discovered : [{location}]");
            }

            if (clients?.Length > 0)
            {
                clients.Foreach(o =>
                {
                    try
                    {
                        o.Location = location;
                    }
                    catch (Exception err)
                    {
                        Logs.LogError(err);
                    }
                });
                ServiceDiscovered?.Invoke(this, new ServiceLocationEventArgs(location));
            }
        }
        private void ResolveServiceLost(ServiceLocation location)
        {
            if (location == null)
            {
                return;
            }

            var code = location.ServiceCode;
            ServiceResolveFuture[] clients = null;
            ServiceLocation newLocation = null;

            ServiceLost?.Invoke(this, new ServiceLocationEventArgs(location));

            lock (_sync)
            {
                ServiceLocation active = _activeServices.GetValueOrDefault(code);
                if (active == location)
                {
                    newLocation = _localServices[code].FirstOrDefault() ?? _globalServices.GetValueOrDefault(code);
                    if (newLocation != null)
                    {
                        _activeServices[code] = newLocation;
                    }
                    else
                    {
                        _activeServices.Remove(code);
                    }
                    clients = _clients[code].ToArray();
                }
            }

            if (newLocation != null)
            {
                Logs.LogInfo($"Service discovered : [{newLocation}]");
            }
            else
            {
                Logs.LogWarning($"Service lost : [{location}]");
            }

            if (clients?.Length > 0)
            {
                clients.Foreach(o => 
                {
                    try
                    {
                        o.Location = newLocation;
                    }
                    catch (Exception err)
                    {
                        Logs.LogError(err);
                    }
                });
                if (newLocation != null)
                {
                    ServiceDiscovered?.Invoke(this, new ServiceLocationEventArgs(newLocation));
                }
            }
        }


        #region ServiceResolveFuture class
        class ServiceResolveFuture : IServiceResolveFuture
        {
            readonly LocalServiceDiscoveryCollection _collection;
            Action<ServiceLocation> _action;
            ServiceLocation _location;


            public ServiceCode ServiceCode { get; }
            public ServiceLocation Location
            {
                get { return _location; }
                set
                {
                    _location = value;
                    _action?.Invoke(value);
                }
            }

            public ServiceResolveFuture(LocalServiceDiscoveryCollection collection, ServiceCode serviceCode)
            {
                _collection = collection ?? throw new ArgumentNullException(nameof(collection));
                ServiceCode = serviceCode ?? throw new ArgumentNullException(nameof(serviceCode));
            }

            public void OnUpdate(Action<ServiceLocation> action)
            {
                _action = action;

                var location = _location;
                if (location != null)
                {
                    action?.Invoke(location);
                }
            }

            public void Dispose()
            {
                _collection.RemoveFuture(this);
            }
        }
        #endregion
    }

}
