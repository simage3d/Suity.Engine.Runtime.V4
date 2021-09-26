// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Synchonizing
{
    /// <summary>
    /// 同步模式
    /// </summary>
    public enum SyncMode
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Initialize,

        /// <summary>
        /// 获取元素类型
        /// </summary>
        RequestElementType,

        /// <summary>
        /// 获取
        /// </summary>
        Get,
        /// <summary>
        /// 设置
        /// </summary>
        Set,
        /// <summary>
        /// 获取全部
        /// </summary>
        GetAll,
        /// <summary>
        /// 设置全部
        /// </summary>
        SetAll,

        /// <summary>
        /// 列表插入
        /// </summary>
        Insert,
        /// <summary>
        /// 列表移除
        /// </summary>
        RemoveAt,
        /// <summary>
        /// 列表创建新项
        /// </summary>
        CreateNew,
    }
}
