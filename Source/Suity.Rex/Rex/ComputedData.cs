// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex
{
    abstract class ComputedData
    {
        public abstract object GetData();
        public abstract void SetData(object data);
    }

    class ComputedData<T> : ComputedData
    {
        readonly Func<T> _getter;
        readonly Action<T> _setter;

        public ComputedData(Func<T> getter, Action<T> setter)
        {
            _getter = getter;
            _setter = setter;
        }

        public override object GetData()
        {
            return _getter != null ? (object)_getter() : null;
        }

        public override void SetData(object data)
        {
            if (_setter == null)
            {
                return;
            }

            if (data is T t)
            {
                _setter(t);
            }
            else if (data == null)
            {
                if (typeof(T).IsClass)
                {
                    _setter(default(T));
                }
            }
        }
    }
}
