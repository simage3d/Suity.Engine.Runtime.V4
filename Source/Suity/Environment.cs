// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace Suity
{
    /// <summary>
    /// 全局环境对象
    /// </summary>
    public static class Environment
    {
        internal static Device _device = DefaultDevice.Default;

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="device"></param>
        public static void InitializeDevice(Device device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (_device == device)
            {
                return;
            }

            if (!(_device is DefaultDevice))
            {
                throw new SecurityException();
            }


            _device = device;
        }

        /// <summary>
        /// 获取环境设置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfig(string key) => _device.GetEnvironmentConfig(key);

        /// <summary>
        /// 获取位置
        /// </summary>
        public static string Location => _device.Location;

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T : class
        {
            return _device.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetService(Type type) => _device.GetService(type);
    }
}
