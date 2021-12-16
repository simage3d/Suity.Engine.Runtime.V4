// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.VirtualDom
{
    abstract class RexNodeListener : IDisposable
    {
        public RexNodeListenerSet Node { get; private set; }
        public string Tag { get; set; }


        public RexNodeListener(RexNodeListenerSet node)
        {
            Node = node ?? throw new ArgumentNullException();
        }
        public abstract void Invoke(object data);
        public abstract Delegate GetKey();

        public virtual void Dispose()
        {
            Node.RemoveListener(this);
            Node = null;
        }
    }


    #region Data Listener
    class RexNodeDataListener<T> : RexNodeListener, IRexHandle
    {
        readonly Action<T> _callBack;

        public RexNodeDataListener(RexNodeListenerSet node, Action<T> callBack)
            : base(node)
        {
            _callBack = callBack;
        }

        public override void Invoke(object data)
        {
            if (data is T t)
            {
                _callBack(t);
            }
            else if (data == null)
            {
                if (typeof(T).IsClass)
                {
                    _callBack(default(T));
                }
            }
        }
        public override Delegate GetKey()
        {
            return _callBack;
        }

        public IRexHandle Push()
        {
            T data = Node._model.GetData<T>(Node._path);
            _callBack(data);
            return this;
        }
    }
    #endregion

    #region Action Listener
    class RexNodeActionListener : RexNodeListener
    {
        readonly Action _callBack;

        public RexNodeActionListener(RexNodeListenerSet node, Action callBack) : base(node)
        {
            _callBack = callBack;
        }


        public override void Invoke(object data)
        {
            if (data is ActionArgument)
            {
                _callBack();
            }
        }
        public override Delegate GetKey()
        {
            return _callBack;
        }
    }
    class RexNodeActionListener<T> : RexNodeListener
    {
        readonly Action<T> _callBack;

        public RexNodeActionListener(RexNodeListenerSet node, Action<T> callBack) : base(node)
        {
            _callBack = callBack;
        }

        public override void Invoke(object data)
        {
            if (data is ActionArgument<T> arg)
            {
                _callBack(arg.Arg1);
            }
        }
        public override Delegate GetKey()
        {
            return _callBack;
        }
    }
    class RexNodeActionListener<T1, T2> : RexNodeListener
    {
        readonly Action<T1, T2> _callBack;

        public RexNodeActionListener(RexNodeListenerSet node, Action<T1, T2> callBack) : base(node)
        {
            _callBack = callBack;
        }

        public override void Invoke(object data)
        {
            if (data is ActionArgument<T1, T2> arg)
            {
                _callBack(arg.Arg1, arg.Arg2);
            }
        }
        public override Delegate GetKey()
        {
            return _callBack;
        }
    }
    class RexNodeActionListener<T1, T2, T3> : RexNodeListener
    {
        readonly Action<T1, T2, T3> _callBack;

        public RexNodeActionListener(RexNodeListenerSet node, Action<T1, T2, T3> callBack) : base(node)
        {
            _callBack = callBack;
        }

        public override void Invoke(object data)
        {
            if (data is ActionArgument<T1, T2, T3> actionArgument)
            {
                var arg = actionArgument;
                _callBack(arg.Arg1, arg.Arg2, arg.Arg3);
            }
        }
        public override Delegate GetKey()
        {
            return _callBack;
        }
    }
    class RexNodeActionListener<T1, T2, T3, T4> : RexNodeListener
    {
        readonly Action<T1, T2, T3, T4> _callBack;

        public RexNodeActionListener(RexNodeListenerSet node, Action<T1, T2, T3, T4> callBack) : base(node)
        {
            _callBack = callBack;
        }

        public override void Invoke(object data)
        {
            if (data is ActionArgument<T1, T2, T3, T4> actionArgument)
            {
                var arg = actionArgument;
                _callBack(arg.Arg1, arg.Arg2, arg.Arg3, arg.Arg4);
            }
        }
        public override Delegate GetKey()
        {
            return _callBack;
        }
    }
    #endregion

    #region Mapping Listener

    class RexNodeMappingListener : RexNodeListener
    {
        readonly Action<object> _action;
        readonly RexPath _pathTo;

        public RexNodeMappingListener(RexNodeListenerSet node, RexPath pathTo) : base(node)
        {
            _pathTo = pathTo ?? throw new ArgumentNullException();
            _action = new Action<object>(Invoke);
        }

        public RexPath PathTo { get { return _pathTo; } }

        public override Delegate GetKey()
        {
            return _action;
        }
        public override void Invoke(object data)
        {
            Node._model.UpdateData(_pathTo);
        }
    }

    #endregion
}
