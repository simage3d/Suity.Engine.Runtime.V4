// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Preset
{
    public sealed class EmptySyncList : ISyncList
    {
        public static readonly EmptySyncList Empty = new EmptySyncList();

        private EmptySyncList()
        {
        }

        public int Count => 0;

        public void Sync(IIndexSync sync, ISyncContext context)
        {
        }
    }
}
