// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity
{
    /// <summary>
    /// 资源实现信息
    /// </summary>
    class AssetImplementInfo
    {
        /// <summary>
        /// 资源键
        /// </summary>
        public string AssetKey { get; }

        /// <summary>
        /// 实现的服务类型
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// 实现的服务实例
        /// </summary>
        public object ServiceInstance { get; private set; }

        public AssetImplementInfo(string assetKey, Type serviceType, object serviceInstance = null)
        {
            AssetKey = assetKey ?? throw new ArgumentNullException(nameof(assetKey));
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            ServiceInstance = serviceInstance;
        }

        public object GetInstance(bool autoCreate)
        {
            var instance = ServiceInstance;
            if (instance != null)
            {
                return instance;
            }

            if (ServiceInstance != null)
            {
                return ServiceInstance;
            }

            if (autoCreate)
            {
                ServiceInstance = Activator.CreateInstance(ServiceType);
                return ServiceInstance;
            }
            else
            {
                return null;
            }
        }
        public object CreateInstance()
        {
            return Activator.CreateInstance(ServiceType);
        }

        public override string ToString()
        {
            return ServiceType.Name;
        }
    }
}
