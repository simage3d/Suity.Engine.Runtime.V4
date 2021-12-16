// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.VirtualDom
{
    public sealed class RexProperty<T> : IRexProperty<T>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexProperty(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
            var value = model.GetData<T>(_path);
            _model.SetData<T>(_path, value); // 强制记录初始化默认值
        }
        public RexProperty(RexTree model, RexPath path, T initValue)
        {
            _model = model;
            _path = path;
            _model.SetData<T>(_path, initValue);
        }
        public RexProperty(RexTree model, RexPropertyDefine<T> define)
            : this(model, define.Path)
        {
        }

        public RexTree Tree { get { return _model; } }
        public RexPath Path { get { return _path; } }
        public T Value
        {
            get
            {
                return _model.GetData<T>(_path);
            }
            set
            {
                _model.SetData<T>(_path, value);
            }
        }
        public void SetValueQueued(T value)
        {
            _model.SetDataQueued<T>(_path, value);
        }
        public IDisposable SetComputed(Func<T> getter = null, Action<T> setter = null)
        {
            return _model.SetComputedData(_path, getter, setter);
        }
        public void Update()
        {
            _model.UpdateData(_path);
        }
        public void UpdateQueued()
        {
            _model.UpdateDataQueued(_path);
        }
        public void SetValueDeep(T value)
        {
            _model.SetDataDeep<T>(_path, value);
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


        public static explicit operator T(RexProperty<T> prop)
        {
            return prop.Value;
        }

    }
}
