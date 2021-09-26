// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    /// <summary>
    /// 支持动作队列
    /// </summary>
    public static class QueuedAction
    {
        /// <summary>
        /// 在队列中执行动作
        /// </summary>
        /// <param name="action"></param>
        public static void Do(Action action)
        {
            Suity.Environment._device.QueueAction(action);
        }
    }
}
