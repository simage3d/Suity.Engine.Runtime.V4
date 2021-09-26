// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using System;
using System.Collections.Generic;

namespace Suity
{
    // TODO DataStorage 未能做到LockedSecure
    /// <summary>
    /// 数据储存
    /// </summary>
    [MultiThreadSecurity(MultiThreadSecurityMethods.LockedSecure)]
    public static class DataStorage
    {
        static readonly DataLayer _defaultLayer = new DataLayer();
        static DataLayer _dataLayer;
        static readonly Stack<DataLayer> _stack = new Stack<DataLayer>();
        static readonly Dictionary<Type, RawStringDataResolver> _rawDataResolvers = new Dictionary<Type, RawStringDataResolver>();
        static readonly Dictionary<Type, Func<string, object>> _factory = new Dictionary<Type, Func<string, object>>();

        public static DataLayer CurrentLayer { get { return _dataLayer; } }
        public static DataLayer DefaultLayer { get { return _defaultLayer; } }

        static DataStorage()
        {
            _defaultLayer._inStorage = true;
            _dataLayer = _defaultLayer;
        }

        public static DataCollection GetCollection(string idOrKey)
        {
            DataCollection collection = _dataLayer.GetCollection(idOrKey);
            if (collection != null)
            {
                return collection;
            }
            if (_stack.Count > 0)
            {
                foreach (var layer in _stack)
                {
                    collection = layer.GetCollection(idOrKey);
                    if (collection != null)
                    {
                        return collection;
                    }
                }
            }
            return null;
        }
        public static IEnumerable<RawStringData> GetRawData(string idOrKey)
        {
            return GetCollection(idOrKey)?.RawDatas;
        }
        public static object GetObject(string idOrKey, Type type)
        {
            if (string.IsNullOrEmpty(idOrKey))
            {
                return null;
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            object obj = _dataLayer.GetObject(idOrKey, type);
            if (obj != null)
            {
                return obj;
            }
            if (_stack.Count > 0)
            {
                foreach (var layer in _stack)
                {
                    obj = layer.GetObject(idOrKey, type);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }

            return _factory.GetValueOrDefault(type)?.Invoke(idOrKey);
        }
        public static T GetObject<T>(string idOrKey) where T : class
        {
            if (string.IsNullOrEmpty(idOrKey))
            {
                return default(T);
            }

            T obj = _dataLayer.GetObject<T>(idOrKey);
            if (obj != null)
            {
                return obj;
            }
            if (_stack.Count > 0)
            {
                foreach (var layer in _stack)
                {
                    obj = layer.GetObject<T>(idOrKey);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }

            return _factory.GetValueOrDefault(typeof(T))?.Invoke(idOrKey) as T;
        }
        public static IEnumerable<T> GetObjects<T>() where T : class
        {
            foreach (T obj in _dataLayer.GetObjects<T>())
            {
                yield return obj;
            }
            if (_stack.Count > 0)
            {
                foreach (var layer in _stack)
                {
                    foreach (T obj in layer.GetObjects<T>())
                    {
                        yield return obj;
                    }
                }
            }
        }
        public static IEnumerable<string> GetKeys<T>()
        {
            foreach (string key in _dataLayer.GetKeys<T>())
            {
                yield return key;
            }
            if (_stack.Count > 0)
            {
                foreach (var layer in _stack)
                {
                    foreach (string key in layer.GetKeys<T>())
                    {
                        yield return key;
                    }
                }
            }
        }
        public static IEnumerable<string> GetDataIds<T>()
        {
            foreach (string id in _dataLayer.GetDataIds<T>())
            {
                yield return id;
            }
            if (_stack.Count > 0)
            {
                foreach (var layer in _stack)
                {
                    foreach (string id in layer.GetDataIds<T>())
                    {
                        yield return id;
                    }
                }
            }
        }
        public static string GetKey(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is string str)
            {
                return str;
            }

            string key = _dataLayer.GetKey(obj);
            if (key != null)
            {
                return key;
            }
            if (_stack.Count > 0)
            {
                foreach (var layer in _stack)
                {
                    key = layer.GetKey(obj);
                    if (key != null)
                    {
                        return key;
                    }
                }
            }
            return null;
        }
        public static string GetDataId(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is string dataId)
            {
                return dataId;
            }

            string id = _dataLayer.GetDataId(obj);
            if (id != null)
            {
                return id;
            }
            if (_stack.Count > 0)
            {
                foreach (var layer in _stack)
                {
                    id = layer.GetDataId(obj);
                    if (id != null)
                    {
                        return id;
                    }
                }
            }
            return null;
        }
        public static DataObject GetDataObject(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            DataObject dataObject = _dataLayer.GetDataObject(obj);
            if (dataObject != null)
            {
                return dataObject;
            }
            if (_stack.Count > 0)
            {
                foreach (var layer in _stack)
                {
                    dataObject = layer.GetDataObject(obj);
                    if (dataObject != null)
                    {
                        return dataObject;
                    }
                }
            }
            return null;
        }


        public static void AddCollection(DataCollection collection, DataConflictMode mode = DataConflictMode.Default)
        {
            _dataLayer.AddCollection(collection, mode);
        }
        public static void Load(IDataReader reader, DataConflictMode mode = DataConflictMode.Default)
        {
            _dataLayer.Load(reader, mode);
        }
        public static void Load(string json, DataConflictMode mode = DataConflictMode.Default)
        {
            _dataLayer.Load(json, mode);
        }
        public static DataCollection LoadCollection(IDataReader reader, DataConflictMode mode = DataConflictMode.Default)
        {
            return _dataLayer.LoadCollection(reader, mode);
        }
        public static DataCollection LoadCollection(string json, DataConflictMode mode = DataConflictMode.Default)
        {
            return _dataLayer.LoadCollection(json, mode);
        }

        public static void Clear(bool clearDefaultLayer = false)
        {
            while (true)
            {
                DataLayer layer = PopDataLayer();
                if (layer != null)
                {
                    layer.Clear();
                }
                else
                {
                    break;
                }
            }

            if (clearDefaultLayer)
            {
                _dataLayer.Clear();
            }
        }

        public static void PushDataLayer()
        {
            PushDataLayer(new DataLayer());
        }
        public static void PushDataLayer(DataLayer dataLayer)
        {
            if (dataLayer == null)
            {
                throw new ArgumentNullException(nameof(dataLayer));
            }
            if (dataLayer._inStorage)
            {
                throw new ArgumentException("Data layer is already in storage.", nameof(dataLayer));
            }

            _stack.Push(_dataLayer);
            _dataLayer = dataLayer;
            _dataLayer._inStorage = true;
        }
        public static DataLayer PopDataLayer()
        {
            if (_stack.Count > 0)
            {
                DataLayer popped = _dataLayer;
                _dataLayer = _stack.Pop();
                popped._inStorage = false;
                return popped;
            }
            else
            {
                return null;
            }
        }


        public static void RegisterRawDataResolver<T>(RawStringDataResolve<T> creation)
        {
            if (creation == null)
            {
                throw new ArgumentNullException(nameof(creation));
            }

            _rawDataResolvers.Add(typeof(T), new RawStringDataResolver<T>(creation));
        }
        public static void RegisterDataFactory<T>(Func<string, T> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _factory[typeof(T)] = (string name) => factory(name);
        }
        public static void AddDefaultObject<T>(string key, string dataId, T obj, DataConflictMode mode = DataConflictMode.Default) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key can not be empty", nameof(key));
            }

            if (string.IsNullOrEmpty(dataId))
            {
                throw new ArgumentException("dataId can not be empty", nameof(dataId));
            }

            _defaultLayer.RegisterObject(key, dataId, typeof(T), obj, mode);
        }

        public static object CreateObjectFromRawData(string idOrKey, RawStringData data, Type type)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            RawStringDataResolver resolver = _rawDataResolvers.GetValueOrDefault(type);
            if (resolver != null)
            {
                return resolver.CreateObject(idOrKey, data);
            }

            return null;
        }
        public static T CreateObjectFromRawData<T>(string idOrKey, RawStringData data) where T : class
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (_rawDataResolvers.GetValueOrDefault(typeof(T)) is RawStringDataResolver<T> resolver)
            {
                return resolver.Create(idOrKey, data);
            }

            return null;
        }
        public static bool ContainsRawDataResolver(Type type)
        {
            return _rawDataResolvers.ContainsKey(type);
        }
    }
}
