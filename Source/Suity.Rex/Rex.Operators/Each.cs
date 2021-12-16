// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class Each<T> : RexListenerBase<IEnumerable<T>, T>
    {
        public Each(IRexListener<IEnumerable<T>> source)
            : base(source)
        {
            source.Subscribe(o => 
            {
                if (o != null)
                {
                    foreach (var item in o)
                    {
                        HandleCallBack(item);
                    }
                }
            });
        }

    }
}
