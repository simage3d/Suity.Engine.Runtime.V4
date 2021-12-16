// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.VirtualDom
{
    public sealed class RexAction : IRexTreeInstance<ActionArgument>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexAction(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }
        public RexAction(RexTree model, RexActionDefine define)
        {
            _model = model;
            _path = define.Path;
        }

        public RexTree Tree { get { return _model; } }
        public RexPath Path { get { return _path; } }

        public void Invoke()
        {
            _model.DoAction(_path);
        }
        public void InvokeQueued()
        {
            _model.DoActionQueued(_path);
        }

        public IDisposable AddActionListener(Action action, string tag = null)
        {
            return _model.AddActionListener(_path, action, tag);
        }
        public IDisposable AddQueuedActionListener(Action action, string tag = null)
        {
            return _model.AddActionListener(_path, () => QueuedAction.Do(action), tag);
        }
        public bool RemoveListener(Action action)
        {
            return _model.RemoveListener(_path, action);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }

    public sealed class RexAction<T> : IRexTreeInstance<ActionArgument<T>>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexAction(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }
        public RexAction(RexTree model, RexActionDefine<T> define)
        {
            _model = model;
            _path = define.Path;
        }

        public RexTree Tree { get { return _model; } }
        public RexPath Path { get { return _path; } }

        public void Invoke(T argument)
        {
            _model.DoAction<T>(_path, argument);
        }
        public void InvokeQueued(T argument)
        {
            _model.DoActionQueued<T>(_path, argument);
        }

        public IDisposable AddActionListener(Action<T> action, string tag = null)
        {
            return _model.AddActionListener(_path, action, tag);
        }
        public IDisposable AddQueuedActionListener(Action<T> action, string tag = null)
        {
            return _model.AddActionListener<T>(_path, v => QueuedAction.Do(() => action(v)), tag);
        }
        public bool RemoveListener(Action<T> action)
        {
            return _model.RemoveListener(_path, action);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }

    public sealed class RexAction<T1, T2> : IRexTreeInstance<ActionArgument<T1, T2>>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexAction(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }
        public RexAction(RexTree model, RexActionDefine<T1, T2> define)
        {
            _model = model;
            _path = define.Path;
        }

        public RexTree Tree { get { return _model; } }
        public RexPath Path { get { return _path; } }

        public void Invoke(T1 arg1, T2 arg2)
        {
            _model.DoAction(_path, arg1, arg2);
        }
        public void InvokeQueued(T1 arg1, T2 arg2)
        {
            _model.DoActionQueued(_path, arg1, arg2);
        }

        public IDisposable AddActionListener(Action<T1, T2> action, string tag = null)
        {
            return _model.AddActionListener(_path, action, tag);
        }
        public IDisposable AddQueuedActionListener(Action<T1, T2> action, string tag = null)
        {
            return _model.AddActionListener<T1, T2>(_path, (v1, v2) => QueuedAction.Do(() => action(v1, v2)), tag);
        }
        public bool RemoveListener(Action<T1, T2> action)
        {
            return _model.RemoveListener(_path, action);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }

    public sealed class RexAction<T1, T2, T3> : IRexTreeInstance<ActionArgument<T1, T2, T3>>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexAction(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }
        public RexAction(RexTree model, RexActionDefine<T1, T2, T3> define)
        {
            _model = model;
            _path = define.Path;
        }

        public RexTree Tree { get { return _model; } }
        public RexPath Path { get { return _path; } }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            _model.DoAction(_path, arg1, arg2, arg3);
        }
        public void InvokeQueued(T1 arg1, T2 arg2, T3 arg3)
        {
            _model.DoActionQueued(_path, arg1, arg2, arg3);
        }

        public IDisposable AddActionListener(Action<T1, T2, T3> action, string tag = null)
        {
            return _model.AddActionListener(_path, action, tag);
        }
        public IDisposable AddQueuedActionListener(Action<T1, T2, T3> action, string tag = null)
        {
            return _model.AddActionListener<T1, T2, T3>(_path, (v1, v2, v3) => QueuedAction.Do(() => action(v1, v2, v3)), tag);
        }
        public bool RemoveListener(Action<T1, T2, T3> action)
        {
            return _model.RemoveListener(_path, action);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }

    public sealed class RexAction<T1, T2, T3, T4> : IRexTreeInstance<ActionArgument<T1, T2, T3, T4>>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexAction(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }
        public RexAction(RexTree model, RexActionDefine<T1, T2, T3, T4> define)
        {
            _model = model;
            _path = define.Path;
        }

        public RexTree Tree { get { return _model; } }
        public RexPath Path { get { return _path; } }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _model.DoAction(_path, arg1, arg2, arg3, arg4);
        }
        public void InvokeQueued(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _model.DoActionQueued(_path, arg1, arg2, arg3, arg4);
        }

        public IDisposable AddActionListener(Action<T1, T2, T3, T4> action, string tag = null)
        {
            return _model.AddActionListener(_path, action, tag);
        }
        public IDisposable AddQueuedActionListener(Action<T1, T2, T3, T4> action, string tag = null)
        {
            return _model.AddActionListener<T1, T2, T3, T4>(_path, (v1, v2, v3, v4) => QueuedAction.Do(() => action(v1, v2, v3, v4)), tag);
        }
        public bool RemoveListener(Action<T1, T2, T3, T4> action)
        {
            return _model.RemoveListener(_path, action);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }
}
