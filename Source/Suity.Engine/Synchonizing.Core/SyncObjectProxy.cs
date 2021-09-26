// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Core
{
    public abstract class SyncObjectProxy : ISyncObject
    {
        public object Target { get; internal set; }

        public T TargetAs<T>() where T : class
        {
            return (T)Target;
        }

        public virtual object CreateNew()
        {
            return null;
        }

        public virtual void Sync(IPropertySync sync, ISyncContext context)
        {
        }

        public virtual SyncObjectProxy Clone()
        {
            return (SyncObjectProxy)Activator.CreateInstance(this.GetType());
        }
    }
}
