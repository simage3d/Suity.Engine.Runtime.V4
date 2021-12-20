// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using Suity.Helpers;
using Suity.Synchonizing.Preset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Core
{
    public static class Member
    {
        public static object GetProperty(object obj, string name, ISyncContext context = null)
        {
            return SyncTypeExtensions.GetSyncObject(obj)?.GetProperty(name, context);
        }
        public static void SetProperty(object obj, string name, object value, ISyncContext context = null)
        {
            SyncTypeExtensions.GetSyncObject(obj)?.SetProperty(name, value, context);
        }
        public static T GetProperty<T>(object obj, string name, ISyncContext context = null)
        {
            ISyncObject syncObject = SyncTypeExtensions.GetSyncObject(obj);
            if (syncObject != null)
            {
                return syncObject.GetProperty<T>(name, context);
            }
            else
            {
                return default(T);
            }
        }
        public static void SetProperty<T>(object obj, string name, T value, ISyncContext context = null)
        {
            ISyncObject syncObject = SyncTypeExtensions.GetSyncObject(obj);
            syncObject?.SetProperty<T>(name, value, context);
        }

        



        public static Type GetElementType(object list)
        {
            ISyncList syncList = SyncTypeExtensions.GetSyncList(list);
            return syncList?.GetElementType();
        }
        public static object GetItem(object list, int index)
        {
            ISyncList syncList = SyncTypeExtensions.GetSyncList(list);
            return syncList?.GetItem(index);
        }
        public static void SetItem(object list, int index, object value)
        {
            ISyncList syncList = SyncTypeExtensions.GetSyncList(list);
            syncList?.SetItem(index, value);
        }
        public static object CreateNewItem(object list, string parameter = null)
        {
            ISyncList syncList = SyncTypeExtensions.GetSyncList(list);
            return syncList?.CreateNewItem(parameter);
        }
        public static void Add(object list, object value)
        {
            ISyncList syncList = SyncTypeExtensions.GetSyncList(list);
            syncList?.Add(value);
        }
        public static void Insert(object list, int index, object value)
        {
            ISyncList syncList = SyncTypeExtensions.GetSyncList(list);
            syncList?.Insert(index, value);
        }
        public static void RemoveAt(object list, int index)
        {
            ISyncList syncList = SyncTypeExtensions.GetSyncList(list);
            syncList?.RemoveAt(index);
        }



        public static IEnumerable<object> GetMembers(object obj, bool deep)
        {
            return GetMembers<object>(obj, deep, SyncContext.Empty);
        }
        public static IEnumerable<T> GetMembers<T>(object obj, bool deep)
        {
            return GetMembers<T>(obj, deep, SyncContext.Empty);
        }
        private static IEnumerable<T> GetMembers<T>(object obj, bool deep, SyncContext context)
        {
            if (context == null)
            {
                context = SyncContext.Empty;
            }

            if (obj == null)
            {
                return EmptyArray<T>.Empty;
            }

            if (obj is ISyncNode node)
            {
                return GetMembers<T>(node, deep, context).Concat(GetMembers<T>(node.GetList(), deep, context));
            }
            if (obj is ISyncObject syncObject)
            {
                return GetMembers<T>(syncObject, deep, context);
            }
            else if (obj is ISyncList syncList)
            {
                return GetMembers<T>(syncList, deep, context);
            }
            else if (SyncTypeExtensions.GetObjectProxyType(obj) != null)
            {
                return GetMembers<T>(SyncTypeExtensions.CreateObjectProxy(obj), deep, context);
            }
            else if (SyncTypeExtensions.GetListProxyType(obj) != null)
            {
                return GetMembers<T>(SyncTypeExtensions.CreateListProxy(obj), deep, context);
            }
            else if (context.Resolver != null)
            {
                object proxy = context.Resolver.CreateProxy(obj);

                if (proxy is ISyncObject syncObject2)
                {
                    return GetMembers<T>(syncObject2, deep, context);
                }
                else if (proxy is ISyncList syncList2)
                {
                    return GetMembers<T>(syncList2, deep, context);
                }
            }

            return EmptyArray<T>.Empty;
        }
        private static IEnumerable<T> GetMembers<T>(ISyncObject obj, bool deep, SyncContext context)
        {
            GetAllPropertySync getAll = new GetAllPropertySync(SyncIntent.Visit, false);
            obj.Sync(getAll, context);

            return GetMembers<T>(getAll.Values.Values.Select(o => o.Value), deep, context);
        }
        private static IEnumerable<T> GetMembers<T>(ISyncList list, bool deep, SyncContext context)
        {
            GetAllIndexSync getAll = new GetAllIndexSync(SyncIntent.Visit);
            list.Sync(getAll, context);

            return GetMembers<T>(getAll.Values.Select(o => o.Value), deep, context);
        }
        private static IEnumerable<T> GetMembers<T>(IEnumerable<object> values, bool deep, SyncContext context)
        {
            if (deep)
            {
                foreach (var item in values)
                {
                    if (item is T t)
                    {
                        yield return t;
                    }
                    else
                    {
                        foreach (var childItem in GetMembers<T>(item, deep, context))
                        {
                            yield return childItem;
                        }
                    }
                }
            }
            else
            {
                foreach (var item in values.OfType<T>())
                {
                    yield return item;
                }
            }
        }


        public static bool SupportSyncObject(object obj)
        {
            if (obj is ISyncObject syncObject)
            {
                return true;
            }
            else if (SyncTypeExtensions.GetObjectProxyType(obj) != null)
            {
                return true;
            }

            return false;
        }
        public static bool SupportSyncList(object obj)
        {
            if (obj is ISyncNode node)
            {
                return true;
            }
            else if (obj is ISyncList syncList)
            {
                return true;
            }
            else if (SyncTypeExtensions.GetListProxyType(obj) != null)
            {
                return true;
            }

            return false;
        }

        public static IEnumerable<KeyValuePair<string, object>> GetProperties(object obj)
        {
            if (obj == null)
            {
                return EmptyArray<KeyValuePair<string, object>>.Empty;
            }

            SyncContext context = SyncContext.Empty;

            if (obj is ISyncObject syncObject)
            {
                return GetProperties(syncObject, context);
            }
            else if (SyncTypeExtensions.GetObjectProxyType(obj) != null)
            {
                return GetProperties(SyncTypeExtensions.CreateObjectProxy(obj), context);
            }
            else if (context.Resolver != null)
            {
                object proxy = context.Resolver.CreateProxy(obj);

                if (proxy is ISyncObject syncObject2)
                {
                    return GetProperties(syncObject2, context);
                }
            }

            return EmptyArray<KeyValuePair<string, object>>.Empty;
        }
        public static IEnumerable<object> GetListItems(object obj)
        {
            if (obj == null)
            {
                return EmptyArray<object>.Empty;
            }

            SyncContext context = SyncContext.Empty;

            if (obj is ISyncNode node)
            {
                return GetListItems(node.GetList(), context);
            }
            else if (obj is ISyncList syncList)
            {
                return GetListItems(syncList, context);
            }
            else if (SyncTypeExtensions.GetListProxyType(obj) != null)
            {
                return GetListItems(SyncTypeExtensions.CreateListProxy(obj), context);
            }
            else if (context.Resolver != null)
            {
                object proxy = context.Resolver.CreateProxy(obj);

                if (proxy is ISyncList syncList2)
                {
                    return GetListItems(syncList2, context);
                }
            }

            return EmptyArray<object>.Empty;
        }

        private static IEnumerable<KeyValuePair<string, object>> GetProperties(ISyncObject obj, SyncContext context)
        {
            GetAllPropertySync getAll = new GetAllPropertySync(SyncIntent.Visit, false);
            obj.Sync(getAll, SyncContext.Empty);

            foreach (var pair in getAll.Values)
            {
                yield return new KeyValuePair<string, object>(pair.Key, pair.Value.Value);
            }
        }
        private static IEnumerable<object> GetListItems(ISyncList list, SyncContext context)
        {
            GetAllIndexSync getAll = new GetAllIndexSync(SyncIntent.Visit);
            list.Sync(getAll, SyncContext.Empty);

            foreach (var item in getAll.Values)
            {
                yield return item.Value;
            }
        }
    }
}
