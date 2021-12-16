// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex
{
    public interface IRexValue<T>
    {
        T Value { get; }
        void AddListener(Action<T> action);
        void RemoveListener(Action<T> action);
    }

    public sealed class RexValue<T> : IRexValue<T>
    {
        T _value;
        Action<T> _callBack;

        public RexValue()
        {
        }
        public RexValue(T value)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _callBack?.Invoke(_value);
            }
        }

        public void AddListener(Action<T> action)
        {
            _callBack += action;
        }
        public void RemoveListener(Action<T> action)
        {
            _callBack -= action;
        }
    }

    public sealed class RexReadonlyValue<T> : IRexValue<T>
    {
        IRexValue<T> _inner;
        public RexReadonlyValue(IRexValue<T> inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public T Value => _inner.Value;

        public void AddListener(Action<T> action)
        {
            _inner.AddListener(action);
        }

        public void RemoveListener(Action<T> action)
        {
            _inner.RemoveListener(action);
        }
    }
}
