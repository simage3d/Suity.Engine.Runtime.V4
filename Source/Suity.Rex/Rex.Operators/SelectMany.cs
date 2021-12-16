// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class SelectMany<TSource, TResult> : RexListenerBase<TSource, TResult>
    {
        public SelectMany(IRexListener<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
            : base(source)
        {
            if (selector == null)
            {
                throw new ArgumentNullException();
            }

            source.Subscribe(o => 
            {
                IEnumerable<TResult> result = selector(o);
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        HandleCallBack(item);
                    }
                }
            });
        }

    }
}
