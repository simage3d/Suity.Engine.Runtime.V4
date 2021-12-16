// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex
{
    #region IRexEvent
    public interface IRexEvent
    {
        void AddListener(Action action);
        void RemoveListener(Action action);
    }
    public interface IRexEvent<T>
    {
        void AddListener(Action<T> action);
        void RemoveListener(Action<T> action);
    }
    public interface IRexEvent<T1, T2>
    {
        void AddListener(Action<T1, T2> action);
        void RemoveListener(Action<T1, T2> action);
    }
    public interface IRexEvent<T1, T2, T3>
    {
        void AddListener(Action<T1, T2, T3> action);
        void RemoveListener(Action<T1, T2, T3> action);
    }
    public interface IRexEvent<T1, T2, T3, T4>
    {
        void AddListener(Action<T1, T2, T3, T4> action);
        void RemoveListener(Action<T1, T2, T3, T4> action);
    }
    #endregion

    #region RexEventHandle
    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexEventHandle : IRexEvent
    {
        Action _action;

        public void AddListener(Action action)
        {
            _action += action;
        }
        public void RemoveListener(Action action)
        {
            _action -= action;
        }
        public void Invoke()
        {
            _action?.Invoke();
        }
    }

    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexEventHandle<T> : IRexEvent<T>
    {
        Action<T> _action;

        public void AddListener(Action<T> action)
        {
            _action += action;
        }
        public void RemoveListener(Action<T> action)
        {
            _action -= action;
        }
        public void Invoke(T arg)
        {
            _action?.Invoke(arg);
        }
    }

    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexEventHandle<T1, T2> : IRexEvent<T1, T2>
    {
        Action<T1, T2> _action;

        public void AddListener(Action<T1, T2> action)
        {
            _action += action;
        }
        public void RemoveListener(Action<T1, T2> action)
        {
            _action -= action;
        }
        public void Invoke(T1 arg1, T2 arg2)
        {
            _action?.Invoke(arg1, arg2);
        }
    }

    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexEventHandle<T1, T2, T3> : IRexEvent<T1, T2, T3>
    {
        Action<T1, T2, T3> _action;

        public void AddListener(Action<T1, T2, T3> action)
        {
            _action += action;
        }
        public void RemoveListener(Action<T1, T2, T3> action)
        {
            _action -= action;
        }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            _action?.Invoke(arg1, arg2, arg3);
        }
    }

    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexEventHandle<T1, T2, T3, T4> : IRexEvent<T1, T2, T3, T4>
    {
        Action<T1, T2, T3, T4> _action;

        public void AddListener(Action<T1, T2, T3, T4> action)
        {
            _action += action;
        }
        public void RemoveListener(Action<T1, T2, T3, T4> action)
        {
            _action -= action;
        }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _action?.Invoke(arg1, arg2, arg3, arg4);
        }
    }
    #endregion

    #region RexEvent
    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexEvent : IRexEvent
    {
        readonly RexEventHandle _handle;

        public RexEvent(RexEventHandle handle)
        {
            _handle = handle ?? throw new ArgumentNullException();
        }

        public void AddListener(Action action)
        {
            _handle.AddListener(action);
        }
        public void RemoveListener(Action action)
        {
            _handle.RemoveListener(action);
        }
    }

    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexEvent<T> : IRexEvent<T>
    {
        readonly RexEventHandle<T> _handle;

        public RexEvent(RexEventHandle<T> handle)
        {
            _handle = handle ?? throw new ArgumentNullException();
        }

        public void AddListener(Action<T> action)
        {
            _handle.AddListener(action);
        }
        public void RemoveListener(Action<T> action)
        {
            _handle.RemoveListener(action);
        }
    }

    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexEvent<T1, T2> : IRexEvent<T1, T2>
    {
        readonly RexEventHandle<T1, T2> _handle;

        public RexEvent(RexEventHandle<T1, T2> handle)
        {
            _handle = handle ?? throw new ArgumentNullException();
        }

        public void AddListener(Action<T1, T2> action)
        {
            _handle.AddListener(action);
        }
        public void RemoveListener(Action<T1, T2> action)
        {
            _handle.RemoveListener(action);
        }
    }

    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexEvent<T1, T2, T3> : IRexEvent<T1, T2, T3>
    {
        readonly RexEventHandle<T1, T2, T3> _handle;

        public RexEvent(RexEventHandle<T1, T2, T3> handle)
        {
            _handle = handle ?? throw new ArgumentNullException();
        }

        public void AddListener(Action<T1, T2, T3> action)
        {
            _handle.AddListener(action);
        }
        public void RemoveListener(Action<T1, T2, T3> action)
        {
            _handle.RemoveListener(action);
        }
    }

    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexEvent<T1, T2, T3, T4> : IRexEvent<T1, T2, T3, T4>
    {
        readonly RexEventHandle<T1, T2, T3, T4> _handle;

        public RexEvent(RexEventHandle<T1, T2, T3, T4> handle)
        {
            _handle = handle ?? throw new ArgumentNullException();
        }

        public void AddListener(Action<T1, T2, T3, T4> action)
        {
            _handle.AddListener(action);
        }
        public void RemoveListener(Action<T1, T2, T3, T4> action)
        {
            _handle.RemoveListener(action);
        }
    }
    #endregion
}
