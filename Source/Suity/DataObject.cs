// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Suity
{
    internal class DataComponent
    {
        public readonly Type ComponentType;
        public readonly object ComponentObject;

        public DataComponent(Type type, object obj)
        {
            ComponentType = type;
            ComponentObject = obj;
        }
    }

    /// <summary>
    /// 数据对象
    /// </summary>
    public sealed class DataObject : Suity.ResourceObject
    {
        internal DataCollection _collection;

        readonly string _localId;
        readonly List<DataComponent> _components = new List<DataComponent>();

        internal string _dataId;

        /// <summary>
        /// 所属集合
        /// </summary>
        public DataCollection Collection => _collection;
        public string DataId => _dataId;
        public string LocalId => _localId;

        public DataObject(string key, string localId)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key is empty", nameof(key));
            }

            Key = key;
            _dataId = _localId = localId;
        }
        protected override string GetName()
        {
            return DataId;
        }

        public void Add(Type type, object obj)
        {
            if (_collection != null)
            {
                throw new InvalidOperationException("DataObject is initialized and can not be modified.");
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (!type.IsAssignableFrom(obj.GetType()))
            {
                throw new ArgumentException("obj is not assignable from type", nameof(obj));
            }
            _components.Add(new DataComponent(type, obj));
        }
        public void Add(object obj)
        {
            if (_collection != null)
            {
                throw new InvalidOperationException("DataObject is initialized and can not be modified.");
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            _components.Add(new DataComponent(obj.GetType(), obj));
        }

        public object GetObject(Type type)
        {
            if (type == null)
            {
                return null;
            }

            foreach (var item in _components)
            {
                if (type.IsAssignableFrom(item.ComponentType))
                {
                    return item.ComponentObject;
                }
            }
            return null;
        }
        public T GetObject<T>() where T : class
        {
            foreach (var item in _components)
            {
                if (typeof(T).IsAssignableFrom(item.ComponentType))
                {
                    return item.ComponentObject as T;
                }
            }
            return null;
        }


        internal int Count { get { return _components.Count; } }
        internal IEnumerable<DataComponent> Components { get { return _components; } }
        internal DataComponent this[int index]
        {
            get { return _components[index]; }
        }

        public override string ToString()
        {
            return DataId ?? Key ?? base.ToString();
        }
    }

    public class RawStringData
    {
        public string Name { get; }
        public string Data { get; }
        public RawStringData(string name, string data)
        {
            Name = name;
            Data = data;
        }
    }

    public delegate T RawStringDataResolve<T>(string idOrKey, RawStringData data);

    abstract class RawStringDataResolver
    {
        public abstract Type DataType { get; }
        public abstract object CreateObject(string idOrKey, RawStringData data);
    }

    class RawStringDataResolver<T> : RawStringDataResolver
    {
        readonly RawStringDataResolve<T> _creation;

        public override Type DataType => typeof(T);

        public RawStringDataResolver(RawStringDataResolve<T> creation)
        {
            _creation = creation ?? throw new ArgumentNullException(nameof(creation));
        }

        public override object CreateObject(string idOrKey, RawStringData data)
        {
            return _creation(idOrKey, data);
        }

        public T Create(string idOrKey, RawStringData data)
        {
            return _creation(idOrKey, data);
        }
    }
}
