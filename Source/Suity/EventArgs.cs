// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public class EventArgs<T> : EventArgs
    {
        public T Value { get; private set; }
        public EventArgs(T value)
        {
            Value = value;
        }
    }

    public delegate void EventArgsHandler<T>(object sender, EventArgs<T> args);

    public class RuntimeLogEventArgs : EventArgs
    {
        public LogMessageType Type { get; }
        public object Message { get; }

        public RuntimeLogEventArgs(LogMessageType type, object message)
        {
            Type = type;
            Message = message;
        }
    }

}
