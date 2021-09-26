﻿// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Synchonizing.Core;

namespace Suity.Synchonizing.Preset
{
    public sealed class SingleIndexSync : MarshalByRefObject, IIndexSync
    {
        private readonly SyncMode _mode;
        private readonly int _index;
        private object _value;

        private SingleIndexSync(SyncMode mode, int index, object value)
        {
            _mode = mode;
            _index = index;
            _value = value;
        }


        public SyncMode Mode => _mode;
        public SyncIntent Intent => SyncIntent.View;

        public int Index => _index;
        public object Value => _value;
        public int Count => 0;


        public T Sync<T>(int index, T obj, SyncFlag flag = SyncFlag.None)
        {
            switch (_mode)
            {
                case SyncMode.RequestElementType:
                    _value = obj;
                    return obj;
                case SyncMode.Get:
                    if (index == _index)
                    {
                        _value = obj;
                    }
                    return obj;
                case SyncMode.Set:
                case SyncMode.Insert:
                    if (index == _index)
                    {
                        if ((flag & SyncFlag.ReadOnly) == SyncFlag.ReadOnly)
                        {
                            if (_value != null && obj != null)
                            {
                                Cloner.CloneProperty((T)_value, obj);
                            }
                            return obj;
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
                case SyncMode.CreateNew:
                    _value = obj;
                    return obj;
                default:
                    return obj;
            }
        }
        public string SyncAttribute(string name, string value)
        {
            return value;
        }



        public static SingleIndexSync CreateElementTypeGetter()
        {
            return new SingleIndexSync(SyncMode.RequestElementType, 0, null);
        }
        /// <summary>
        /// 创建读取同步
        /// </summary>
        /// <param name="index">索引</param>
        public static SingleIndexSync CreateGetter(int index)
        {
            return new SingleIndexSync(SyncMode.Get, index, null);
        }
        /// <summary>
        /// 创建写入同步
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">写入的值</param>
        public static SingleIndexSync CreateSetter(int index, object value)
        {
            return new SingleIndexSync(SyncMode.Set, index, value);
        }
        public static SingleIndexSync CreateInserter(int index, object value)
        {
            return new SingleIndexSync(SyncMode.Insert, index, value);
        }
        public static SingleIndexSync CreateRemover(int index)
        {
            return new SingleIndexSync(SyncMode.RemoveAt, index, null);
        }
        public static SingleIndexSync CreateActivator(string parameter)
        {
            return new SingleIndexSync(SyncMode.CreateNew, 0, parameter);
        }

    }
}
