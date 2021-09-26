// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Synchonizing
{
    [Flags]
    public enum SyncFlag
    {
        None = 0x0,
        /// <summary>
        /// 值只读，始终不会传递新值。
        /// </summary>
        ReadOnly = 0x1,
        /// <summary>
        /// 值非空，若结果为空时会返回传递的值。
        /// </summary>
        NotNull = 0x2,
        /// <summary>
        /// 值通过引用传递，当克隆时此对象不会进行克隆。
        /// </summary>
        ByRef = 0x4,
        /// <summary>
        /// 影响其他值，当此值改变时会通知父级编辑器更新。
        /// </summary>
        AffectsOthers = 0x8,
        /// <summary>
        /// 影响父级值
        /// </summary>
        AffectsParent = 0x10,
        /// <summary>
        /// 属性模式(在序列化中使用)
        /// </summary>
        AttributeMode = 0x20,
        /// <summary>
        /// 指示目标是一个元素分支
        /// </summary>
        Element = 0x40,
        /// <summary>
        /// 在路径获取时隐藏此路径节点
        /// </summary>
        PathHidden = 0x80,
        /// <summary>
        /// 禁止序列化
        /// </summary>
        NoSerialize = 0x100,
    }
}
