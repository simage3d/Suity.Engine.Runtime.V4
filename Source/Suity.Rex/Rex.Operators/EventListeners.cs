// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class EventListener : IRexListener<object>, IDisposable
    {
        readonly IRexEvent _event;
        Action<object> _callBack;

        public EventListener(IRexEvent rexEvent)
        {
            _event = rexEvent ?? throw new ArgumentException();
            _event.AddListener(OnEvent);
        }
        public IRexHandle Subscribe(Action<object> callBack)
        {
            _callBack += callBack;
            return this;
        }
        public void Dispose()
        {
            _callBack = null;
            _event.RemoveListener(OnEvent);
        }
        public IRexHandle Push()
        {
            return this;
        }

        private void OnEvent()
        {
            _callBack?.Invoke(null);
        }
    }

    class EventListener<T> : IRexListener<T>, IDisposable
    {
        readonly IRexEvent<T> _event;
        Action<T> _callBack;

        public EventListener(IRexEvent<T> rexEvent)
        {
            _event = rexEvent ?? throw new ArgumentException();
            _event.AddListener(OnEvent);
        }
        public IRexHandle Subscribe(Action<T> callBack)
        {
            _callBack += callBack;
            return this;
        }
        public void Dispose()
        {
            _callBack = null;
            _event.RemoveListener(OnEvent);
        }
        public IRexHandle Push()
        {
            return this;
        }


        private void OnEvent(T value)
        {
            _callBack?.Invoke(value);
        }
    }
}
