// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using Suity.Collections;
using Suity.Helpers;
using Suity.Json;

namespace Suity
{
    /// <summary>
    /// 数据集合
    /// </summary>
    [MultiThreadSecurity(MultiThreadSecurityMethods.ReadonlySecure)]
    [AssetDefinitionType(AssetDefinitionCodes.DataFamily)]
    public class DataCollection : Suity.ResourceObject
    {
        readonly Dictionary<string, DataObject> _dataDic = new Dictionary<string, DataObject>();
        readonly List<DataObject> _dataList = new List<DataObject>();
        List<RawStringData> _rawDataList;


        public DataCollection(string key, string tableId = null)
        {
            Key = key;
            TableId = tableId;
        }
        public DataCollection(IDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            ReadData(reader);
        }


        public DataLayer Layer { get; internal set; }
        public string TableId { get; private set; }
        public IEnumerable<RawStringData> RawDatas
        {
            get
            {
                if (_rawDataList != null)
                {
                    return _rawDataList.Select(o => o);
                }
                else
                {
                    return EmptyArray<RawStringData>.Empty;
                }
            }
        }


        protected override string GetName()
        {
            return TableId;
        }

        private void ReadData(IDataReader reader)
        {
            Key = reader.Node("Key").ReadString();
            TableId = reader.Node("TableId").ReadString();

            foreach (var dataReader in reader.Nodes("Data"))
            {
                try
                {
                    string key = dataReader.Node("Key").ReadString();
                    string localId = dataReader.Node("LocalId").ReadString();
                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }

                    var rawData = dataReader.Node("RawData").ReadString();
                    if (rawData != null)
                    {
                        (_rawDataList ?? (_rawDataList = new List<RawStringData>())).Add(new RawStringData(key, rawData));
                    }

                    DataObject data = null;
                    foreach (var compReader in dataReader.Nodes("Component"))
                    {
                        try
                        {
                            if (ObjectType.TryReadObject(compReader, out string compTypeName, out object comp, out bool compIsArray) && comp != null)
                            {
                                (data ?? (data = new DataObject(key, localId))).Add(comp.GetType(), comp);
                            }
                        }
                        catch (Exception e)
                        {
                            throw new DataException($"Data read failed. Key:{Key} TableId:{TableId} Key:{key}", e);
                        }
                    }
                    if (data != null)
                    {
                        AddDataObject(data);
                    }
                }
                catch (Exception e2)
                {
                    Logs.LogError(e2);
                }
            }
        }

        public void AddDataObject(DataObject data)
        {
            if (Layer != null)
            {
                throw new InvalidOperationException("DataCollection is initialized and can not be modified");
            }
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (data._collection != null)
            {
                throw new InvalidOperationException("DataObject is in another container");
            }
            string key = data.Key;
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key is empty");
            }
            if (_dataDic.ContainsKey(key))
            {
                throw new ArgumentException("Key exists : " + key);
            }

            data._collection = this;

            _dataDic[key] = data;
            _dataList.Add(data);

            if (!string.IsNullOrEmpty(TableId))
            {
                data._dataId = $"{TableId}.{data.LocalId}";
            }
        }

        public int Count { get { return _dataList.Count; } }
        public string GetKeyAt(int index)
        {
            return _dataList.GetListItemSafe(index)?.Key;
        }
        public string GetDataIdAt(int index)
        {
            return _dataList.GetListItemSafe(index)?.DataId;
        }

        public T GetObject<T>(string key) where T : class
        {
            if (_dataDic.TryGetValue(key, out DataObject data))
            {
                return data.GetObject<T>();
            }
            else
            {
                return null;
            }
        }
        public T GetObject<T>(int index) where T : class
        {
            DataObject data = _dataList.GetListItemSafe(index);

            if (data != null)
            {
                return data.GetObject<T>();
            }

            return null;
        }
        public bool ContainsKey(string key)
        {
            return _dataDic.ContainsKey(key);
        }
        public IEnumerable<string> Keys { get { return _dataList.Select(o => o.Key); } }
        public IEnumerable<string> DataIds { get { return _dataList.Select(o => o.DataId); } }
        public IEnumerable<DataObject> DataObjects { get { return _dataList.Select(o => o); } }
        public IEnumerable<T> GetObjects<T>() where T : class
        {
            return _dataList.Select(o => o.GetObject<T>()).OfType<T>();
        }

        public void WriteTo(IDataWriter writer)
        {
            writer.Node("Asset").WriteString(Key);
            var datasWriter = writer.Nodes("Data", _dataList.Count);
            foreach (var data in _dataList)
            {
                var dataWriter = datasWriter.Item();
                dataWriter.Node("Key").WriteString(data.Key);

                var compsWriter = dataWriter.Nodes("Component", data.Count);
                foreach (var comp in data.Components)
                {
                    ObjectType.WriteObject(compsWriter.Item(), comp.ComponentObject);
                }
                compsWriter.Finish();
            }
            datasWriter.Finish();
        }


        public static DataCollection LoadJson(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var reader = JsonDataReader.Create(json);
            return new DataCollection(reader);
        }
        public static DataCollection LoadReader(IDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return new DataCollection(reader);
        }

        public override string ToString()
        {
            return TableId ?? Key ?? base.ToString();
        }
    }
}
