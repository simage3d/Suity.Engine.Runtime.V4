// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using Suity.Synchonizing;

namespace Suity.Views
{
    public interface IViewObjectSetup : ISyncContext
    {
        /// <summary>
        /// 获取目标类型是否支持当前的编辑类型
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>如果目标类型匹配，返回true，否则返回false。</returns>
        bool IsTypeSupported(Type type);

        bool IsViewIdSupported(int viewId);

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="property"></param>
        void AddField(Type type, ViewProperty property);

        IEnumerable<object> GetObjects();
    }
}
