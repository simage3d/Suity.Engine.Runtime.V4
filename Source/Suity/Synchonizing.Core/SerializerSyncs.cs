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
    delegate object DeserializeElementCreate(Type type, INodeReader reader);

    delegate void DeserializeElementDeserialize(object obj, INodeReader reader);

    class DeserializePropertySync : MarshalByRefObject, IPropertySync
    {
        readonly private INodeReader _reader;

        public DeserializeElementCreate Creater;
        public DeserializeElementDeserialize Deserializer;

        public DeserializePropertySync(INodeReader reader)
        {
            _reader = reader;
        }
        public SyncMode Mode => SyncMode.SetAll;
        public SyncIntent Intent => SyncIntent.Serialize;

        public string Name { get { return default(string); } }
        public IEnumerable<string> Names => _reader.NodeNames;
        public object Value { get; internal set; }
        public T Sync<T>(string name, T obj, SyncFlag flag = SyncFlag.None, T defaultValue = default(T))
        {
            if ((flag & SyncFlag.NoSerialize) == SyncFlag.NoSerialize)
            {
                return obj;
            }

            bool readOnly = (flag & SyncFlag.ReadOnly) == SyncFlag.ReadOnly;
            bool notNull = (flag & SyncFlag.NotNull) == SyncFlag.NotNull;
            bool attrMode = (flag & SyncFlag.AttributeMode) == SyncFlag.AttributeMode;


            if (!attrMode)
            {
                INodeReader subReader = _reader.Node(name);
                if (!subReader.Exist)
                {
                    return (readOnly || notNull) ? obj : defaultValue;
                }

                object resultObj = readOnly ? obj : Creater(typeof(T), subReader);

                if (resultObj is T)
                {
                    Deserializer(resultObj, subReader);

                    return (T)resultObj;
                }
                else
                {
                    T noResult = readOnly ? obj : defaultValue;
                    if (noResult == null && notNull)
                    {
                        noResult = obj;
                    }

                    return noResult;
                }
            }
            else
            {
                if (typeof(T) != typeof(string))
                {
                    throw new InvalidOperationException("SyncFlag.AttributeMode supports String type only.");
                }
                object resultObj = readOnly ? (object)obj : _reader.GetAttribute(name);

                if (resultObj is T t)
                {
                    return t;
                }
                else
                {
                    T noResult = readOnly ? obj : defaultValue;
                    if (noResult == null && notNull)
                    {
                        noResult = obj;
                    }

                    return noResult;
                }

            }
        }
    }

    class DeserializeIndexSync : MarshalByRefObject, IIndexSync
    {
        readonly private INodeReader _reader;

        public DeserializeElementCreate Creater;
        public DeserializeElementDeserialize Deserializer;

        public DeserializeIndexSync(INodeReader reader)
        {
            _reader = reader;
        }

        public SyncMode Mode => SyncMode.SetAll;
        public SyncIntent Intent => SyncIntent.Serialize;

        public int Count => _reader.ChildCount;
        public int Index => -1;
        public object Value { get; internal set; }
        public int SyncCount(int count)
        {
            return _reader.ChildCount;
        }
        public T Sync<T>(int index, T obj, SyncFlag flag = SyncFlag.None)
        {
            bool readOnly = (flag & SyncFlag.ReadOnly) == SyncFlag.ReadOnly;
            bool notNull = (flag & SyncFlag.NotNull) == SyncFlag.NotNull;

            INodeReader subReader = _reader.Node(index);
            if (!subReader.Exist)
            {
                return (readOnly || notNull) ? obj : default(T);
            }

            object result = readOnly ? obj : Creater(typeof(T), subReader);

            if (result is T)
            {
                Deserializer(result, subReader);

                return (T)result;
            }
            else
            {
                T noResult = readOnly ? obj : default(T);
                if (noResult == null && notNull)
                {
                    noResult = obj;
                }

                return noResult;
            }
        }
        public string SyncAttribute(string name, string value)
        {
            return _reader.GetAttribute(name);
        }
    }

}
