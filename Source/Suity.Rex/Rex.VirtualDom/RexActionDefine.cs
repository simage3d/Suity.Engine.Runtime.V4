// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.VirtualDom
{
    public sealed class RexActionDefine : IRexTreeDefine<ActionArgument>
    {
        readonly RexPath _path;

        public RexPath Path { get { return _path; } }

        public RexActionDefine(RexPath path)
        {
            _path = path;
        }

        public void Invoke(RexTree model)
        {
            model.DoAction(_path);
        }
        public void InvokeQueued(RexTree model)
        {
            model.DoActionQueued(_path);
        }

        public RexAction MakeAction(RexTree model)
        {
            return new RexAction(model, _path);
        }

        public IDisposable AddActionListener(RexTree model, Action action, string tag = null)
        {
            return model.AddActionListener(_path, action, tag);
        }
        public IDisposable AddQueuedActionListener(RexTree model, Action action, string tag = null)
        {
            return model.AddActionListener(_path, () => QueuedAction.Do(action), tag);
        }
        public bool RemoveListener(RexTree model, Action action)
        {
            return model.RemoveListener(_path, action);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }

    public sealed class RexActionDefine<T> : IRexTreeDefine<ActionArgument<T>>
    {
        readonly RexPath _path;

        public RexPath Path { get { return _path; } }

        public RexActionDefine(RexPath path)
        {
            _path = path;
        }

        public void Invoke(RexTree model, T argument)
        {
            model.DoAction<T>(_path, argument);
        }
        public void InvokeQueued(RexTree model, T argument)
        {
            model.DoActionQueued<T>(_path, argument);
        }

        public RexAction<T> MakeAction(RexTree model)
        {
            return new RexAction<T>(model, _path);
        }

        public IDisposable AddActionListener(RexTree model, Action<T> action, string tag = null)
        {
            return model.AddActionListener(_path, action, tag);
        }
        public IDisposable AddQueuedActionListener(RexTree model, Action<T> action, string tag = null)
        {
            return model.AddActionListener<T>(_path, v => QueuedAction.Do(() => action(v)), tag);
        }
        public bool RemoveListener(RexTree model, Action<T> action)
        {
            return model.RemoveListener(_path, action);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }

    public sealed class RexActionDefine<T1, T2> : IRexTreeDefine<ActionArgument<T1, T2>>
    {
        readonly RexPath _path;

        public RexPath Path { get { return _path; } }

        public RexActionDefine(RexPath path)
        {
            _path = path;
        }

        public void Invoke(RexTree model, T1 arg1, T2 arg2)
        {
            model.DoAction(_path, arg1, arg2);
        }
        public void InvokeQueued(RexTree model, T1 arg1, T2 arg2)
        {
            model.DoActionQueued(_path, arg1, arg2);
        }

        public RexAction<T1, T2> MakeAction(RexTree model)
        {
            return new RexAction<T1, T2>(model, _path);
        }

        public IDisposable AddActionListener(RexTree model, Action<T1, T2> action, string tag = null)
        {
            return model.AddActionListener(_path, action, tag);
        }
        public IDisposable AddQueuedActionListener(RexTree model, Action<T1, T2> action, string tag = null)
        {
            return model.AddActionListener<T1, T2>(_path, (v1, v2) => QueuedAction.Do(() => action(v1, v2)), tag);
        }
        public bool RemoveListener(RexTree model, Action<T1, T2> action)
        {
            return model.RemoveListener(_path, action);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }

    public sealed class RexActionDefine<T1, T2, T3> : IRexTreeDefine<ActionArgument<T1, T2, T3>>
    {
        readonly RexPath _path;

        public RexPath Path { get { return _path; } }

        public RexActionDefine(RexPath path)
        {
            _path = path;
        }

        public void Invoke(RexTree model, T1 arg1, T2 arg2, T3 arg3)
        {
            model.DoAction(_path, arg1, arg2, arg3);
        }
        public void InvokeQueued(RexTree model, T1 arg1, T2 arg2, T3 arg3)
        {
            model.DoActionQueued(_path, arg1, arg2, arg3);
        }

        public RexAction<T1, T2, T3> MakeAction(RexTree model)
        {
            return new RexAction<T1, T2, T3>(model, _path);
        }

        public IDisposable AddActionListener(RexTree model, Action<T1, T2, T3> action, string tag = null)
        {
            return model.AddActionListener(_path, action, tag);
        }
        public IDisposable AddQueuedActionListener(RexTree model, Action<T1, T2, T3> action, string tag = null)
        {
            return model.AddActionListener<T1, T2, T3>(_path, (v1, v2, v3) => QueuedAction.Do(() => action(v1, v2, v3)), tag);
        }
        public bool RemoveListener(RexTree model, Action<T1, T2, T3> action)
        {
            return model.RemoveListener(_path, action);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }

    public sealed class RexActionDefine<T1, T2, T3, T4> : IRexTreeDefine<ActionArgument<T1, T2, T3, T4>>
    {
        readonly RexPath _path;

        public RexPath Path { get { return _path; } }

        public RexActionDefine(RexPath path)
        {
            _path = path;
        }

        public void Invoke(RexTree model, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            model.DoAction(_path, arg1, arg2, arg3, arg4);
        }
        public void InvokeQueued(RexTree model, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            model.DoActionQueued(_path, arg1, arg2, arg3, arg4);
        }

        public RexAction<T1, T2, T3, T4> MakeAction(RexTree model)
        {
            return new RexAction<T1, T2, T3, T4>(model, _path);
        }

        public IDisposable AddActionListener(RexTree model, Action<T1, T2, T3, T4> action, string tag = null)
        {
            return model.AddActionListener(_path, action, tag);
        }
        public IDisposable AddQueuedActionListener(RexTree model, Action<T1, T2, T3, T4> action, string tag = null)
        {
            return model.AddActionListener<T1, T2, T3, T4>(_path, (v1, v2, v3, v4) => QueuedAction.Do(() => action(v1, v2, v3, v4)), tag);
        }
        public bool RemoveListener(RexTree model, Action<T1, T2, T3, T4> action)
        {
            return model.RemoveListener(_path, action);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }
}
