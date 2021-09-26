// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using System;
using System.Collections.Generic;

namespace Suity
{
    /// <summary>
    /// 函数调用上下文
    /// </summary>
    public class FunctionContext
    {
        internal Dictionary<string, object> _arguments;
        internal FunctionContext _inner;
        internal object _value;

        public FunctionContext()
        {
        }
        public FunctionContext(string eventKey)
        {
            EventKey = eventKey;
        }
        public FunctionContext(string eventKey, object value)
        {
            EventKey = eventKey;
            _value = value;
        }
        public FunctionContext(FunctionContext inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
        public FunctionContext(FunctionContext inner, string eventKey)
            : this(inner)
        {
            EventKey = eventKey;
        }
        public FunctionContext(FunctionContext inner, string eventKey, object value)
            : this(inner)
        {
            EventKey = eventKey;
            _value = value;
        }

        public string EventKey { get; internal set; }
        public object Value => _value;


        public void SetArgument(string id, object argument)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (_arguments == null)
            {
                _arguments = new Dictionary<string, object>();
            }

            if (argument != null)
            {
                _arguments[id] = argument;
            }
            else
            {
                _arguments.Remove(id);
            }
        }
        public object GetArgument(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return _arguments?.GetValueOrDefault(id) ?? _inner?.GetArgument(id);
        }


        public override string ToString()
        {
            if (!string.IsNullOrEmpty(EventKey))
            {
                return "FunctionContext:" + EventKey;
            }
            else
            {
                return "FunctionContext";
            }
        }
    }

    /// <summary>
    /// 函数调用上下文对象池
    /// </summary>
    public class FunctionContextPool
    {
        readonly Stack<FunctionContext> _pool = new Stack<FunctionContext>();

        public FunctionContext Get()
        {
            if (_pool.Count > 0)
            {
                var ctx = _pool.Pop();
                return ctx;
            }

            return new FunctionContext();
        }
        public FunctionContext Get(string eventKey)
        {
            if (_pool.Count > 0)
            {
                var ctx = _pool.Pop();
                ctx.EventKey = eventKey;
                return ctx;
            }

            return new FunctionContext(eventKey);
        }
        public FunctionContext Get(string eventKey, object value)
        {
            if (_pool.Count > 0)
            {
                var ctx = _pool.Pop();
                ctx.EventKey = eventKey;
                ctx._value = value;
                return ctx;
            }

            return new FunctionContext(eventKey, value);
        }
        public FunctionContext Get(FunctionContext inner)
        {
            if (_pool.Count > 0)
            {
                var ctx = _pool.Pop();
                ctx._inner = inner;
                return ctx;
            }

            return new FunctionContext(inner);
        }
        public FunctionContext Get(FunctionContext inner, string eventKey)
        {
            if (_pool.Count > 0)
            {
                var ctx = _pool.Pop();
                ctx._inner = inner;
                ctx.EventKey = eventKey;
                return ctx;
            }

            return new FunctionContext(inner, eventKey);
        }
        public FunctionContext Get(FunctionContext inner, string eventKey, object value)
        {
            if (_pool.Count > 0)
            {
                var ctx = _pool.Pop();
                ctx._inner = inner;
                ctx.EventKey = eventKey;
                ctx._value = value;
                return ctx;
            }

            return new FunctionContext(inner, eventKey, value);
        }

        public void Recycle(FunctionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context._inner = null;
            context.EventKey = null;
            context._value = null;
            context._arguments?.Clear();

            _pool.Push(context);
        }
    }
}
