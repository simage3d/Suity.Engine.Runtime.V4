// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Synchonizing
{
    /// <summary>
    /// 同步意图
    /// </summary>
    public enum SyncIntent
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 数据访问
        /// </summary>
        Serialize,
        /// <summary>
        /// 视图互动访问
        /// </summary>
        View,
        /// <summary>
        /// 数据输出
        /// </summary>
        DataExport,
        /// <summary>
        /// 克隆
        /// </summary>
        Clone,
        /// <summary>
        /// 浏览
        /// </summary>
        Visit,
    }
}
