// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    /// <summary>
    /// 多前程安全模式
    /// </summary>
    public enum MultiThreadSecurityMethods
    {
        /// <summary>
        /// 通过加锁实现线程安全
        /// </summary>
        LockedSecure,
        /// <summary>
        /// 通过并发实现线程安全
        /// </summary>
        ConcurrentSecure,
        /// <summary>
        /// 通过每线程缓存实现线程安全
        /// </summary>
        PerThreadSecure,
        /// <summary>
        /// 通过初始化写，之后全读取实现线程安全
        /// </summary>
        ReadonlySecure,
        /// <summary>
        /// 通过专用线程访问，其他线程访问会出错
        /// </summary>
        LimitedInOneThread,
        /// <summary>
        /// 线程不安全
        /// </summary>net
        Insecure,
    }

    /// <summary>
    /// 标记多线程安全模型
    /// </summary>
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class MultiThreadSecurityAttribute : Attribute
    {
        public MultiThreadSecurityAttribute(MultiThreadSecurityMethods medhod)
        {
            SecurityMethod = medhod;
        }

        /// <summary>
        /// 安全模式
        /// </summary>
        public MultiThreadSecurityMethods SecurityMethod { get; }
    }
}
