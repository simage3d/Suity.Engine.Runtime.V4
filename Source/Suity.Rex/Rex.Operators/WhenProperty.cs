// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Rex.VirtualDom;

namespace Suity.Rex.Operators
{
    class WhenProperty<T, TData> : RexListenerBase<T, T>
    {
        public WhenProperty(IRexListener<T> source, IRexProperty<TData> property, Predicate<TData> predicate)
            : base(source)
        {
            if (property == null)
            {
                throw new ArgumentNullException();
            }
            if (predicate == null)
            {
                throw new ArgumentNullException();
            }

            source.Subscribe(o =>
            {
                if (predicate(property.Value))
                {
                    HandleCallBack(o);
                }
            });
        }
    }
}
