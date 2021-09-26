// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    /// <summary>
    /// 网络信息
    /// </summary>
    public interface INetworkInfo
    {
        /// <summary>
        /// 传输方法
        /// </summary>
        NetworkDeliveryMethods Method { get; }
        /// <summary>
        /// 频道
        /// </summary>
        int Channel { get; }
        /// <summary>
        /// 键
        /// </summary>
        string Key { get; }
        /// <summary>
        /// 信息对象
        /// </summary>
        object Body { get; }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetArgs(string name);
    }
}
