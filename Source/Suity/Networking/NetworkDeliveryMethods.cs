// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    /// <summary>
    /// 网络传输方法
    /// </summary>
    public enum NetworkDeliveryMethods
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,

        /// <summary>
        /// 不可靠，无顺序传输
        /// </summary>
        Unreliable,

        /// <summary>
        /// 不可靠传输，但会自动丢弃滞后的信息
        /// </summary>
        UnreliableSequenced,

        /// <summary>
        /// 可靠传输，但不保证顺序
        /// </summary>
        ReliableUnordered,

        /// <summary>
        /// 可靠传输，滞后信息可能丢失，但最新的一条信息保证送达
        /// </summary>
        ReliableSequenced,

        /// <summary>
        /// 可靠并保证顺序传输
        /// </summary>
        ReliableOrdered,
    }
}
