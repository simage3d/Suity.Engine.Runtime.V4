// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Core
{
    public interface ISyncTypeResolver
    {
        string ResolveTypeName(Type type, object obj);
        Type ResolveType(string typeName, string parameter);
        object ResolveObject(string typeName, string parameter);
        string ResolveObjectValue(object obj);


        /// <summary>
        /// 为对象创建Sync代理
        /// </summary>
        /// <param name="obj">请求的对象</param>
        /// <returns>返回的对象可以是ISyncObject, ISyncList</returns>
        object CreateProxy(object obj);
    }
}
