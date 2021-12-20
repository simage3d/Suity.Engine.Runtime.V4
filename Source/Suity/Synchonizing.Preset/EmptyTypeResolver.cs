// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Synchonizing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Preset
{
    public class EmptyTypeResolver : ISyncTypeResolver
    {
        public static readonly EmptyTypeResolver Instance = new EmptyTypeResolver();

        public string ResolveTypeName(Type type, object obj)
        {
            return null;
        }
        public Type ResolveType(string typeName, string parameter)
        {
            return null;
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
