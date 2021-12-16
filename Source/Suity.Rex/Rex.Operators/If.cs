// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class If<TResult> : RexListenerBase<bool, TResult>
    {
        public If(IRexListener<bool> source, TResult truePart, TResult falsePart)
            : base(source)
        {
            source.Subscribe(o => 
            {
                if (o)
                {
                    HandleCallBack(truePart);
                }
                else
                {
                    HandleCallBack(falsePart);
                }
            });
        }

    }
}
