// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.AutoDiscovery
{
    public class ServiceResolveResultFuture : IServiceResolveFuture
    {
        public ServiceLocation Location { get; }

        public ServiceResolveResultFuture(ServiceLocation location)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public void OnUpdate(Action<ServiceLocation> action)
        {
            action?.Invoke(Location);
        }

        public void Dispose()
        {
        }
    }
}
