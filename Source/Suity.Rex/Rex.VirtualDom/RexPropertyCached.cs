// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.VirtualDom
{
    public sealed class RexPropertyCached<T> : IRexProperty<T>, IDisposable
    {
        readonly RexTree _model;
        readonly RexPath _path;
        T _value;

        public RexPropertyCached(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
            _value = model.GetData<T>(_path);
            _model.SetData<T>(_path, _value); // 强制记录初始化默认值
            _model.AddDataListener<T>(_path, HandleCallBack);
        }
        public RexPropertyCached(RexTree model, RexPath path, T initValue)
        {
            _model = model;
            _path = path;
            _value = initValue;
            _model.SetData<T>(_path, initValue);
            _model.AddDataListener<T>(_path, HandleCallBack);
        }
        public RexPropertyCached(RexTree model, RexPropertyDefine<T> define)
            : this(model, define.Path)
        {
        }

        public RexTree Tree { get { return _model; } }
        public RexPath Path { get { return _path; } }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _model.SetData<T>(_path, value);
            }
        }
        public void SetValueQueued(T value)
        {
            _model.SetDataQueued<T>(_path, value);
        }
        public void SetValueDeep(T value)
        {
            _model.SetDataDeep<T>(_path, value);
        }

        public void Update()
        {
            _model.UpdateData(_path);
        }
        public void UpdateQueued()
        {
            _model.UpdateDataQueued(_path);
        }


        private void HandleCallBack(T value)
        {
            _value = value;
        }

        public void Dispose()
        {
            _model.RemoveListener<T>(_path, HandleCallBack);
        }

        public override string ToString()
        {
            T value = Value;
            if (value != null)
            {
                return _path.ToString() + " = " + value.ToString();
            }
            else
            {
                return _path.ToString() + " = null";
            }
        }

        public static explicit operator T(RexPropertyCached<T> prop)
        {
            return prop.Value;
        }
    }
}
