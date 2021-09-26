// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Suity.Engine
{
    public class ArrayInstance<T> : IEnumerable<T>
    {
        readonly List<T> _list = new List<T>();


        public ArrayInstance()
        {
        }
        public ArrayInstance(params T[] values)
        {
            _list.AddRange(values);
        }

        public void Push(T value)
        {
            _list.Add(value);
        }

        public int Length { get { return _list.Count; } }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
