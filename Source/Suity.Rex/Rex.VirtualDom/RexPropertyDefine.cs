// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.VirtualDom
{
    public sealed class RexPropertyDefine<T> : IRexTreeDefine<T>
    {
        readonly RexPath _path;

        public RexPath Path { get { return _path; } }

        public RexPropertyDefine(RexPath path)
        {
            _path = path;
        }

        public T GetValue(RexTree model)
        {
            return model.GetData<T>(_path);
        }
        public void SetValue(RexTree model, T value)
        {
            model.SetData<T>(_path, value);
        }


        public RexProperty<T> MakeProperty(RexTree model)
        {
            return new RexProperty<T>(model, _path);
        }
        public RexPropertyCached<T> MakePropertyCached(RexTree model)
        {
            return new RexPropertyCached<T>(model, _path);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }
}
