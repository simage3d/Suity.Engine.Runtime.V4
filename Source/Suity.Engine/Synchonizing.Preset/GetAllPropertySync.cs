// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Preset
{
    public class GetAllPropertySync : MarshalByRefObject, IPropertySync
    {
        readonly public Dictionary<string, SyncValueInfo> Values = new Dictionary<string, SyncValueInfo>();
        readonly SyncIntent _intent;
        readonly bool _ignoreDefaultValue;

        public GetAllPropertySync(bool ignoreDefaultValue)
        {
            _intent = SyncIntent.Serialize;
            _ignoreDefaultValue = ignoreDefaultValue;
        }
        public GetAllPropertySync(SyncIntent intent, bool ignoreDefaultValue)
        {
            _intent = intent;
            _ignoreDefaultValue = ignoreDefaultValue;
        }


        public SyncMode Mode => SyncMode.GetAll;
        public SyncIntent Intent => _intent;
        public string Name => null;
        public IEnumerable<string> Names => Values.Keys;
        public object Value => null;

        public T Sync<T>(string name, T obj, SyncFlag flag = SyncFlag.None, T defaultValue = default(T))
        {
            if ((flag & SyncFlag.AttributeMode) == SyncFlag.AttributeMode)
            {
                name = "@" + name;
            }

            if (_ignoreDefaultValue && object.Equals(obj, defaultValue))
            {
                //Do nothing
            }
            else
            {
                Values[name] = new SyncValueInfo(typeof(T), obj, flag);
            }
            
            return obj;
        }
    }
}
