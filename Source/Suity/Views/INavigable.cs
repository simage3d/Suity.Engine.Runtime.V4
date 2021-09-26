// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Suity.Views
{
    /// <summary>
    /// 可导航对象
    /// </summary>
    public interface INavigable
    {
        /// <summary>
        /// 获取导航目标对象
        /// </summary>
        /// <returns></returns>
        object GetNavigationTarget();
    }
}
