// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    [Obsolete]
    public interface IDataStorage<T> where T : class
    {
        T GetData(long id);

        bool SetData(T data);
    }


    public interface IDataStorage<TKey, TValue> where TValue : class
    {
        TValue GetData(TKey id);

        bool SetData(TValue data);
    }
}
