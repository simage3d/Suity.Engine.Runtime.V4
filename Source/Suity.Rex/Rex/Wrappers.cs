// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex
{
    class DisposableWrapper : IDisposable
    {
        IDisposable _disposable;
        Action _action;
        public DisposableWrapper(IDisposable disposable, Action action)
        {
            _disposable = disposable;
            _action = action;
        }

        public void Dispose()
        {
            var dispose = _disposable;
            var action = _action;
            _disposable = null;
            _action = null;

            action?.Invoke();
            _disposable?.Dispose();
        }
    }

    class RexHandleWrapper : IRexHandle
    {
        IRexHandle _handle;
        Action _disposeAction;
        public RexHandleWrapper(IRexHandle handle, Action disposeAction)
        {
            _handle = handle;
            _disposeAction = disposeAction;
        }

        public IRexHandle Push()
        {
            _handle?.Push();
            return this;
        }

        public void Dispose()
        {
            var dispose = _handle;
            var action = _disposeAction;
            _handle = null;
            _disposeAction = null;

            action?.Invoke();
            _handle?.Dispose();
        }
    }
}
