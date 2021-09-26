// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Collections
{
    public class CappedArray<T> : IEnumerable<T>
    {
        readonly T[] _array;

        int _cursorIndex;

        public CappedArray(int length)
        {
            if (length < 1)
            {
                throw new ArgumentException("length < 1", nameof(length));
            }
            _array = new T[length];
        }

        /// <summary>
        /// 在数组前端增加一个值，此值将替换LastValue，并将光标前移
        /// </summary>
        /// <param name="value"></param>
        public void AddFirst(T value)
        {
            MoveCursorBackward();
            _array[_cursorIndex] = value;
        }

        /// <summary>
        /// 在输入后端增加一个值，此值将替换FirstValue，并将光标后移
        /// </summary>
        /// <param name="value"></param>
        public void AddLast(T value)
        {
            _array[_cursorIndex] = value;
            MoveCursorForward();
        }

        public void MoveCursorForward()
        {
            _cursorIndex++;
            if (_cursorIndex >= _array.Length)
            {
                _cursorIndex = 0;
            }
        }
        public void MoveCursorBackward()
        {
            _cursorIndex--;
            if (_cursorIndex < 0)
            {
                _cursorIndex = _array.Length - 1;
            }
        }


        public T FirstValue
        {
            get { return _array[_cursorIndex]; }
            set { _array[_cursorIndex] = value; }
        }
        public T LastValue
        {
            get { return _array[GetRealIndex(_array.Length - 1)]; }
            set { _array[GetRealIndex(_array.Length - 1)] = value; }
        }


        public T this[int index]
        {
            get
            {
                return _array[GetRealIndex(index)];
            }
            set
            {
                _array[GetRealIndex(index)] = value;
            }
        }

        public int Length => _array.Length;

        public IEnumerable<T> RawValues => _array;

        private int GetRealIndex(int index)
        {
            int i = index + _cursorIndex;
            if (i >= _array.Length)
            {
                i -= _array.Length;
            }
            return i;
        }

        public void Clear()
        {
            Array.Clear(_array, 0, _array.Length);
        }

        private IEnumerable<T> GetEnumerable()
        {
            for (int i = 0; i < _array.Length; i++)
            {
                yield return this[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }
    }
}
