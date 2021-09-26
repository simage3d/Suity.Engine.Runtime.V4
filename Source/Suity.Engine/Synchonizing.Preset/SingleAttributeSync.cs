// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Preset
{
    public sealed class SingleAttributeSync : MarshalByRefObject, IIndexSync
    {
        private readonly SyncMode _mode;
        private readonly string _name;
        private string _value;


        private SingleAttributeSync(SyncMode mode, string name, string value)
        {
            _mode = mode;
            _name = name;
            _value = value;
        }


        public SyncMode Mode => _mode;
        public SyncIntent Intent => SyncIntent.View;

        public int Count => 0;

        public int Index => 0;

        public object Value => null;

        public int SyncCount(int count)
        {
            return count;
        }

        public T Sync<T>(int index, T obj, SyncFlag flag = SyncFlag.None)
        {
            return obj;
        }

        public string SyncAttribute(string name, string value)
        {
            if (_mode == SyncMode.Get)
            {
                if (name == _name)
                {
                    _value = value;
                }
                return value;
            }
            else if (_mode == SyncMode.Set)
            {
                if (name == _name)
                {
                    return _value;
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return value;
            }
        }


        /// <summary>
        /// 创建读取同步
        /// </summary>
        /// <param name="index">索引</param>
        public static SingleAttributeSync CreateGetter(string name)
        {
            return new SingleAttributeSync(SyncMode.Get, name, null);
        }
        /// <summary>
        /// 创建写入同步
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">写入的值</param>
        public static SingleAttributeSync CreateSetter(string name, string value)
        {
            return new SingleAttributeSync(SyncMode.Set, name, value);
        }
    }
}
