// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Mapping
{
    public interface IRexHandler<T>
    {
        void Handle(T value);
    }

    public delegate void RexHandleDelegate<T>(T value);

    public class RexHandler<T> : IRexHandler<T>
    {
        readonly RexHandleDelegate<T> _handler;

        public RexHandler(RexHandleDelegate<T> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void Handle(T value)
        {
            _handler(value);
        }
    }
}
