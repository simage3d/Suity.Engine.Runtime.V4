// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class Select<TSource, TResult> : RexListenerBase<TSource, TResult>
    {
        public Select(IRexListener<TSource> source, Func<TSource, TResult> selector)
            : base(source)
        {
            if (selector == null)
            {
                throw new ArgumentNullException();
            }

            source.Subscribe(o => 
            {
                TResult result = selector(o);
                HandleCallBack(result);
            });
        }
    }
}
