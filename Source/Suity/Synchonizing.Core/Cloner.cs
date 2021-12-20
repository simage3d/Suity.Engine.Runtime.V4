// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.NodeQuery;
using Suity.Synchonizing.Preset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Core
{
    public static class Cloner
    {
        public static object Clone(object source)
        {
            return Clone<object>(source, null, null);
        }
        public static object Clone(object source, ISyncTypeResolver resolver, IServiceProvider provider)
        {
            return Clone<object>(source, resolver, provider);
        }
        public static T Clone<T>(T source)
        {
            return Clone<T>(source, null, null);
        }
        public static T Clone<T>(T source, ISyncTypeResolver resolver, IServiceProvider provider)
        {
            if (source == null) return default(T);

            if (source.GetType().IsValueType || source is string)
            {
                return source;
            }

            object objClone = null;

            if (source is ISerializeAsString serializeAsString)
            {
                return (T)SyncTypeExtensions.CreateObject(source.GetType(), null, serializeAsString.Key, resolver, source as ISyncTypeResolver);
            }
            if (source is ISyncObject)
            {
                objClone = SyncTypeExtensions.CreateObject(source.GetType(), resolver, source as ISyncTypeResolver);
            }
            else if (source is ISyncList)
            {
                objClone = SyncTypeExtensions.CreateObject(source.GetType(), resolver, source as ISyncTypeResolver);
            }
            else if (SyncTypeExtensions.GetObjectProxyType(source) != null)
            {
                objClone = SyncTypeExtensions.CreateObject(source.GetType(), resolver, SyncTypeExtensions.GetObjectProxyType(source) as ISyncTypeResolver);
            }
            else if (SyncTypeExtensions.GetListProxyType(source.GetType()) != null)
            {
                objClone = SyncTypeExtensions.CreateObject(source.GetType(), resolver, SyncTypeExtensions.GetListProxyType(source) as ISyncTypeResolver);
            }
            
            if (objClone != null)
            {
                CloneProperty(source, objClone, new SyncContext(null, resolver, provider), new SyncContext(null, resolver, provider));
                return (T)objClone;
            }
            else
            {
                return default(T);
            }
        }

        public static void CloneProperty(object objFrom, object objTo)
        {
            CloneProperty(objFrom, objTo, SyncContext.Empty, SyncContext.Empty);
        }
        public static void CloneProperty(object objFrom, object objTo, ISyncTypeResolver resolver, IServiceProvider provider)
        {
            SyncContext context = new SyncContext(null, resolver, provider);
            CloneProperty(objFrom, objTo, context, context);
        }

        private static void CloneProperty(object objFrom, object objTo, SyncContext contextFrom, SyncContext contextTo)
        {
            if (objFrom == null) throw new ArgumentNullException(nameof(objFrom));
            if (objTo == null) throw new ArgumentNullException(nameof(objTo));
            if (objFrom.GetType() != objTo.GetType()) throw new ArgumentException();


            if (objFrom.GetType().IsValueType || objFrom is string)
            {
                //Do nothing
            }
            else if (objFrom is ISyncObject syncObject)
            {
                ClonePropertySyncObject(syncObject, (ISyncObject)objTo, contextFrom, contextTo);
            }
            else if (objFrom is ISyncList syncList)
            {
                ClonePropertySyncList(syncList, (ISyncList)objTo, contextFrom, contextTo);
            }
            else if (SyncTypeExtensions.GetObjectProxyType(objFrom) != null)
            {
                ClonePropertySyncObject(SyncTypeExtensions.CreateObjectProxy(objFrom), SyncTypeExtensions.CreateObjectProxy(objTo), contextFrom, contextTo);
            }
            else if (SyncTypeExtensions.GetListProxyType(objFrom) != null)
            {
                ClonePropertySyncList(SyncTypeExtensions.CreateListProxy(objFrom), SyncTypeExtensions.CreateListProxy(objTo), contextFrom, contextTo);
            }
            else if (objFrom is RawNode rawFrom)
            {
                (objTo as RawNode)?.ClonePropertyFrom(rawFrom);
            }
            else if (contextFrom.Resolver != null && contextTo.Resolver != null)
            {
                object proxyFrom = contextFrom.Resolver.CreateProxy(objFrom);
                object proxyTo = contextTo.Resolver.CreateProxy(objTo);

                if (proxyFrom is ISyncObject && proxyTo is ISyncObject)
                {
                    ClonePropertySyncObject((ISyncObject)proxyFrom, (ISyncObject)proxyTo, contextFrom, contextTo);
                }
                else if (proxyFrom is ISyncList && proxyTo is ISyncList)
                {
                    ClonePropertySyncList((ISyncList)proxyFrom, (ISyncList)proxyTo, contextFrom, contextTo);
                }
            }
        }

        private static void ClonePropertySyncObject(ISyncObject objFrom, ISyncObject objTo, SyncContext contextFrom, SyncContext contextTo)
        {
            GetAllPropertySync getterSync = new GetAllPropertySync(SyncIntent.Clone, false);
            objFrom.Sync(getterSync, contextFrom);

            SyncContext elementContextFrom = contextFrom.CreateNew(objFrom);
            SyncContext elementContextTo = contextTo.CreateNew(objTo);

            ClonePropertySync setterSync = new ClonePropertySync(getterSync.Values)
            {
                Creater = (elementType, elementParam) =>
                {
                    return CreateObject(elementType, elementParam, contextTo.Resolver, objTo as ISyncTypeResolver);
                },
                Cloner = (elementObjFrom, elementObjTo) =>
                {
                    CloneProperty(elementObjFrom, elementObjTo, elementContextFrom, elementContextTo);
                }
            };

            objTo.Sync(setterSync, contextTo);
        }
        private static void ClonePropertySyncList(ISyncList objFrom, ISyncList objTo, SyncContext contextFrom, SyncContext contextTo)
        {
            GetAllIndexSync getterSync = new GetAllIndexSync(SyncIntent.Clone);
            objFrom.Sync(getterSync, contextFrom);

            SyncContext elementContextFrom = contextFrom.CreateNew(objFrom);
            SyncContext elementContextTo = contextTo.CreateNew(objTo);

            CloneIndexSync setterSync = new CloneIndexSync(getterSync.Values, getterSync.Attributes)
            {
                Creater = (elementType, elementParam) =>
                {
                    return CreateObject(elementType, elementParam, contextTo.Resolver, objTo as ISyncTypeResolver);
                },
                Cloner = (elementObjFrom, elementObjTo) =>
                {
                    CloneProperty(elementObjFrom, elementObjTo, elementContextFrom, elementContextTo);
                }
            };

            objTo.Sync(setterSync, contextTo);
        }

        private static object CreateObject(Type type, object parameter, ISyncTypeResolver callerResolver, ISyncTypeResolver localResolver)
        {
            if (type.IsValueType || type == typeof(string))
            {
                return parameter;
            }
            else
            {
                return SyncTypeExtensions.CreateObject(type, callerResolver, localResolver);
            }
        }
    }
}
