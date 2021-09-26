﻿// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections.Generic;

namespace Suity.Synchonizing
{
    public interface IPropertySync
    {
        SyncMode Mode { get; }
        SyncIntent Intent { get; }
        string Name { get; }
        IEnumerable<string> Names { get; }
        object Value { get; }

        T Sync<T>(string name, T obj, SyncFlag flag = SyncFlag.None, T defaultValue = default(T));
    }
}
