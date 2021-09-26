// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.AutoDiscovery
{
    public class ServiceResolve
    {
        public ServiceCode ServiceCode { get; }

        public ServiceLocation ResolvedLocation { get; set; }

        public ServiceResolve(ServiceCode serviceCode)
        {
            ServiceCode = serviceCode ?? throw new ArgumentNullException(nameof(serviceCode));
        }
        public ServiceResolve(ServiceCode serviceCode, ServiceLocation location)
            : this(serviceCode)
        {
            ResolvedLocation = location;
        }
    }
}
