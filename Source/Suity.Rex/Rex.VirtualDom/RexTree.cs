// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Collections;
using Suity.Helpers;

namespace Suity.Rex.VirtualDom
{
    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexTree : Suity.Object
    {
        public static readonly RexTree Global = new RexTree(true);

        internal readonly bool _isGlobal;
        readonly RexNode _rootNode;
        readonly Dictionary<RexPath, RexNodeListenerSet> _listeners = new Dictionary<RexPath, RexNodeListenerSet>();

        public event EventHandler<RexPathValueEventArgs> PathSet;
        public event EventHandler<RexPathEventArgs> PathSetComputed;
        public event EventHandler<RexPathEventArgs> PathUpdate;
        public event EventHandler<RexPathEventArgs> PathGet;
        public event EventHandler<RexPathValueEventArgs> PathDoAction;


        public RexTree()
        {
            _rootNode = new RexNode(PathItem.Empty)
            {
                _model = this,
                _path = RexPath.Empty
            };
        }
        public RexTree(object rootData)
            : this()
        {
            SetDataDeep(RexPath.Empty, rootData);
        }
        private RexTree(bool isGlobal)
            : this()
        {
            _isGlobal = isGlobal;
        }
        protected override string GetName()
        {
            if (_isGlobal)
            {
                return "Global RexTree";
            }
            else
            {
                return base.GetName();
            }
        }

        public RexNode RootNode { get { return _rootNode; } }


        #region Data
        public void SetDataDeep<T>(T data)
        {
            SetDataDeep(RexPath.Empty, data);
        }
        public void SetDataDeep<T>(RexPath path, T data)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            OnPathSet(path, data);

            RexNode currentNode = _rootNode;
            PathItem lastPathItem = PathItem.Empty;

            foreach (var item in path.Items)
            {
                currentNode = currentNode.EnsureNode(item);
                lastPathItem = item;
            }
            currentNode.SetDataDeep(data);
        }


        public void SetData<T>(RexPath path, T data)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            OnPathSet(path, data);

            RexNode currentNode = _rootNode;
            PathItem lastPathItem = PathItem.Empty;

            foreach (var item in path.Items)
            {
                currentNode = currentNode.EnsureNode(item);
                lastPathItem = item;
            }
            currentNode.SetData(data);
            //if (currentWNode != null && currentWNode != currentNode)
            //{
            //    currentWNode.UpdateDataListenerDeep(true, lastPathItem);
            //}
        }
        public void SetDefaultData<T>(RexPath path, T defaultData)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            OnPathSet(path, defaultData);

            RexNode currentNode = _rootNode;
            RexNode currentWNode = null;
            PathItem lastPathItem = PathItem.Empty;

            foreach (var item in path.Items)
            {
                currentNode = currentNode.EnsureNode(item);
                lastPathItem = item;
            }
            currentNode.SetDefaultData(defaultData);
            //if (currentWNode != null && currentWNode != currentNode)
            //{
            //    currentWNode.UpdateDataListenerDeep(true, lastPathItem);
            //}
        }
        public void UpdateData(RexPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            OnPathUpdate(path);

            RexNode currentNode = _rootNode;
            RexNode currentWNode = null;
            PathItem lastPathItem = PathItem.Empty;

            foreach (var item in path.Items)
            {
                currentNode = currentNode.GetNode(item);
                lastPathItem = item;
                if (currentNode == null && currentWNode == null)
                {
                    return;
                }
            }
            currentNode?.UpdateData();
            //if (currentWNode != null && currentWNode != currentNode)
            //{
            //    currentWNode.UpdateDataListenerDeep(false, lastPathItem);
            //}
        }
        public void SetDataQueued<T>(RexPath path, T data)
        {
            QueuedAction.Do(() => SetData<T>(path, data));
        }
        public void UpdateDataQueued(RexPath path)
        {
            QueuedAction.Do(() => UpdateData(path));
        }

        public IDisposable SetComputedData<T>(RexPath path, Func<T> getter = null, Action<T> setter = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            OnPathSetComputed(path);

            RexNode currentNode = _rootNode;
            foreach (var item in path.Items)
            {
                currentNode = currentNode.EnsureNode(item);
            }
            if (getter != null || setter != null)
            {
                return currentNode.SetComputed(new ComputedData<T>(getter, setter));
            }
            else
            {
                return currentNode.SetComputed(null);
            }
        }

        public object GetData(RexPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            OnPathGet(path);

            RexNode currentNode = _rootNode;
            foreach (var item in path.Items)
            {
                currentNode = currentNode.GetNode(item);
                if (currentNode == null)
                {
                    return null;
                }
            }

            if (currentNode != null)
            {
                return currentNode.GetData();
            }
            else
            {
                return null;
            }
        }
        public T GetData<T>(RexPath path)
        {
            object data = GetData(path);

            if (data is T t)
            {
                return t;
            }
            else
            {
                return default(T);
            }
        } 
        #endregion

        #region DoAction
        public void DoAction(RexPath path)
        {
            DoAction(path, ActionArgument.Empty as ActionArguments);
        }
        public void DoAction<T>(RexPath path, T argument)
        {
            DoAction(path, new ActionArgument<T>(argument) as ActionArguments);
        }
        public void DoAction<T1, T2>(RexPath path, T1 arg1, T2 arg2)
        {
            DoAction(path, new ActionArgument<T1, T2>(arg1, arg2) as ActionArguments);
        }
        public void DoAction<T1, T2, T3>(RexPath path, T1 arg1, T2 arg2, T3 arg3)
        {
            DoAction(path, new ActionArgument<T1, T2, T3>(arg1, arg2, arg3) as ActionArguments);
        }
        public void DoAction<T1, T2, T3, T4>(RexPath path, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            DoAction(path, new ActionArgument<T1, T2, T3, T4>(arg1, arg2, arg3, arg4) as ActionArguments);
        }
        public void DoAction(RexPath path, ActionArguments arguments)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (arguments == null)
            {
                throw new ArgumentNullException();
            }

            OnPathDoAction(path, arguments);

            var currentNode = GetListener(path);
            currentNode?.DispatchAction(arguments);

            //var obj = GetData(path);
            //if (obj != null)
            //{
            //    if (obj is Action<ActionArguments>)
            //    {
            //        ((Action<ActionArguments>)obj)(arguments);
            //    }
            //    else if (obj is Action && arguments is ActionArgument)
            //    {
            //        ((Action)obj)();
            //    }
            //}
        }


        public void DoActionQueued(RexPath path)
        {
            QueuedAction.Do(() => DoAction(path));
        }
        public void DoActionQueued<T>(RexPath path, T argument)
        {
            QueuedAction.Do(() => DoAction(path, argument));
        }
        public void DoActionQueued<T1, T2>(RexPath path, T1 arg1, T2 arg2)
        {
            QueuedAction.Do(() => DoAction(path, arg1, arg2));
        }
        public void DoActionQueued<T1, T2, T3>(RexPath path, T1 arg1, T2 arg2, T3 arg3)
        {
            QueuedAction.Do(() => DoAction(path, arg1, arg2, arg3));
        }
        public void DoActionQueued<T1, T2, T3, T4>(RexPath path, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            QueuedAction.Do(() => DoAction(path, arg1, arg2, arg3, arg4));
        }
        #endregion

        #region Listener
        public IRexHandle AddDataListener<T>(RexPath path, Action<T> callBack, string tag = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (callBack == null)
            {
                throw new ArgumentNullException();
            }
            RexNodeListenerSet currentNode = EnsureListener(path);
            var listener = currentNode.AddDataListener<T>(callBack);
            listener.Tag = tag;
            return listener;
        }
        public IDisposable AddActionListener(RexPath path, Action callBack, string tag = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (callBack == null)
            {
                throw new ArgumentNullException();
            }
            RexNodeListenerSet currentNode = EnsureListener(path);
            var listener = currentNode.AddActionListener(callBack);
            listener.Tag = tag;
            return listener;
        }
        public IDisposable AddActionListener<T>(RexPath path, Action<T> callBack, string tag = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (callBack == null)
            {
                throw new ArgumentNullException();
            }
            RexNodeListenerSet currentNode = EnsureListener(path);
            var listener = currentNode.AddActionListener(callBack);
            listener.Tag = tag;
            return listener;
        }
        public IDisposable AddActionListener<T1, T2>(RexPath path, Action<T1, T2> callBack, string tag = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (callBack == null)
            {
                throw new ArgumentNullException();
            }
            RexNodeListenerSet currentNode = EnsureListener(path);
            var listener = currentNode.AddActionListener(callBack);
            listener.Tag = tag;
            return listener;
        }
        public IDisposable AddActionListener<T1, T2, T3>(RexPath path, Action<T1, T2, T3> callBack, string tag = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (callBack == null)
            {
                throw new ArgumentNullException();
            }
            RexNodeListenerSet currentNode = EnsureListener(path);
            var listener = currentNode.AddActionListener(callBack);
            listener.Tag = tag;
            return listener;
        }
        public IDisposable AddActionListener<T1, T2, T3, T4>(RexPath path, Action<T1, T2, T3, T4> callBack, string tag = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (callBack == null)
            {
                throw new ArgumentNullException();
            }
            RexNodeListenerSet currentNode = EnsureListener(path);
            var listener = currentNode.AddActionListener(callBack);
            listener.Tag = tag;
            return listener;
        }


        public IRexHandle AddBeforeListener<T>(RexPath path, Action<T> callBack, string tag = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            RexNodeListenerSet currentNode = EnsureListener(path);
            var listener = currentNode.AddBeforeListener<T>(callBack);
            listener.Tag = tag;
            return listener;
        }
        public bool RemoveBeforeListener<T>(RexPath path, Action<T> callBack)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            RexNodeListenerSet currentNode = GetListener(path);
            if (currentNode != null)
            {
                return currentNode.RemoveBeforeListener(callBack);
            }
            else
            {
                return false;
            }
        }
        public IRexHandle AddAfterListener<T>(RexPath path, Action<T> callBack, string tag = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            RexNodeListenerSet currentNode = EnsureListener(path);
            var listener = currentNode.AddAfterListener(callBack);
            listener.Tag = tag;
            return listener;
        }
        public bool UnsetAfterListener<T>(RexPath path, Action<T> callBack)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            RexNodeListenerSet currentNode = GetListener(path);
            if (currentNode != null)
            {
                return currentNode.RemoveAfterListener(callBack);
            }
            else
            {
                return false;
            }
        }

        public bool RemoveListener(RexPath path, Action callBack)
        {
            return RemoveDelegateListener(path, callBack);
        }
        public bool RemoveListener<T>(RexPath path, Action<T> callBack)
        {
            return RemoveDelegateListener(path, callBack);
        }
        public bool RemoveListener<T1, T2>(RexPath path, Action<T1, T2> callBack)
        {
            return RemoveDelegateListener(path, callBack);
        }
        public bool RemoveListener<T1, T2, T3>(RexPath path, Action<T1, T2, T3> callBack)
        {
            return RemoveDelegateListener(path, callBack);
        }
        public bool RemoveListener<T1, T2, T3, T4>(RexPath path, Action<T1, T2, T3, T4> callBack)
        {
            return RemoveDelegateListener(path, callBack);
        }
        private bool RemoveDelegateListener(RexPath path, Delegate callBack)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            RexNodeListenerSet currentNode = GetListener(path);
            if (currentNode != null)
            {
                return currentNode.RemoveListener(callBack);
            }
            else
            {
                return false;
            }
        } 


        public int RemoveListenersByTag(string tag)
        {
            int count = 0;
            _rootNode.RemoveListenersByTagDeep(tag, ref count);
            return count;
        }
        #endregion

        #region Mapping

        public IDisposable AddMapping(RexPath path, RexPath pathTo)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (pathTo == null)
            {
                throw new ArgumentNullException();
            }
            var listener = EnsureListener(path).AddMapping(pathTo);
            return listener;
        }
        public void AddMappings(RexPath path, params RexPath[] pathTos)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            var node = EnsureListener(path);
            foreach (var pathTo in pathTos)
            {
                if (pathTo != null)
                {
                    node.AddMapping(pathTo);
                }
            }
        }
        public void AddMappingsFrom(RexPath pathTo, params RexPath[] paths)
        {
            if (pathTo == null)
            {
                throw new ArgumentNullException();
            }
            foreach (var path in paths)
            {
                if (path != null)
                {
                    EnsureListener(path).AddMapping(pathTo);
                }
            }
        }
        public bool RemoveMapping(RexPath path, RexPath pathTo)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (pathTo == null)
            {
                throw new ArgumentNullException();
            }
            var node = GetListener(path);
            if (node != null)
            {
                return node.RemoveMapping(pathTo);
            }
            return false;
        }
        public void RemoveMappings(RexPath path, params RexPath[] pathTos)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            var node = GetListener(path);
            if (node != null)
            {
                foreach (var pathTo in pathTos)
                {
                    if (pathTo != null)
                    {
                        node.RemoveMapping(pathTo);
                    }
                }
            }
        }
        public void RemoveMappingsFrom(RexPath pathTo, params RexPath[] paths)
        {
            if (pathTo == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var path in paths)
            {
                var node = path != null ? GetListener(path) : null;
                node?.RemoveMapping(pathTo);
            }
        }

        #endregion

        public void Clear()
        {
            _rootNode.Clear();
        }

        #region Internal
        internal void OnPathSet(RexPath path, object value)
        {
            PathSet?.Invoke(this, new RexPathValueEventArgs(path, value));
        }
        internal void OnPathSetComputed(RexPath path)
        {
            PathSetComputed?.Invoke(this, new RexPathEventArgs(path));
        }

        internal void OnPathGet(RexPath path)
        {
            PathGet?.Invoke(this, new RexPathEventArgs(path));
        }
        internal void OnPathUpdate(RexPath path)
        {
            PathUpdate?.Invoke(this, new RexPathEventArgs(path));
        }
        internal void OnPathDoAction(RexPath path, ActionArguments args)
        {
            PathDoAction?.Invoke(this, new RexPathValueEventArgs(path, args));
        }

        private RexNode EnsureNode(RexPath path)
        {
            RexNode currentNode = _rootNode;
            foreach (var item in path.Items)
            {
                currentNode = currentNode.EnsureNode(item);
            }
            return currentNode;
        }
        private RexNode GetNode(RexPath path)
        {
            RexNode currentNode = _rootNode;
            foreach (var item in path.Items)
            {
                currentNode = currentNode.GetNode(item);
                if (currentNode == null)
                {
                    break;
                }
            }
            return currentNode;
        }
        private RexNodeListenerSet EnsureListener(RexPath path)
        {
            if (!_listeners.TryGetValue(path, out RexNodeListenerSet listener))
            {
                listener = new RexNodeListenerSet(this, path);
                _listeners.Add(path, listener);
                RexNode node = GetNode(path);
                if (node != null)
                {
                    node._listener = listener;
                }
            }
            return listener;
        }
        internal RexNodeListenerSet GetListener(RexPath path)
        {
            return _listeners.GetValueOrDefault(path);
        } 
        #endregion
    }
}
