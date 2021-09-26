// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity
{
    /// <summary>
    /// 文本状态
    /// </summary>
    public enum TextStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 引用
        /// </summary>
        Reference,
        /// <summary>
        /// 文件引用
        /// </summary>
        FileReference,
        /// <summary>
        /// 枚举引用
        /// </summary>
        EnumReference,

        /// <summary>
        /// 添加
        /// </summary>
        Add,
        /// <summary>
        /// 移除
        /// </summary>
        Remove,
        /// <summary>
        /// 修改
        /// </summary>
        Modify,

        /// <summary>
        /// 禁用
        /// </summary>
        Disabled,
        /// <summary>
        /// 匿名
        /// </summary>
        Anonymous,
        /// <summary>
        /// 导入
        /// </summary>
        Import,
        /// <summary>
        /// 标签
        /// </summary>
        Tag,
        /// <summary>
        /// 用户代码
        /// </summary>
        UserCode,
        /// <summary>
        /// 使用资源
        /// </summary>
        ResourceUse,
        /// <summary>
        /// 注释
        /// </summary>
        Comment,

        /// <summary>
        /// 信息
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
    }
}
