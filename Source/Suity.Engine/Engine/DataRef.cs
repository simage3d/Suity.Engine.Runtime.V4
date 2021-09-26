// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Engine
{
    public abstract class DataRef
    {
        string _key;

        public string Key
        {
            get => _key;
            set => _key = value;
        }
        public abstract Type DataType { get; }

        public DataRef()
        {
        }
        public DataRef(string key)
        {
            Key = key;
        }
        public object Value => DataStorage.GetObject(Key, DataType);
        public string GetTypeName()
        {
            return ObjectType.GetClassTypeInfo(DataType)?.Name ?? DataType?.Name;
        }
        public string GetFullTypeName()
        {
            return ObjectType.GetClassTypeInfo(DataType)?.FullName ?? DataType?.FullName;
        }
        public override string ToString()
        {
            return Value?.ToString() ?? Key;
        }

        public static DataRef Create(Type dataType, string key = null)
        {
            Type type = typeof(DataRef<>).MakeGenericType(new Type[] { dataType });
            DataRef dataRef = (DataRef)Activator.CreateInstance(type);
            dataRef.Key = key;
            return dataRef;
        }
    }
    public sealed class DataRef<T> : DataRef where T :class
    {
        public override Type DataType => typeof(T);

        public new T Value => DataStorage.GetObject<T>(Key);

        public DataRef()
        {
        }
        public DataRef(string key)
            : base(key)
        {
        }
    }
}
