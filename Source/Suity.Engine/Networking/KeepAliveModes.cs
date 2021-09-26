using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    /// <summary>
    /// 保持连接模式
    /// </summary>
    public enum KeepAliveModes
    {
        /// <summary>
        /// 一次性连接，不保持连接
        /// </summary>
        None,
        /// <summary>
        /// 需要网络数据保持连接
        /// </summary>
        KeepAlive,
        /// <summary>
        /// 长期连接
        /// </summary>
        LongTerm,
    }
}
