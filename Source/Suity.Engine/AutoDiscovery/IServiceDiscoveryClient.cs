// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.AutoDiscovery
{
    public interface IServiceDiscoveryClient
    {
        ServiceLocation[] GetServiceLocations();

        void AddServiceDiscovery(ServiceLocation location);
        void RemoveServiceDiscovery(ServiceLocation location);
    }
}
