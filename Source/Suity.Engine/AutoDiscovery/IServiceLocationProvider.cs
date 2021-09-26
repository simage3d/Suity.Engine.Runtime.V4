// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.AutoDiscovery
{
    /// <summary>
    /// 为服务发现提供被动获取服务的接口
    /// </summary>
    public interface IServiceLocationProvider
    {
        /// <summary>
        /// 本接口所提供的所有服务地址
        /// </summary>
        IEnumerable<ServiceLocation> ServiceLocations { get; }
    }
}
