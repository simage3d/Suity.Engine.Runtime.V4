// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Suity.Collections;

namespace Suity.Rex
{
    public struct PathItem
    {
        public static readonly PathItem Empty = new PathItem(string.Empty, -1);
        public static readonly PathItem WildCard = new PathItem("*", -1);

        public readonly string Key;
        public readonly int Index;

        internal PathItem(string key, int index)
        {
            Key = key;
            Index = index;
        }
        public override string ToString()
        {
            return Key ?? "[" + Index + "]";
        }
    }

    public class RexPath
    {
        public static readonly RexPath Empty = new RexPath();


        readonly string _raw;
        readonly PathItem[] _items;

        private RexPath()
        {
            _raw = string.Empty;
            _items = EmptyArray<PathItem>.Empty;
        }
        private RexPath(string raw, PathItem[] items)
        {
            _raw = raw;
            _items = items;
        }
        public RexPath(PathItem item)
        {
            _raw = item.ToString();
            _items = new PathItem[] { item };
        }
        public RexPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                _raw = string.Empty;
                _items = EmptyArray<PathItem>.Empty;
                return;
            }

            _raw = path;

            string[] strs = path.Split('.');
            _items = new PathItem[strs.Length];

            for (int i = 0; i < strs.Length; i++)
            {
                string str = strs[i];

                if (str.StartsWith("[") && str.EndsWith("]"))
                {
                    string intStr = str.Substring(1, str.Length - 2);
                    if (int.TryParse(intStr, out int value) && value >= 0)
                    {
                        _items[i] = new PathItem(null, value);
                    }
                    else
                    {
                        _items[i] = new PathItem(str, -1);
                    }
                }
                else
                {
                    _items[i] = new PathItem(str, -1);
                }
            }

        }


        public int Count { get { return _items.Length; } }
        public PathItem this[int index]
        {
            get { return _items[index]; }
        }
        public IEnumerable<PathItem> Items { get { return _items.Select(o => o); } }


        public PathItem First => _items.Length > 0 ? _items[0] : PathItem.Empty;
        public PathItem Last => _items.Length > 0 ? _items[_items.Length - 1] : PathItem.Empty;


        public override string ToString()
        {
            return _raw;
        }
        public override bool Equals(object obj)
        {
            RexPath temp = obj as RexPath;
            if (temp == null)
            {
                return false;
            }

            return _raw == temp._raw;
        }
        public override int GetHashCode()
        {
            return _raw.GetHashCode();
        }

        public RexPath Append(PathItem item)
        {
            PathItem[] items = new PathItem[_items.Length + 1];
            _items.CopyTo(items, 0);
            items[items.Length - 1] = item;
            string raw = string.IsNullOrEmpty(_raw) ? item.ToString() : _raw + "." + item.ToString();
            return new RexPath(raw, items);
        }
        public RexPath Append(string key)
        {
            return Append(new RexPath(key));
        }
        public RexPath Append(int index)
        {
            return Append(new PathItem(null, index));
        }
        public RexPath Append(RexPath path)
        {
            PathItem[] items = new PathItem[_items.Length + path._items.Length];
            _items.CopyTo(items, 0);
            for (int i = 0; i < path._items.Length; i++)
            {
                items[_items.Length + i] = path._items[i];
            }
            string raw = string.IsNullOrEmpty(_raw) ? path._raw : _raw + "." + path._raw;
            return new RexPath(raw, items);
        }
        public RexPath Prepend(PathItem item)
        {
            PathItem[] items = new PathItem[_items.Length + 1];
            _items.CopyTo(items, 1);
            items[0] = item;
            string raw = string.IsNullOrEmpty(_raw) ? item.ToString() : item.ToString() + "." + _raw;
            return new RexPath(raw, items);
        }
        public RexPath Prepend(string key)
        {
            return Prepend(new RexPath(key));
        }
        public RexPath Prepend(int index)
        {
            return Prepend(new PathItem(null, index));
        }
        public RexPath Prepend(RexPath path)
        {
            return path.Append(this);
        }

        public static bool operator ==(RexPath rec1, RexPath rec2)
        {
            return Equals(rec1, rec2);
        }
        public static bool operator !=(RexPath rec1, RexPath rec2)
        {
            return !Equals(rec1, rec2);
        }

        public static implicit operator RexPath(string path)
        {
            return new RexPath(path);
        }
        public static implicit operator RexPath(int index)
        {
            return new RexPath(new PathItem(null, index));
        }
        public static explicit operator string(RexPath path)
        {
            return path.ToString();
        }

        public static implicit operator RexPath(PathItem item)
        {
            return new RexPath(item);
        }


        public static RexPath operator +(RexPath s, PathItem item)
        {
            if (s == null)
            {
                s = RexPath.Empty;
            }
            return s.Append(item);
        }
        public static RexPath operator +(RexPath s, string str)
        {
            if (s == null)
            {
                s = RexPath.Empty;
            }
            return s.Append(str);
        }
        public static RexPath operator +(RexPath s, int index)
        {
            if (s == null)
            {
                s = RexPath.Empty;
            }
            return s.Append(index);
        }
    }

    public class RexPathEventArgs : EventArgs
    {
        public RexPath Path { get; }

        public RexPathEventArgs(RexPath path)
        {
            Path = path;
        }
    }

    public class RexPathValueEventArgs : RexPathEventArgs
    {
        public object Value { get; }

        public RexPathValueEventArgs(RexPath path, object value)
            : base(path)
        {
            Value = value;
        }
    }
}
