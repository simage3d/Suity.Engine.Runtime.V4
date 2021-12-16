// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class RexValueListener<T> : IRexListener<T>
    {
        readonly IRexValue<T> _value;

        Action<T> _callBack;

        public RexValueListener(IRexValue<T> value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));

            _value.AddListener(HandleCallBack);
        }

        public void Dispose()
        {
            _callBack = null;
            _value.RemoveListener(HandleCallBack);
        }
        public IRexHandle Push()
        {
            HandleCallBack(_value.Value);
            return this;
        }

        public IRexHandle Subscribe(Action<T> callBack)
        {
            _callBack += callBack;
            return this;
        }
        private void HandleCallBack(T value)
        {
            _callBack?.Invoke(value);
        }
    }
}
