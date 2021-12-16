// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class Where<T> : RexListenerBase<T, T>
    {
        public Where(IRexListener<T> source, Predicate<T> predicate)
            : base(source)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException();
            }

            source.Subscribe(o => 
            {
                if (predicate(o))
                {
                    HandleCallBack(o);
                }
            });
        }
    }
}
