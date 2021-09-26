// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Core
{
    public abstract class SyncListProxy : ISyncList
    {
        public abstract int Count { get; }

        public object Target { get; internal set; }

        public virtual object CreateNew()
        {
            return null;
        }
        public virtual void Sync(IIndexSync sync, ISyncContext context)
        {
        }

        public virtual SyncListProxy Clone()
        {
            return (SyncListProxy)Activator.CreateInstance(this.GetType());
        }
    }
}
