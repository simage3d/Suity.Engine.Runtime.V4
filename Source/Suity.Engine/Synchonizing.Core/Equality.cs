// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Synchonizing.Preset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Core
{
    public static class Equality
    {
        public static bool ObjectEquals(object objA, object objB)
        {
            return ObjectEquals(objA, objB, SyncContext.Empty, SyncContext.Empty);
        }
        public static bool ObjectEquals(object objA, object objB, SyncContext contextA, SyncContext contextB)
        {
            if (objA == null && objB == null)
            {
                return true;
            }
            if (objA == null || objB == null)
            {
                return false;
            }
            if (objA.GetType() != objB.GetType())
            {
                return false;
            }
            if (contextA == null)
            {
                contextA = SyncContext.Empty;
            }
            if (contextB == null)
            {
                contextB = SyncContext.Empty;
            }

            if (objA.GetType().IsValueType || objA is string)
            {
                return object.Equals(objA, objB);
            }
            else if (objA is ISerializeAsString sA && objB is ISerializeAsString sB)
            {
                return sA.Key == sB.Key;
            }
            else if (objA is ISyncObject syncObject)
            {
                return SyncObjectEquals(syncObject, (ISyncObject)objB, contextA, contextB);
            }
            else if (objA is ISyncList syncList)
            {
                return SyncListEquals(syncList, (ISyncList)objB, contextA, contextB);
            }
            else if (SyncTypeExtensions.GetObjectProxyType(objA) != null)
            {
                return SyncObjectEquals(SyncTypeExtensions.CreateObjectProxy(objA), SyncTypeExtensions.CreateObjectProxy(objB), contextA, contextB);
            }
            else if (SyncTypeExtensions.GetListProxyType(objA) != null)
            {
                return SyncListEquals(SyncTypeExtensions.CreateListProxy(objA), SyncTypeExtensions.CreateListProxy(objB), contextA, contextB);
            }
            else if (contextA.Resolver != null && contextB.Resolver != null)
            {
                object proxyA = contextA.Resolver.CreateProxy(objA);
                object proxyB = contextB.Resolver.CreateProxy(objB);

                if (proxyA is ISyncObject && proxyB is ISyncObject)
                {
                    return SyncObjectEquals((ISyncObject)proxyA, (ISyncObject)proxyB, contextA, contextB);
                }
                else if (proxyA is ISyncList && proxyB is ISyncList)
                {
                    return SyncListEquals((ISyncList)proxyA, (ISyncList)proxyB, contextA, contextB);
                }

                string strA = contextA.Resolver.ResolveObjectValue(objA);
                string strB = contextB.Resolver.ResolveObjectValue(objB);
                if (strA != null || strB != null)
                {
                    return strA == strB;
                }
            }

            return object.Equals(objA, objB);
        }

        private static bool SyncObjectEquals(ISyncObject objA, ISyncObject objB, SyncContext contextA, SyncContext contextB)
        {
            GetAllPropertySync syncA = new GetAllPropertySync(false);
            objA.Sync(syncA, contextA);

            GetAllPropertySync syncB = new GetAllPropertySync(false);
            objB.Sync(syncB, contextB);

            if (syncA.Values.Count != syncB.Values.Count)
            {
                return false;
            }

            SyncContext childContextA = contextA.CreateNew(objA);
            SyncContext childContextB = contextB.CreateNew(objB);

            foreach (var key in syncA.Values.Keys)
            {
                if (!syncB.Values.ContainsKey(key))
                {
                    return false;
                }

                object childObjA = syncA.Values[key].Value;
                object childObjB = syncB.Values[key].Value;

                if (!ObjectEquals(childObjA, childObjB, childContextA, childContextB))
                {
                    return false;
                }
            }

            return true;
        }
        private static bool SyncListEquals(ISyncList listA, ISyncList listB, SyncContext contextA, SyncContext contextB)
        {
            GetAllIndexSync syncA = new GetAllIndexSync();
            listA.Sync(syncA, contextA);

            GetAllIndexSync syncB = new GetAllIndexSync();
            listB.Sync(syncB, contextB);

            if (syncA.Values.Count != syncB.Values.Count)
            {
                return false;
            }

            SyncContext childContextA = contextA.CreateNew(listA);
            SyncContext childContextB = contextB.CreateNew(listB);

            for (int i = 0; i < syncA.Values.Count; i++)
            {
                object childObjA = syncA.Values[i].Value;
                object childObjB = syncB.Values[i].Value;

                if (!ObjectEquals(childObjA, childObjB, childContextA, childContextB))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
