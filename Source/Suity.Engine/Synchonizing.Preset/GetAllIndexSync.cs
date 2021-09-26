// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Preset
{
    public class GetAllIndexSync : MarshalByRefObject, IIndexSync
    {
        readonly public List<SyncValueInfo> Values = new List<SyncValueInfo>();
        readonly public Dictionary<string, string> Attributes = new Dictionary<string, string>();

        readonly SyncIntent _intent;

        public GetAllIndexSync()
        {
            _intent = SyncIntent.Serialize;
        }
        public GetAllIndexSync(SyncIntent intent)
        {
            _intent = intent;
        }

        public SyncMode Mode => SyncMode.GetAll;
        public SyncIntent Intent => _intent;
        public int Count => Values.Count;
        public int Index => -1;
        public object Value { get; private set; }

        public int SyncCount(int count)
        {
            Value = count;
            return count;
        }
        public T Sync<T>(int index, T obj, SyncFlag flag = SyncFlag.None)
        {
            while (Values.Count <= index)
            {
                Values.Add(null);
            }
            Values[index] = new SyncValueInfo(typeof(T), obj, flag);
            return obj;
        }
        public string SyncAttribute(string name, string value)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Attributes[name] = value;
            }
            return value;
        }
    }
}
