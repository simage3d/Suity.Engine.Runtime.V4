// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex
{
    public interface IRexListener<T> : IRexHandle
    {
        IRexHandle Subscribe(Action<T> callBack);
    }

    public class EmptyRexListener<T> : IRexListener<T>
    {
        public static readonly EmptyRexListener<T> Empty = new EmptyRexListener<T>();

        public void Dispose()
        {
        }

        public IRexHandle Push()
        {
            return this;
        }

        public IRexHandle Subscribe(Action<T> callBack)
        {
            return this;
        }
    }
}
