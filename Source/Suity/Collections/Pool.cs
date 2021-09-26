// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Collections
{
    public class Pool<T> where T : new()
    {
        readonly Stack<T> _pool = new Stack<T>();

        public T Get()
        {
            if (_pool.Count > 0)
            {
                return _pool.Pop();
            }
            else
            {
                return new T();
            }
        }
        public void Recycle(T value)
        {
            _pool.Push(value);
        }
    }
}
