// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Collections;
using Suity.Engine;
using Suity.Helpers;
using Suity.Reflecting;
using Suity.Synchonizing.Preset;
using Suity.Views;

namespace Suity.Synchonizing.Core
{
    /// <summary>
    /// 同步工具
    /// </summary>
    public static class SyncTypeExtensions
    {
        internal static ISyncTypeResolver _globalResolver;
        public static void InitializeGlobalResolver(ISyncTypeResolver resolver)
        {
            if (_globalResolver != null)
            {
                throw new InvalidOperationException();
            }
            _globalResolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }


        private readonly static ConcurrentDictionary<Type, SyncObjectProxy> _objectProxyCache = new ConcurrentDictionary<Type, SyncObjectProxy>();
        private readonly static ConcurrentDictionary<Type, SyncListProxy> _listProxyCache = new ConcurrentDictionary<Type, SyncListProxy>();
        private readonly static Dictionary<Type, Func<string, object>> _valueResolvers = new Dictionary<Type, Func<string, object>>();


        static SyncTypeExtensions()
        {
            _valueResolvers[typeof(string)] = s => s;
            _valueResolvers[typeof(bool)] = s => Convert.ToBoolean(s);
            _valueResolvers[typeof(byte)] = s => Convert.ToByte(s);
            _valueResolvers[typeof(char)] = s => Convert.ToChar(s);
            _valueResolvers[typeof(decimal)] = s => Convert.ToDecimal(s);
            _valueResolvers[typeof(double)] = s => Convert.ToDouble(s);
            _valueResolvers[typeof(Int16)] = s => Convert.ToInt16(s);
            _valueResolvers[typeof(Int32)] = s => Convert.ToInt32(s);
            _valueResolvers[typeof(Int64)] = s => Convert.ToInt64(s);
            _valueResolvers[typeof(sbyte)] = s => Convert.ToSByte(s);
            _valueResolvers[typeof(Single)] = s => Convert.ToSingle(s);
            _valueResolvers[typeof(UInt16)] = s => Convert.ToUInt16(s);
            _valueResolvers[typeof(UInt32)] = s => Convert.ToUInt32(s);
            _valueResolvers[typeof(UInt64)] = s => Convert.ToUInt64(s);
            _valueResolvers[typeof(DateTime)] = s => Convert.ToDateTime(s);
            _valueResolvers[typeof(TimeSpan)] = s => 
            {
                if (TimeSpan.TryParse(s, out TimeSpan timeSpan))
                {
                    return timeSpan;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            };
            _valueResolvers[typeof(Guid)] = s => 
            {
                if (Guid.TryParseExact(s, "D", out Guid id))
                {
                    return id;
                }
                else
                {
                    return Guid.Empty;
                }
            };
            _valueResolvers[typeof(ButtonValue)] = s =>
            {
                if (ButtonValue.TryParse(s, out ButtonValue value))
                {
                    return value;
                }
                else
                {
                    return ButtonValue.Empty;
                }
            };
        }


        internal static void RegisterProxy(Type type, SyncObjectProxy proxy)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (proxy == null)
            {
                throw new ArgumentNullException(nameof(proxy));
            }

            _objectProxyCache[type] = proxy;
        }
        internal static void RegisterProxy(Type type, SyncListProxy proxy)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (proxy == null)
            {
                throw new ArgumentNullException(nameof(proxy));
            }

            _listProxyCache[type] = proxy;
        }
        internal static SyncObjectProxy GetObjectProxyType(object obj)
        {
            return GetObjectProxyType(obj.GetType());
        }
        internal static SyncObjectProxy GetObjectProxyType(Type editedType)
        {
            if (editedType == null)
            {
                return null;
            }

            if (_objectProxyCache.TryGetValue(editedType, out SyncObjectProxy proxy))
            {
                return proxy;
            }

            foreach (Type editorProxyType in typeof(SyncObjectProxy).GetDerivedTypes())
            {
                var assignment = editorProxyType.GetAttributesCached<SyncProxyUsageAttribute>().FirstOrDefault();
                if (assignment != null && assignment.ObjectType == editedType)
                {
                    proxy = (SyncObjectProxy)Activator.CreateInstance(editorProxyType);
                    _objectProxyCache[editedType] = proxy;
                    return proxy;
                }
            }

            if (editedType.IsGenericType)
            {
                var defType = editedType.GetGenericTypeDefinition();
                if (defType != null && defType != editedType)
                {
                    proxy = GetObjectProxyType(defType);
                    if (proxy != null)
                    {
                        _objectProxyCache[editedType] = proxy;
                        return proxy;
                    }
                }
            }

            _objectProxyCache[editedType] = null;

            return null;
        }
        internal static SyncObjectProxy CreateObjectProxy(object obj)
        {
            SyncObjectProxy proxy = GetObjectProxyType(obj);
            if (proxy == null)
            {
                proxy = WrapSyncObjectProxy(obj);
                if (proxy != null)
                {
                    RegisterProxy(obj.GetType(), proxy);
                }
            }

            if (proxy != null)
            {
                proxy = proxy.Clone();
                proxy.Target = obj;
                return proxy;
            }

            return null;
        }
        internal static SyncListProxy GetListProxyType(object list)
        {
            Type editedType = list.GetType();

            if (_listProxyCache.TryGetValue(editedType, out SyncListProxy proxy))
            {
                return proxy;
            }

            foreach (Type editorProxyType in typeof(SyncListProxy).GetDerivedTypes())
            {
                var assignment = editorProxyType.GetAttributesCached<SyncProxyUsageAttribute>().FirstOrDefault();
                if (assignment != null && assignment.ObjectType == editedType)
                {
                    proxy = (SyncListProxy)Activator.CreateInstance(editorProxyType);
                    _listProxyCache[editedType] = proxy;
                    return proxy;
                }
            }

            if (editedType.IsGenericType)
            {
                Type[] args = editedType.GetGenericArguments();
                if (args.Length == 1)
                {
                    Type iListType = typeof(IList<>).MakeGenericType(args);
                    if (iListType.IsAssignableFrom(editedType))
                    {
                        Type proxyType = typeof(GenericListProxy<>).MakeGenericType(args);
                        proxy = (SyncListProxy)Activator.CreateInstance(proxyType);
                        _listProxyCache[editedType] = proxy;
                        return proxy;
                    }
                }
            }

            return null;
        }
        internal static SyncListProxy CreateListProxy(object list)
        {
            SyncListProxy proxy = GetListProxyType(list);
            if (proxy != null)
            {
                proxy = proxy.Clone();
                proxy.Target = list;
                return proxy;
            }
            else
            {
                return null;
            }
        }

        public static ISyncObject GetSyncObject(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj is ISyncObject syncObject)
            {
                return syncObject;
            }
            else
            {
                return CreateObjectProxy(obj);
            }
        }
        public static ISyncList GetSyncList(object list)
        {
            if (list == null)
            {
                return null;
            }
            else if (list is ISyncList syncList)
            {
                return syncList;
            }
            else if (list is ISyncNode syncNode)
            {
                return syncNode.GetList();
            }
            else
            {
                return CreateListProxy(list);
            }
        }


        internal static object CreateObject(Type type, ISyncTypeResolver callerResolver, ISyncTypeResolver localResolver)
        {
            return CreateObject(type, null, null, callerResolver, localResolver);
        }
        internal static object CreateObject(Type type, string overrideTypeName, string content, ISyncTypeResolver callerResolver, ISyncTypeResolver localResolver)
        {
            Type defaultOverrideType = type;
            object obj = null;

            //检查覆盖类型
            if (!string.IsNullOrEmpty(overrideTypeName))
            {
                if (localResolver != null)
                {
                    obj = ResolveObject(overrideTypeName, content, localResolver);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
                if (callerResolver != null)
                {
                    obj = ResolveObject(overrideTypeName, content, callerResolver);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
                if (_globalResolver != null)
                {
                    obj = ResolveObject(overrideTypeName, content, _globalResolver);
                    if (obj != null)
                    {
                        return obj;
                    }
                }

                //默认解析器不尝试CreateObject
                defaultOverrideType = DefaultSyncTypeResolver.Instance.ResolveType(overrideTypeName, content);
                if (defaultOverrideType == null)
                {
                    throw new TypeResolveException($"Can not resolve type : {overrideTypeName} L:{localResolver?.GetType().Name} C:{callerResolver?.GetType().Name} G:{_globalResolver?.GetType().Name}");
                }
            }

            //各种通过content构建支持的类型
            if (_valueResolvers.TryGetValue(defaultOverrideType, out Func<string, object> func))
            {
                return func(content);
            }
            if (defaultOverrideType.IsEnum)
            {
                try
                {
                    obj = Enum.Parse(defaultOverrideType, content);
                }
                catch (Exception)
                {
                    Array enumValues = Enum.GetValues(defaultOverrideType);
                    obj = enumValues.Length > 0 ? enumValues.GetValue(0) : null;
                }
                return obj;
            }
            if (typeof(ISerializeAsString).IsAssignableFrom(defaultOverrideType))
            {
                var sas = (ISerializeAsString)Activator.CreateInstance(defaultOverrideType);
                if (sas != null)
                {
                    sas.Key = content;
                    return sas;
                }
                else
                {
                    return null;
                }
            }
            // 无类型定义
            if (defaultOverrideType == typeof(object))
            {
                return null;
            }
            // DBNull
            if (defaultOverrideType == typeof(DBNull))
            {
                return DBNull.Value;
            }

            if (localResolver != null)
            {
                string typeName = localResolver.ResolveTypeName(type, null);
                if (!string.IsNullOrEmpty(typeName))
                {
                    obj = ResolveObject(typeName, content, localResolver);
                    if (obj != null)
                    {
                        return obj;
                    }
                }

                //尝试让localResolver创建默认对象
                obj = localResolver.ResolveObject(null, content);
                if (obj != null)
                {
                    return obj;
                }

                Type defaultObjectType = localResolver.ResolveType(null, content);
                if (defaultObjectType != null)
                {
                    obj = Activator.CreateInstance(defaultObjectType);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }

            //如果存在内容，尝试1参数构建
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    obj = Activator.CreateInstance(defaultOverrideType, content);
                }
                catch (Exception)
                {
                }
            }

            if (obj == null)
            {
                try
                {
                    //尝试无参数构建
                    obj = Activator.CreateInstance(defaultOverrideType);
                }
                catch (Exception)
                {
                }
            }

            if (obj == null)
            {
                throw new TypeResolveException("Can not create object : " + defaultOverrideType.FullName);
            }
            return obj;
        }

        private static object ResolveObject(string typeName, string content, ISyncTypeResolver resolver)
        {
            object obj = resolver.ResolveObject(typeName, content);
            if (obj != null)
            {
                return obj;
            }

            Type objType = resolver.ResolveType(typeName, content);
            if (objType != null)
            {
                if (typeof(ISerializeAsString).IsAssignableFrom(objType))
                {
                    obj = Activator.CreateInstance(objType);
                    ((ISerializeAsString)obj).Key = content;
                }
                else if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        //尝试1参数构建
                        obj = Activator.CreateInstance(objType, content);
                    }
                    catch (Exception err)
                    {
                        Logs.LogError(err);
                    }
                }

                if (obj == null)
                {
                    try
                    {
                        //尝试无参数构建
                        obj = Activator.CreateInstance(objType);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return obj;
        }


        public static SyncObjectProxy WrapSyncObjectProxy(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var info = ObjectType.GetClassTypeInfo(obj.GetType());
            if (info != null)
            {
                return WrapSyncObjectProxy(info);
            }
            else
            {
                return null;
            }
        }
        public static SyncObjectProxy WrapSyncObjectProxy(this ClassTypeInfo info)
        {
            return new ObjectTypeSyncProxyWrapper(info.Exchanger);
        }

        class ObjectTypeSyncProxyWrapper : SyncObjectProxy
        {
            public readonly ObjectType.ExchangeDelegate Exchanger;

            public ObjectTypeSyncProxyWrapper(ObjectType.ExchangeDelegate exchanger)
            {
                Exchanger = exchanger ?? throw new ArgumentNullException(nameof(exchanger));
            }

            public override void Sync(IPropertySync sync, ISyncContext context)
            {
                Exchanger(Target, new ExchangeSync(sync));
            }

            public override SyncObjectProxy Clone()
            {
                return new ObjectTypeSyncProxyWrapper(Exchanger);
            }
        }
    }

    class ExchangeSync : IExchange
    {
        readonly IPropertySync _sync;

        public ExchangeSync(IPropertySync sync)
        {
            _sync = sync ?? throw new ArgumentNullException(nameof(sync));
        }

        public object Exchange(string name, object value)
        {
            return _sync.Sync(name, value);
        }
    }

    public class SyncPropertyResolver : IObjectResolver, IInitialize
    {
        public SyncPropertyResolver()
        {
            ObjectType.ObjectResolver = this;
        }


        public IEnumerable<string> GetPropertyNames(object obj)
        {
            var syncObj = SyncTypeExtensions.GetSyncObject(obj);
            if (syncObj != null)
            {
                GetAllPropertySync sync = new GetAllPropertySync(false);
                syncObj.Sync(sync, EmptySyncContext.Empty);
                return sync.Names;
            }
            else
            {
                return EmptyArray<string>.Empty;
            }
        }
        public object GetProperty(object obj, string propertyName)
        {
            var syncObj = SyncTypeExtensions.GetSyncObject(obj);
            return syncObj?.GetProperty(propertyName, EmptySyncContext.Empty);
        }
        public void SetProperty(object obj, string propertyName, object value)
        {
            var syncObj = SyncTypeExtensions.GetSyncObject(obj);
            syncObj?.SetProperty(propertyName, value, EmptySyncContext.Empty);
        }
        public bool ObjectEquals(object objA, object objB)
        {
            return Equality.ObjectEquals(objA, objB);
        }
        public object Clone(object source)
        {
            return Cloner.Clone(source);
        }
    }
}
