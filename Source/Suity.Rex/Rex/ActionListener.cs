// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex
{
    class ActionListener<T> : IRexListener<T>
    {
        internal Action<T> _callBack;

        public ActionListener()
        {
        }

        public IRexHandle Subscribe(Action<T> callBack)
        {
            _callBack += callBack;
            return this;
        }

        internal void HandleCallBack(T result)
        {
            _callBack?.Invoke(result);
        }

        public void Dispose()
        {
            _callBack = null;
        }

        public IRexHandle Push()
        {
            return this;
        }
    }
}
