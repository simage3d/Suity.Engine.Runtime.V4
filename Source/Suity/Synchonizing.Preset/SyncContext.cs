// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Synchonizing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Preset
{
    public class SyncContext : MarshalByRefObject, ISyncContext
    {
        public static readonly SyncContext Empty = new SyncContext();

        readonly object _parent;
        readonly ISyncTypeResolver _resolver;
        readonly IServiceProvider _provider;

        public object Parent { get { return _parent; } }

        public ISyncTypeResolver Resolver { get { return _resolver; } }

        public IServiceProvider Provider { get { return _provider; } }


        private SyncContext()
        {

        }

        public SyncContext(object parent)
        {
            _parent = parent;
        }
        public SyncContext(object parent, ISyncTypeResolver resolver, IServiceProvider provider)
        {
            _parent = parent;
            _resolver = resolver;
            _provider = provider;
        }

        public object GetService(Type serviceType)
        {
            return _provider?.GetService(serviceType);
        }

        // TODO: 是否可以取消Parent机制？
        internal SyncContext CreateNew(object newParent)
        {
            return new SyncContext(newParent, _resolver, _provider);
        }
    }
}
