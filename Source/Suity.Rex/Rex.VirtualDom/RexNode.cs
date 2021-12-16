// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Collections;

namespace Suity.Rex.VirtualDom
{
    public class RexNode
    {
        public static readonly bool SetNullWhenEmtpy = false;

        readonly PathItem _localPath;
        internal RexPath _path;

        internal RexTree _model;
        internal RexNode _parent;
        internal RexNodeListenerSet _listener;

        Dictionary<PathItem, RexNode> _childNodes;


        object _data;
        ComputedData _computed;
        int _counter;

        internal RexNode(PathItem pathNode)
        {
            _localPath = pathNode;
        }


        public RexNode Parent { get { return _parent; } }
        public PathItem LocalPath { get { return _localPath; } }
        public RexPath Path { get { return _path; } }
        public IEnumerable<RexNode> ChildNodes
        {
            get
            {
                if (_childNodes != null)
                {
                    return _childNodes.Values.Select(o => o);
                }
                else
                {
                    return EmptyArray<RexNode>.Empty;
                }
            }
        }
        public bool IsTerminal { get { return _childNodes == null || _childNodes.Count == 0; } }
        public bool IsAction
        {
            get
            {
                var obj = GetData();
                return obj is Action<ActionArguments> || obj is Action;
            }
        }

        public object GetData()
        {
            if (_computed != null)
            {
                return _computed.GetData();
            }
            return _data;
        }
        public bool IsComputedData { get { return _computed != null; } }
        public int Counter { get { return _counter; } }
        public int ListenerCount { get { return _listener?.ListenerCount ?? 0; } }
        

        internal void AddChild(RexNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException();
            }
            if (node == this)
            {
                throw new InvalidOperationException();
            }
            node._parent?.RemoveChild(node);

            if (_childNodes == null)
            {
                _childNodes = new Dictionary<PathItem, RexNode>();
            }
            if (_childNodes.TryGetValue(node.LocalPath, out RexNode current))
            {
                current._parent = null;
            }
            _childNodes[node.LocalPath] = node;
            node._model = _model;
            node._parent = this;
            node._path = _path.Append(node.LocalPath);
            node._listener = _model.GetListener(node._path);
        }

        internal bool RemoveChild(RexNode node)
        {
            if (node == null)
            {
                return false;
            }
            if (node._parent != this)
            {
                return false;
            }
            if (_childNodes == null)
            {
                return false;
            }

            if (_childNodes.TryGetValue(node.LocalPath, out RexNode current) && current == node)
            {
                _childNodes.Remove(node.LocalPath);
                node._model = null;
                node._parent = null;
                node._path = null;
                node._listener = null;

                if (SetNullWhenEmtpy)
                {
                    if (_childNodes.Count == 0)
                    {
                        _childNodes = null;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        internal RexNode EnsureNode(PathItem childPath)
        {
            if (_childNodes == null)
            {
                _childNodes = new Dictionary<PathItem, RexNode>();
            }

            if (_childNodes.TryGetValue(childPath, out RexNode node))
            {
                return node;
            }
            else
            {
                node = new RexNode(childPath);
                AddChild(node);
                return node;
            }
        }
        internal RexNode GetNode(PathItem childPath)
        {
            if (_childNodes != null && _childNodes.TryGetValue(childPath, out RexNode node))
            {
                return node;
            }
            else
            {
                return null;
            }
        }



        internal void SetData(object data)
        {
            if (_computed != null)
            {
                _computed.SetData(data);
                return;
            }

            _data = data;

            _listener?.DispatchData(data);

            _counter++;
        }
        internal void SetDataDeep(object data)
        {
            if (_computed != null)
            {
                _computed.SetData(data);
                return;
            }

            Clear();

            _data = data;

            _listener?.DispatchData(data);

            _counter++;

            var propNames = ObjectType.GetPropertyNames(data);
            foreach (var name in propNames)
            {
                var value = ObjectType.GetProperty(data, name);
                EnsureNode(new PathItem(name, -1)).SetDataDeep(value);
            }

            if (data is Array ary)
            {
                for (int i = 0; i < ary.Length; i++)
                {
                    EnsureNode(new PathItem(null, i)).SetDataDeep(ary.GetValue(i));
                }
                return;
            }

            if (data is IList list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    EnsureNode(new PathItem(null, i)).SetDataDeep(list[i]);
                }
            }
        }
        internal void UpdateData()
        {
            _listener?.DispatchData(GetData());
        }
        internal void SetDefaultData(object data)
        {
            if (_computed != null)
            {
                return;
            }
            if (_data != null)
            {
                return;
            }

            _data = data;

            _listener?.DispatchData(data);

            _counter++;
        }
        internal IDisposable SetComputed(ComputedData computed)
        {
            _computed = computed;
            if (computed != null)
            {
                _data = null;
            }
            _counter++;

            return new DisposableAction(() => _computed = null);
        }
        internal bool DoAction(ActionArguments argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException();
            }
            _counter++;
            return _listener?.DispatchAction(argument) ?? false;
        }


        internal void RemoveListenersByTagDeep(string tag, ref int count)
        {
            _listener?.RemoveByTag(tag, ref count);

            if (_childNodes != null)
            {
                foreach (var node in _childNodes.Values)
                {
                    node.RemoveListenersByTagDeep(tag, ref count);
                }
            }
        }



        internal void Clear()
        {
            _data = null;
            _computed = null;

            _childNodes?.Clear();
        }

        public string GetBreifString()
        {
            var data = GetData();
            string str = string.Empty;
            if (data == null)
            {
                if (_childNodes?.Count > 0)
                {
                    str = string.Empty;
                }
                else
                {
                    str = "null";
                }
            }
            else if (data is string)
            {
                str = string.Format("\"{0}\"", data);
            }
            else if (data is Action<ActionArguments> || data is Action)
            {
                str = "[Action]";
            }
            else if (data is ICollection collection)
            {
                str = string.Format("[{0} items]", collection.Count);
            }
            else
            {
                str = data.ToString();
            }

            if (IsComputedData)
            {
                str = "(Computed) " + str;
            }
            return str;
        }

        public override string ToString()
        {
            if (_parent != null)
            {
                return _parent.ToString() + "." + _localPath.ToString();
            }
            else
            {
                return _localPath.ToString();
            }
        }
    }
}
