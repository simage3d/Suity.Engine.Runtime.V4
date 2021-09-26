// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public class DisposeCollector : IDisposable
    {
        readonly List<object> _list = new List<object>();

        public void Dispose()
        {
            foreach (var obj in _list)
            {
                if (obj is Suity.Object o)
                {
                    try
                    {
                        Suity.Object.DestroyObject(o);
                    }
                    catch (Exception err)
                    {
                        Logs.LogError(err);
                    }
                }
                else if (obj is IDisposable disposable)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception err)
                    {
                        Logs.LogError(err);
                    }
                }
            }
            _list.Clear();
        }

        public static DisposeCollector operator +(DisposeCollector s, object d)
        {
            if (d != null)
            {
                if (s == null)
                {
                    s = new DisposeCollector();
                }
                s._list.Add(d);
            }
            return s;
        }
        public static DisposeCollector operator +(DisposeCollector s, IEnumerable<object> d)
        {
            if (d != null)
            {
                if (s == null)
                {
                    s = new DisposeCollector();
                }
                foreach (var item in d)
                {
                    if (item != null)
                    {
                        s._list.Add(item);
                    }
                }
            }
            return s;
        }
        public static DisposeCollector operator -(DisposeCollector s, object d)
        {
            if (d != null)
            {
                s._list.Remove(d);
            }
            if (s._list.Count == 0)
            {
                return null;
            }
            return s;
        }
        public static DisposeCollector operator -(DisposeCollector s, IEnumerable<object> d)
        {
            if (d != null)
            {
                foreach (var item in d)
                {
                    s._list.Remove(item);
                }
            }
            if (s._list.Count == 0)
            {
                return null;
            }
            return s;
        }
    }
}
