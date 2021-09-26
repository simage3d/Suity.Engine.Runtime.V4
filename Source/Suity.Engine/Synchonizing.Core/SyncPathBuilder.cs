using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Core
{
    public class SyncPathBuilder
    {
        readonly LinkedList<object> _list = new LinkedList<object>();

        public SyncPathBuilder()
        {
        }

        public void Append(string pathItem)
        {
            _list.AddLast(SyncPath.ParseStr(pathItem));
        }
        public void Append(int pathItem)
        {
            _list.AddLast(pathItem);
        }
        public void Append(Guid pathItem)
        {
            _list.AddLast(pathItem);
        }
        public void Append(Loid pathItem)
        {
            _list.AddLast(pathItem);
        }
        public void Append(SyncPath path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                _list.AddLast(path[i]);
            }
        }
        public bool Append(object obj)
        {
            switch (obj)
            {
                case string str:
                    Append(str);
                    return true;
                case int index:
                    Append(index);
                    return true;
                case Guid guid:
                    Append(guid);
                    return true;
                case Loid loid:
                    Append(loid);
                    return true;
                case SyncPath path:
                    Append(path);
                    return true;
                default:
                    return false;
            }
        }

        public void Prepend(string pathItem)
        {
            _list.AddFirst(SyncPath.ParseStr(pathItem));
        }
        public void Prepend(int pathItem)
        {
            _list.AddFirst(pathItem);
        }
        public void Prepend(Guid pathItem)
        {
            _list.AddFirst(pathItem);
        }
        public void Prepend(Loid pathItem)
        {
            _list.AddFirst(pathItem);
        }
        public void Prepend(SyncPath path)
        {
            for (int i = path.Length -1; i >= 0; i--)
            {
                _list.AddFirst(path[i]);
            }
        }
        public bool Prepend(object obj)
        {
            switch (obj)
            {
                case string str:
                    Prepend(str);
                    return true;
                case int index:
                    Prepend(index);
                    return true;
                case Guid guid:
                    Prepend(guid);
                    return true;
                case Loid loid:
                    Prepend(loid);
                    return true;
                case SyncPath path:
                    Prepend(path);
                    return true;
                default:
                    return false;
            }
        }

        public object First => _list.First?.Value;
        public object Last => _list.Last?.Value;

        public bool RemoveFirst()
        {
            if (_list.Count > 0)
            {
                _list.RemoveFirst();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool RemoveLast()
        {
            if (_list.Count > 0)
            {
                _list.RemoveLast();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Trim()
        {
            TrimFirst();
            TrimLast();
        }
        public void TrimFirst()
        {
            while (_list.First?.Value is string s && string.IsNullOrEmpty(s))
            {
                _list.RemoveFirst();
            }
        }
        public void TrimLast()
        {
            while (_list.Last?.Value is string s && string.IsNullOrEmpty(s))
            {
                _list.RemoveLast();
            }
        }

        public void Clear()
        {
            _list.Clear();
        }
        public SyncPath ToSyncPath()
        {
            if (_list.Count > 0)
            {
                return new SyncPath(_list);
            }
            else
            {
                return SyncPath.Empty;
            }
        }
        public override string ToString()
        {
            return ToSyncPath().ToString();
        }

    }
}
