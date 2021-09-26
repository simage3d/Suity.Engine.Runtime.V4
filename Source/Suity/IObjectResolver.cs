// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    /// <summary>
    /// 对象解算器
    /// </summary>
    public interface IObjectResolver
    {
        IEnumerable<string> GetPropertyNames(object obj);
        object GetProperty(object obj, string propertyName);
        void SetProperty(object obj, string propertyName, object value);
        bool ObjectEquals(object objA, object objB);
        object Clone(object source);
    }
}
