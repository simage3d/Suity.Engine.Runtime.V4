// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.AutoDiscovery
{
    public interface IServiceDiscoveryHost
    {
        //TODO: 服务单一性的两种原则：1是支持一个节点内可重复提供同一种服务，2是一种服务只能提供一次，2方案实行比较方便
        //TODO: 服务动态性的两种原则：1是支持动态增减服务，2是不支持
        //TODO: 单一服务 可以支持 动态服务，  静态服务 可以支持 重复服务。 重复服务 同时支持 动态服务 会非常复杂。
        //TODO: 目前的实现方案是： 重复服务，静态服务（只增不减）

        void AddServiceLocation(IServiceDiscoveryClient client, ServiceLocation location);
        void RemoveServiceLocation(IServiceDiscoveryClient client, ServiceLocation location);
        void RequestAutoDiscovery(IServiceDiscoveryClient client, ServiceCode serviceCode);
        void CancelAutoDiscovery(IServiceDiscoveryClient client, ServiceCode serviceCode);
    }
}
