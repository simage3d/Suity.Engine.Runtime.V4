// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Core
{
    class GenericListProxy<T> : SyncListProxy
    {
        public override int Count
        {
            get
            {
                return ((IList<T>)Target).Count;
            }
        }

        public override void Sync(IIndexSync sync, ISyncContext context)
        {
            IList<T> list = (IList<T>)Target;
            sync.SyncGenericIList<T>(list);
        }
    }
}
