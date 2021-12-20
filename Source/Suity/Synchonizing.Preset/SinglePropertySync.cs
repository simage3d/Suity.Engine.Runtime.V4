// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Synchonizing.Core;

namespace Suity.Synchonizing.Preset
{
    public sealed class SinglePropertySync : MarshalByRefObject, IPropertySync
    {
        private readonly SyncMode _mode;
        private readonly string _name;
        private object _value;
        private Type _baseType;
        private SyncFlag _flag;

        private SinglePropertySync(SyncMode mode, string name, object value)
        {
            _mode = mode;
            _name = name;
            _value = value;
        }


        public SyncMode Mode => _mode;
        public SyncIntent Intent => SyncIntent.View;

        public string Name => _name;
        public IEnumerable<string> Names { get { yield return _name; } }
        public object Value => _value;
        public Type ValueBaseType => _baseType;
        public SyncFlag Flag => _flag;


        public T Sync<T>(string name, T obj, SyncFlag flag = SyncFlag.None, T defaultValue = default(T))
        {
            if ((flag & SyncFlag.AttributeMode) == SyncFlag.AttributeMode)
            {
                name = "@" + name;
            }

            if (_mode == SyncMode.Get)
            {
                if (name == _name)
                {
                    _value = obj;
                    _baseType = typeof(T);
                    _flag = flag;
                    //string null 保护
                    if (_baseType == typeof(string) && _value == null)
                    {
                        _value = string.Empty;
                    }
                }
                return obj;
            }
            else if (_mode == SyncMode.Set)
            {
                if (name == _name)
                {
                    _baseType = typeof(T);
                    _flag = flag;

                    if ((flag & SyncFlag.ReadOnly) == SyncFlag.ReadOnly)
                    {
                        if (_value != null && _value is T)
                        {
                            Cloner.CloneProperty((T)_value, obj);
                            return obj;
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                    else
                    {
                        T result = _value is T ? (T)_value : default(T);
                        if (result == null && (flag & SyncFlag.NotNull) == SyncFlag.NotNull)
                        {
                            result = obj;
                        }
                        return result;
                    }
                }
                else
                {
                    return obj;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// 创建读取同步
        /// </summary>
        /// <param name="name">属性名</param>
        public static SinglePropertySync CreateGetter(string name)
        {
            return new SinglePropertySync(SyncMode.Get, name, null);
        }
        /// <summary>
        /// 创建写入同步
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="value">写入的值</param>
        public static SinglePropertySync CreateSetter(string name, object value)
        {
            return new SinglePropertySync(SyncMode.Set, name, value);
        }

    }
}
