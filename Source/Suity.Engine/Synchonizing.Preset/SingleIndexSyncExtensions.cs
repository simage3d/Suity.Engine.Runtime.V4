// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Suity.Synchonizing.Preset
{
    public static class SingleIndexSyncExtensions
    {
        public static Type GetElementType(this ISyncList list)
        {
            SingleIndexSync sync = SingleIndexSync.CreateElementTypeGetter();
            list.Sync(sync, SyncContext.Empty);
            return sync.Value as Type;
        }
        public static object GetItem(this ISyncList list, int index)
        {
            if (list == null) throw new ArgumentNullException();

            SingleIndexSync sync = SingleIndexSync.CreateGetter(index);
            list.Sync(sync, SyncContext.Empty);
            return sync.Value;
        }
        public static void SetItem(this ISyncList list, int index, object value)
        {
            SingleIndexSync sync = SingleIndexSync.CreateSetter(index, value);
            list.Sync(sync, SyncContext.Empty);
        }
        public static object CreateNewItem(this ISyncList list, string parameter = null)
        {
            if (list == null) throw new NullReferenceException();

            SingleIndexSync sync = SingleIndexSync.CreateActivator(parameter);
            list.Sync(sync, SyncContext.Empty);
            return sync.Value;
        }
        public static void Add(this ISyncList list, object value)
        {
            if (list == null) throw new NullReferenceException();

            SingleIndexSync sync = SingleIndexSync.CreateInserter(list.Count, value);
            list.Sync(sync, SyncContext.Empty);
        }
        public static void Insert(this ISyncList list, int index, object value)
        {
            if (list == null) throw new NullReferenceException();

            SingleIndexSync sync = SingleIndexSync.CreateInserter(index, value);
            list.Sync(sync, SyncContext.Empty);
        }
        public static void RemoveAt(this ISyncList list, int index)
        {
            if (list == null) throw new NullReferenceException();

            SingleIndexSync sync = SingleIndexSync.CreateRemover(index);
            list.Sync(sync, SyncContext.Empty);
        }
    }
}
