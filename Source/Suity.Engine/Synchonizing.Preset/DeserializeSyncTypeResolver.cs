// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Engine;
using Suity.Helpers;
using Suity.Synchonizing.Core;

namespace Suity.Synchonizing.Preset
{
    public class DeserializeSyncTypeResolver : ISyncTypeResolver
    {
        public static readonly DeserializeSyncTypeResolver Instance = new DeserializeSyncTypeResolver();

        public DeserializeSyncTypeResolver()
        {
        }


        public Type ResolveType(string typeName, string parameter)
        {
            switch (typeName)
            {
                case "DataRef":
                case "Enum":
                    return typeof(string);
                default:
                    return null;
            }
        }

        public string ResolveTypeName(Type type, object obj)
        {
            return null;
        }

        public object ResolveObject(string typeName, string parameter)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            switch (typeName)
            {
                case "DataRef":
                case "Enum":
                    return parameter ?? string.Empty; //强制转换String.Empty否则视为解析失败
            }

            if (typeName.StartsWith("Object:"))
            {
                string subTypeName = typeName.RemoveFromFirst(7);
                try
                {
                    object obj = ObjectType.CreateObject(subTypeName);
                    if (obj != null)
                    {
                        //RemoteHelpers.FillInitialStruct(obj);
                        return obj;
                    }
                }
                catch (Exception)
                {
                }
            }

            return null;
        }

        public string ResolveObjectValue(object obj)
        {
            return null;
        }

        public object CreateProxy(object obj)
        {
            if (obj != null && ObjectType.GetClassTypeInfo(obj.GetType()) != null)
            {
                return new Proxy(obj);
            }
            return null;
        }

        class Proxy : ISyncObject
        {
            readonly object _obj;
            readonly string _typeName;

            public Proxy(object obj)
            {
                _obj = obj ?? throw new ArgumentNullException(nameof(obj));
                _typeName = ObjectType.GetClassTypeInfo(_obj.GetType())?.Key;
            }

            public void Sync(IPropertySync sync, ISyncContext context)
            {
                if (_obj == null)
                {
                    return;
                }
                if (string.IsNullOrEmpty(_typeName))
                {
                    return;
                }
                if (sync.Mode != SyncMode.SetAll)
                {
                    return;
                }

                foreach (var name in sync.Names)
                {
                    object value = ObjectType.GetProperty(_obj, name);
                    if (value is System.Collections.IList)
                    {
                        sync.Sync(name, value, SyncFlag.ReadOnly);
                    }
                    else
                    {
                        value = sync.Sync(name, value);
                        ObjectType.SetProperty(_obj, name, value);
                    }
                }
            }
        }

    }
}
