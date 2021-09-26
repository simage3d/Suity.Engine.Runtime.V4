// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    /// <summary>
    /// 本地时间
    /// </summary>
    public static class LocalTime
    {
        public static float Time => Suity.Environment._device.Time;
    }
}
