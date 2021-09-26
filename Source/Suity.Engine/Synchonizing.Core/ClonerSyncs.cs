// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using Suity.Collections;
using Suity.Helpers;
using Suity.Synchonizing.Preset;

namespace Suity.Synchonizing.Core
{
    delegate object CloneElementCreate(Type type, object parameter);
    delegate void CloneElementClone(object objFrom, object objTo);

    class ClonePropertySync : MarshalByRefObject, IPropertySync
    {
        private readonly Dictionary<string, SyncValueInfo> _values;

        public CloneElementCreate Creater;
        public CloneElementClone Cloner;

        public ClonePropertySync(Dictionary<string, SyncValueInfo> values)
        {
            _values = values;
        }


        public SyncMode Mode => SyncMode.SetAll;

        public SyncIntent Intent => SyncIntent.Clone;

        public string Name => null;

        public IEnumerable<string> Names => _values.Keys.Where(str => !str.StartsWith("@"));

        public object Value => null;

        public T Sync<T>(string name, T obj, SyncFlag flag = SyncFlag.None, T defaultValue = default(T))
        {
            bool readOnly = (flag & SyncFlag.ReadOnly) == SyncFlag.ReadOnly;
            bool byRef = (flag & SyncFlag.ByRef) == SyncFlag.ByRef;
            bool notNull = (flag & SyncFlag.NotNull) == SyncFlag.NotNull;

            if ((flag & SyncFlag.AttributeMode) == SyncFlag.AttributeMode)
            {
                name = "@" + name;
            }

            SyncValueInfo info = _values.GetValueOrDefault(name);
            if (info == null) return readOnly || notNull ? obj : defaultValue;

            object resultObj = null;

            if (readOnly)
            {
                resultObj = obj;
            }
            else
            {
                if (byRef)
                {
                    resultObj = info.Value;
                }
                else
                {
                    resultObj = info.Value != null ? Creater(info.Value.GetType(), info.Value) : null;
                }
            }

            if (resultObj is T)
            {
                if (!byRef)
                {
                    Cloner(info.Value, resultObj);
                }
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
    }

    class CloneIndexSync : MarshalByRefObject, IIndexSync
    {
        readonly List<SyncValueInfo> _values;
        readonly Dictionary<string, string> _attributes;

        public CloneElementCreate Creater;
        public CloneElementClone Cloner;

        public CloneIndexSync(List<SyncValueInfo> values, Dictionary<string, string> attributes)
        {
            _values = values;
            _attributes = attributes;
        }

        public SyncMode Mode => SyncMode.SetAll;
        public SyncIntent Intent => SyncIntent.Clone;
        public int Count => _values.Count;
        public int Index => -1;
        public object Value => null;

        public int SyncCount(int count)
        {
            return _values.Count;
        }

        public T Sync<T>(int index, T obj, SyncFlag flag = SyncFlag.None)
        {
            bool readOnly = (flag & SyncFlag.ReadOnly) == SyncFlag.ReadOnly;
            bool byRef = (flag & SyncFlag.ByRef) == SyncFlag.ByRef;
            bool notNull = (flag & SyncFlag.NotNull) == SyncFlag.NotNull;

            SyncValueInfo info = _values.GetValueOrDefault(index);
            if (info == null) return readOnly || notNull ? obj : default(T);

            object result = null;

            if (readOnly)
            {
                result = obj;
            }
            else
            {
                if (byRef)
                {
                    result = info.Value;
                }
                else
                {
                    result = info.Value != null ? Creater(info.Value.GetType(), info.Value) : null;
                }
            }

            if (result is T)
            {
                if (!byRef)
                {
                    Cloner(info.Value, result);
                }
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
            if (!string.IsNullOrEmpty(name))
            {
                if (_attributes.TryGetValue(name, out string result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

    }
}
