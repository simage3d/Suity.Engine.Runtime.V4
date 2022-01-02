// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Collections
{
    [MultiThreadSecurity(MultiThreadSecurityMethods.ConcurrentSecure)]
    public class Pool<T> where T : new()
    {
        readonly ConcurrentStack<T> _pool = new ConcurrentStack<T>();

        public int? Capacity { get; set; }

        public Pool()
        {
        }
        public Pool(int capacity)
        {
            Capacity = capacity;
        }

        public T Get()
        {
            if (_pool.TryPop(out T value))
            {
                return value;
            }

            return new T();
        }
        public void Recycle(T value)
        {
            if (!Capacity.HasValue || Capacity.Value < _pool.Count)
            {
                _pool.Push(value);
            }
        }

        public int Count => _pool.Count();

        public void Clear() { _pool.Clear(); }
    }
}
