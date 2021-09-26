// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Synchonizing
{
    public interface IIndexSyncOperation
    {
        Type GetElementType(IIndexSync sync);

        int Count { get; }

        object GetItem(IIndexSync sync, int index);

        void SetItem(IIndexSync sync, int index, object value);

        void Insert(IIndexSync sync, int index, object value);

        void RemoveAt(IIndexSync sync, int index);

        void Clear(IIndexSync sync);

        object CreateNew(IIndexSync sync);
    }
}
