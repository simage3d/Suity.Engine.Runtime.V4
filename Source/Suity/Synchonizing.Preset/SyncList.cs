// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Preset
{
    public class SyncList<T> : ISyncList where T : new()
    {
        readonly List<T> _list = new List<T>();
        public List<T> List => _list;

        int ISyncList.Count => _list.Count;

        public SyncList()
        {
        }

        public virtual void Sync(IIndexSync sync, ISyncContext context)
        {
            sync.SyncGenericIList(_list, typeof(T), OnCheckValue, () => OnCreateNew(), OnAdded, OnRemoved);
        }

        protected virtual void OnAdded(T obj)
        {
        }
        protected virtual void OnRemoved(T obj)
        {
        }
        protected virtual T OnCreateNew()
        {
            return new T();
        }
        protected virtual bool OnCheckValue(T obj)
        {
            return true;
        }
    }
}
