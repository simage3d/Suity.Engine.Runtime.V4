// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.VirtualDom
{
    public interface IRexProperty<T> : IRexTreeInstance<T>
    {
        T Value { get; set; }
        void SetValueQueued(T value);
        void SetValueDeep(T value);
    }
}
