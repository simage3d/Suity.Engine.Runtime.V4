// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using Suity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.VirtualDom
{
    class RexNodeListenerSet
    {
        public static readonly bool SetNullWhenEmtpy = false;

        internal readonly RexTree _model;
        internal readonly RexPath _path;

        internal Dictionary<Delegate, RexNodeListener> _listeners;
        internal Dictionary<Delegate, RexNodeListener> _beforeListeners;
        internal Dictionary<Delegate, RexNodeListener> _afterListeners;


        public RexNodeListenerSet(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }


        public int ListenerCount
        {
            get
            {
                int count = _listeners?.Count ?? 0;
                count += _beforeListeners?.Count ?? 0;
                count += _afterListeners?.Count ?? 0;

                return count;
            }
        }

        internal RexNodeDataListener<T> AddDataListener<T>(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            var listener = new RexNodeDataListener<T>(this, action);
            EnsureListenerDic()[action] = listener;
            return listener;
        }
        internal RexNodeListener AddActionListener(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            var listener = new RexNodeActionListener(this, action);
            EnsureListenerDic()[action] = listener;
            return listener;
        }
        internal RexNodeListener AddActionListener<T>(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            var listener = new RexNodeActionListener<T>(this, action);
            EnsureListenerDic()[action] = listener;
            return listener;
        }
        internal RexNodeListener AddActionListener<T1, T2>(Action<T1, T2> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            var listener = new RexNodeActionListener<T1, T2>(this, action);
            EnsureListenerDic()[action] = listener;
            return listener;
        }
        internal RexNodeListener AddActionListener<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            var listener = new RexNodeActionListener<T1, T2, T3>(this, action);
            EnsureListenerDic()[action] = listener;
            return listener;
        }
        internal RexNodeListener AddActionListener<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            var listener = new RexNodeActionListener<T1, T2, T3, T4>(this, action);
            EnsureListenerDic()[action] = listener;
            return listener;
        }

        internal bool RemoveListener(Delegate action)
        {
            if (action == null)
            {
                return false;
            }
            if (_listeners == null)
            {
                return false;
            }
            bool removed = _listeners.Remove(action);
            if (SetNullWhenEmtpy)
            {
                if (_listeners.Count == 0)
                {
                    _listeners = null;
                }
            }
            return removed;
        }
        internal void RemoveListener(RexNodeListener listener)
        {
            RemoveListener(listener.GetKey());
            RemoveBeforeListener(listener.GetKey());
            RemoveAfterListener(listener.GetKey());
        }


        internal RexNodeDataListener<T> AddBeforeListener<T>(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            if (_beforeListeners == null)
            {
                _beforeListeners = new Dictionary<Delegate, RexNodeListener>();
            }
            var listener = new RexNodeDataListener<T>(this, action);
            _beforeListeners[action] = listener;
            return listener;
        }
        internal RexNodeDataListener<T> AddAfterListener<T>(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            if (_afterListeners == null)
            {
                _afterListeners = new Dictionary<Delegate, RexNodeListener>();
            }
            var listener = new RexNodeDataListener<T>(this, action);
            _afterListeners[action] = listener;
            return listener;
        }
        internal bool RemoveBeforeListener(Delegate action)
        {
            if (action == null)
            {
                return false;
            }
            if (_beforeListeners == null)
            {
                return false;
            }
            bool removed = _beforeListeners.Remove(action);
            if (SetNullWhenEmtpy)
            {
                if (_beforeListeners.Count == 0)
                {
                    _beforeListeners = null;
                }
            }
            return removed;
        }
        internal bool RemoveAfterListener(Delegate action)
        {
            if (action == null)
            {
                return false;
            }
            if (_afterListeners == null)
            {
                return false;
            }
            bool removed = _afterListeners.Remove(action);
            if (SetNullWhenEmtpy)
            {
                if (_afterListeners.Count == 0)
                {
                    _afterListeners = null;
                }
            }
            return removed;
        }


        internal RexNodeListener AddMapping(RexPath pathTo)
        {
            if (pathTo == null)
            {
                throw new ArgumentNullException();
            }
            if (_model == null)
            {
                throw new NullReferenceException("Model");
            }
            if (_listeners != null)
            {
                RexNodeMappingListener current = _listeners.Values.OfType<RexNodeMappingListener>().FirstOrDefault(o => o.PathTo == pathTo);
                if (current != null)
                {
                    return current;
                }
            }
            else
            {
                _listeners = new Dictionary<Delegate, RexNodeListener>();
            }
            RexNodeMappingListener listener = new RexNodeMappingListener(this, pathTo);
            _listeners.Add(listener.GetKey(), listener);
            return listener;
        }
        internal bool RemoveMapping(RexPath pathTo)
        {
            if (_listeners != null)
            {
                RexNodeMappingListener current = _listeners.Values.OfType<RexNodeMappingListener>().FirstOrDefault(o => o.PathTo == pathTo);
                if (current != null)
                {
                    _listeners.Remove(current.GetKey());
                    return true;
                }
            }

            return false;
        }


        internal void RemoveByTag(string tag, ref int count)
        {
            List<Delegate> removes = null;

            if (_listeners != null)
            {
                foreach (var pair in _listeners)
                {
                    if (pair.Value.Tag == tag)
                    {
                        (removes ?? (removes = new List<Delegate>())).Add(pair.Key);
                    }
                }
            }
            if (_beforeListeners != null)
            {
                foreach (var pair in _beforeListeners)
                {
                    if (pair.Value.Tag == tag)
                    {
                        (removes ?? (removes = new List<Delegate>())).Add(pair.Key);
                    }
                }
            }
            if (_afterListeners != null)
            {
                foreach (var pair in _afterListeners)
                {
                    if (pair.Value.Tag == tag)
                    {
                        (removes ?? (removes = new List<Delegate>())).Add(pair.Key);
                    }
                }
            }

            if (removes != null)
            {
                foreach (var remove in removes)
                {
                    _listeners.Remove(remove);
                    count++;
                }
            }

        }

        internal void Clear()
        {
            _listeners?.Clear();
            _beforeListeners?.Clear();
            _afterListeners?.Clear();
        }


        internal bool DispatchData(object data)
        {
            List<RexNodeListener> list = _listPool.Get();

            if (_beforeListeners != null)
            {
                try
                {
                    list.AddRange(_beforeListeners.Values);
                    foreach (var listener in list)
                    {
                        listener.Invoke(data);
                    }
                    list.Clear();
                }
                catch (RexCancelException)
                {
                    list.Clear();
                    _listPool.Recycle(list);
                    return false;
                }
            }

            bool handled = false;

            if (_listeners != null)
            {
                list.AddRange(_listeners.Values);
                foreach (var listener in list)
                {
                    listener.Invoke(data);
                    handled = true;
                }
                list.Clear();
            }

            if (_afterListeners != null)
            {
                list.AddRange(_afterListeners.Values);
                foreach (var listener in list)
                {
                    listener.Invoke(data);
                }
                list.Clear();
            }

            _listPool.Recycle(list);

            return handled;
        }
        internal bool DispatchAction(ActionArguments arguments)
        {
            List<RexNodeListener> list = _listPool.Get();

            if (_beforeListeners != null)
            {
                try
                {
                    list.AddRange(_beforeListeners.Values);
                    foreach (var listener in list)
                    {
                        try
                        {
                            listener.Invoke(arguments);
                        }
                        catch (Exception err)
                        {
                            Logs.LogError(err);
                        }
                    }
                    list.Clear();
                }
                catch (RexCancelException)
                {
                    list.Clear();
                    _listPool.Recycle(list);
                    return false;
                }
            }

            bool handled = false;

            if (_listeners != null)
            {
                list.AddRange(_listeners.Values);
                foreach (var listener in list)
                {
                    try
                    {
                        listener.Invoke(arguments);
                        handled = true;
                    }
                    catch (Exception err)
                    {
                        Logs.LogError(err);
                    }
                }
                list.Clear();
            }
            if (_afterListeners != null)
            {
                list.AddRange(_afterListeners.Values);
                foreach (var listener in list)
                {
                    try
                    {
                        listener.Invoke(arguments);
                    }
                    catch (Exception err)
                    {
                        Logs.LogError(err);
                    }
                }
                list.Clear();
            }

            _listPool.Recycle(list);

            return handled;
        }
        private Dictionary<Delegate, RexNodeListener> EnsureListenerDic()
        {
            return _listeners ?? (_listeners = new Dictionary<Delegate, RexNodeListener>());
        }

        private static readonly Pool<List<RexNodeListener>> _listPool = new Pool<List<RexNodeListener>>();
    }
}
