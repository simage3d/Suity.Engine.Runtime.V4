// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Preset
{
    public class SyncValueInfo
    {
        public Type BaseType { get; set; }
        public object Value { get; set; }
        public SyncFlag Flag { get; set; }

        public SyncValueInfo(Type baseType, object value, SyncFlag flag)
        {
            BaseType = baseType;
            Value = value;
            Flag = flag;
        }

        public override string ToString()
        {
            return string.Format("<{0}, {1}>", BaseType, Value);
        }
    }
}
