// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using Suity.Collections;
using Suity.Json;

namespace Suity
{
    /// <summary>
    /// 数据层
    /// </summary>
    [MultiThreadSecurity(MultiThreadSecurityMethods.LockedSecure)]
    public class DataLayer : RuntimeObject
    {
        readonly object _sync = new object();
        readonly Dictionary<string, DataCollection> _collectionsByKey = new Dictionary<string, DataCollection>();
        readonly Dictionary<string, DataCollection> _collectionsByTableId = new Dictionary<string, DataCollection>();
        readonly Dictionary<Type, Dictionary<string, object>> _datasByDataId = new Dictionary<Type, Dictionary<string, object>>();
        readonly Dictionary<Type, Dictionary<string, object>> _datasByKey = new Dictionary<Type, Dictionary<string, object>>();
        readonly Dictionary<object, DataObject> _dataObjectLookBack = new Dictionary<object, DataObject>();

        internal bool _inStorage;

        public IEnumerable<DataCollection> Collections => _collectionsByKey.Values.Select(o => o);
        public DataCollection GetCollection(string idOrKey)
        {
            if (string.IsNullOrEmpty(idOrKey))
            {
                return null;
            }

            lock (_sync)
            {
                if (_collectionsByTableId.TryGetValue(idOrKey, out DataCollection collection))
                {
                    collection.MarkAccess();
                    return collection;
                }
                if (_collectionsByKey.TryGetValue(idOrKey, out collection))
                {
                    collection.MarkAccess();
                    return collection;
                }

                return null;
            }
        }
        public IEnumerable<RawStringData> GetRawData(string idOrKey)
        {
            return GetCollection(idOrKey)?.RawDatas;
        }
        public object GetObject(string idOrKey, Type type)
        {
            if (string.IsNullOrEmpty(idOrKey))
            {
                return null;
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            lock (_sync)
            {
                if (type == typeof(string))
                {
                    return idOrKey;
                }

                var dic = _datasByDataId.GetValueOrDefault(type);
                if (dic != null)
                {
                    if (dic.TryGetValue(idOrKey, out object obj))
                    {
                        Logs.AddResourceLog(idOrKey, null);
                        return obj;
                    }
                }

                dic = _datasByKey.GetValueOrDefault(type);
                if (dic != null)
                {
                    if (dic.TryGetValue(idOrKey, out object obj))
                    {
                        Logs.AddResourceLog(idOrKey, null);
                        return obj;
                    }
                }

                if (_dataObjectLookBack.TryGetValue(idOrKey, out DataObject dataObj))
                {
                    object obj = dataObj.GetObject(type);
                    if (obj != null)
                    {
                        Logs.AddResourceLog(idOrKey, null);
                        return obj;
                    }
                }

                if (DataStorage.ContainsRawDataResolver(type))
                {
                    DataCollection collection = _collectionsByTableId.GetValueOrDefault(idOrKey);
                    if (collection == null)
                    {
                        collection = _collectionsByKey.GetValueOrDefault(idOrKey);
                    }

                    if (collection != null)
                    {
                        var result = collection.RawDatas
                            .Select(data => DataStorage.CreateObjectFromRawData(idOrKey, data, type))
                            .FirstOrDefault(r => r != null && type.IsAssignableFrom(r.GetType()));

                        if (result != null)
                        {
                            Logs.AddResourceLog(idOrKey, null);
                            return result;
                        }
                    }
                }
            }

            return null;
        }
        public T GetObject<T>(string idOrKey) where T : class
        {
            if (string.IsNullOrEmpty(idOrKey))
            {
                return default(T);
            }

            return GetObject(idOrKey, typeof(T)) as T;
        }
        public IEnumerable<T> GetObjects<T>() where T : class
        {
            lock (_sync)
            {
                Dictionary<string, object> dicKey = _datasByKey.GetValueOrDefault(typeof(T));
                if (dicKey != null)
                {
                    return dicKey.Values.OfType<T>();
                }
                else
                {
                    return EmptyArray<T>.Empty;
                }
            }
        }
        public IEnumerable<string> GetKeys<T>()
        {
            lock (_sync)
            {
                Dictionary<string, object> dicKey = _datasByKey.GetValueOrDefault(typeof(T));
                if (dicKey != null)
                {
                    return dicKey.Keys;
                }
                else
                {
                    return EmptyArray<string>.Empty;
                }
            }
        }
        public IEnumerable<string> GetDataIds<T>()
        {
            lock (_sync)
            {
                Dictionary<string, object> dicDataId = _datasByDataId.GetValueOrDefault(typeof(T));
                if (dicDataId != null)
                {
                    return dicDataId.Keys;
                }
                else
                {
                    return EmptyArray<string>.Empty;
                }
            }
        }
        public string GetKey(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is string str)
            {
                return str;
            }

            lock (_sync)
            {
                if (_dataObjectLookBack.TryGetValue(obj, out DataObject data))
                {
                    return data.Key;
                }
                else
                {
                    return null;
                }
            }
        }
        public string GetDataId(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is string dataId)
            {
                return dataId;
            }

            lock (_sync)
            {
                if (_dataObjectLookBack.TryGetValue(obj, out DataObject data))
                {
                    return data.DataId ?? data.Key;
                }
                else
                {
                    return null;
                }
            }
        }
        public DataObject GetDataObject(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            lock (_sync)
            {
                if (_dataObjectLookBack.TryGetValue(obj, out DataObject data))
                {
                    data.MarkAccess();
                    return data;
                }
                else
                {
                    return null;
                }
            }
        }


        public void AddCollection(DataCollection collection, DataConflictMode mode = DataConflictMode.Default)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }


            lock (_sync)
            {
                if (collection.Layer != null)
                {
                    throw new ArgumentException("Collection is in another data layer", nameof(collection));
                }
                collection.Layer = this;

                if (!string.IsNullOrEmpty(collection.Key))
                {
                    switch (mode)
                    {
                        case DataConflictMode.Override:
                        case DataConflictMode.Default:
                            _collectionsByKey[collection.Key] = collection;
                            break;
                        case DataConflictMode.Ignore:
                            if (!_collectionsByKey.ContainsKey(collection.Key))
                            {
                                _collectionsByKey.Add(collection.Key, collection);
                            }
                            break;
                        case DataConflictMode.Throw:
                            _collectionsByKey.Add(collection.Key, collection);
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(collection.TableId))
                {
                    switch (mode)
                    {
                        case DataConflictMode.Override:
                        case DataConflictMode.Default:
                            _collectionsByTableId[collection.TableId] = collection;
                            break;
                        case DataConflictMode.Ignore:
                            if (!_collectionsByTableId.ContainsKey(collection.TableId))
                            {
                                _collectionsByTableId.Add(collection.TableId, collection);
                            }
                            break;
                        case DataConflictMode.Throw:
                            _collectionsByTableId.Add(collection.TableId, collection);
                            break;
                        default:
                            break;
                    }
                }

                foreach (var dataObj in collection.DataObjects)
                {
                    AddDataObject(dataObj, mode);
                }
                if (!string.IsNullOrEmpty(collection.Key))
                {
                    ObjectType.RegisterAssetImplement(collection.Key, typeof(DataCollection), collection);
                }
            }

            if (DataStorage.LogDataLoading) 
            {
                Logs.LogInfo($"Data collection added : {collection.TableId} data count = {collection.Count}");
            }
        }
        private void AddDataObject(DataObject data, DataConflictMode mode = DataConflictMode.Default)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (data.Collection == null)
            {
                throw new ArgumentException("Data collection not exist", nameof(data));
            }

            string key = data.Key;
            string dataId = data.DataId;

            lock (_sync)
            {
                _dataObjectLookBack[key] = data;
                _dataObjectLookBack[dataId] = data;

                foreach (var comp in data.Components)
                {
                    RegisterObject(key, dataId, comp.ComponentType, comp.ComponentObject, mode);

                    if (comp.ComponentObject != null)
                    {
                        _dataObjectLookBack[comp.ComponentObject] = data;
                    }
                }
            }
        }
        internal void RegisterObject(string key, string dataId, Type type, object obj, DataConflictMode mode = DataConflictMode.Default)
        {
            lock (_sync)
            {
                var dicKey = _datasByKey.GetValueOrCreate(type, () => new Dictionary<string, object>());
                var dicDataId = _datasByDataId.GetValueOrCreate(type, () => new Dictionary<string, object>());

                switch (mode)
                {
                    case DataConflictMode.Default:
                    case DataConflictMode.Override:
                        if (!string.IsNullOrEmpty(key))
                        {
                            dicKey[key] = obj;
                        }
                        if (!string.IsNullOrEmpty(dataId))
                        {
                            dicDataId[dataId] = obj;
                        }
                        break;
                    case DataConflictMode.Ignore:
                        if (!string.IsNullOrEmpty(key) && !dicKey.ContainsKey(key))
                        {
                            dicKey.Add(key, obj);
                        }
                        if (!string.IsNullOrEmpty(dataId) && !dicDataId.ContainsKey(dataId))
                        {
                            dicDataId.Add(dataId, obj);
                        }
                        break;
                    case DataConflictMode.Throw:
                        if (!string.IsNullOrEmpty(key))
                        {
                            dicKey.Add(key, obj);
                        }
                        if (!string.IsNullOrEmpty(dataId))
                        {
                            dicDataId.Add(dataId, obj);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void Load(IDataReader reader, DataConflictMode mode = DataConflictMode.Default)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            foreach (var collectionReader in reader.Nodes("DataCollection"))
            {
                try
                {
                    LoadCollection(collectionReader, mode);
                }
                catch (Exception err)
                {
                    err.LogError("Load collection failed.");
                }
            }
        }
        public void Load(string json, DataConflictMode mode = DataConflictMode.Default)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var reader = JsonDataReader.Create(json);
            Load(reader, mode);
        }
        public DataCollection LoadCollection(IDataReader reader, DataConflictMode mode = DataConflictMode.Default)
        {
            DataCollection collection = DataCollection.LoadReader(reader);
            AddCollection(collection, mode);
            return collection;
        }
        public DataCollection LoadCollection(string json, DataConflictMode mode = DataConflictMode.Default)
        {
            DataCollection collection = DataCollection.LoadJson(json);
            AddCollection(collection, mode);
            return collection;
        }


        public void Clear()
        {
            lock (_sync)
            {
                foreach (var collection in _collectionsByKey.Values)
                {
                    collection.Layer = null;
                    ObjectType.UnregisterAssetImplement(collection.Key, typeof(DataCollection));
                }
                _collectionsByKey.Clear();
                _collectionsByTableId.Clear();
                _datasByKey.Clear();
                _datasByDataId.Clear();
                _dataObjectLookBack.Clear();
            }
        }
    }
}
