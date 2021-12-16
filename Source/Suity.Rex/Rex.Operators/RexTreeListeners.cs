// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Rex.VirtualDom;

namespace Suity.Rex.Operators
{
    class RexTreeListener<T> : IRexListener<T>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        Action<T> _callBack;

        public RexTreeListener(RexTree model, RexPath path, string tag = null)
        {
            _model = model;
            _path = path;

            model.AddDataListener<T>(path, HandleCallBack, tag);
        }

        public void Dispose()
        {
            _callBack = null;
            _model.RemoveListener<T>(_path, HandleCallBack);
        }
        public IRexHandle Push()
        {
            HandleCallBack(_model.GetData<T>(_path));
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

    class RexTreeBeforeListener<T> : IRexListener<T>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        Action<T> _callBack;

        public RexTreeBeforeListener(RexTree model, RexPath path, string tag = null)
        {
            _model = model;
            _path = path;

            model.AddBeforeListener<T>(path, HandleCallBack, tag);
        }

        public void Dispose()
        {
            _callBack = null;
            _model.RemoveBeforeListener<T>(_path, HandleCallBack);
        }
        public IRexHandle Push()
        {
            HandleCallBack(_model.GetData<T>(_path));
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

    class RexTreeAfterListener<T> : IRexListener<T>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        Action<T> _callBack;

        public RexTreeAfterListener(RexTree model, RexPath path, string tag = null)
        {
            _model = model;
            _path = path;

            model.AddAfterListener<T>(path, HandleCallBack, tag);
        }

        public void Dispose()
        {
            _callBack = null;
            _model.UnsetAfterListener<T>(_path, HandleCallBack);
        }
        public IRexHandle Push()
        {
            HandleCallBack(_model.GetData<T>(_path));
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
