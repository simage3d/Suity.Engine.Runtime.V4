// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Helpers;
using Suity.Reflecting;

namespace Suity.Synchonizing.Core
{
    public class DefaultSyncTypeResolver : ISyncTypeResolver
    {
        public static readonly DefaultSyncTypeResolver Instance = new DefaultSyncTypeResolver();

        public string ResolveTypeName(Type type, object obj)
        {
            if (obj != null)
            {
                type = obj.GetType();
            }
            return type.GetTypeId();
        }

        public Type ResolveType(string typeName, string parameter)
        {
            return typeName.ResolveType();
        }
        public object ResolveObject(string typeName, string parameter)
        {
            return null;
        }
        public string ResolveObjectValue(object obj)
        {
            return null;
        }
        public object CreateProxy(object obj)
        {
            return null;
        }

    }
}
