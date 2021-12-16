// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class Or : RexListenerBase<bool, bool, bool>
    {
        bool? _value1;
        bool? _value2;

        public Or(IRexListener<bool> source1, IRexListener<bool> source2) : base(source1, source2)
        {
            source1.Subscribe(v =>
            {
                _value1 = v;
                if (_value2.HasValue)
                {
                    HandleCallBack(v || _value2.Value);
                }
            });
            source2.Subscribe(v =>
            {
                _value2 = v;
                if (_value1.HasValue)
                {
                    HandleCallBack(_value1.Value || v);
                }
            });
        }
    }
}
