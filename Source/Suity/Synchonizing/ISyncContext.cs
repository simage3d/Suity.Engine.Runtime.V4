﻿// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Synchonizing
{
    public interface ISyncContext : IServiceProvider
    {
        object Parent { get; }
    }
}
