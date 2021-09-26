// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Synchonizing
{
#if BRIDGE
    public class EmptySyncContext : ISyncContext
#else
    public class EmptySyncContext : MarshalByRefObject, ISyncContext
#endif
    {
        public static readonly EmptySyncContext Empty = new EmptySyncContext();

        public object Parent => null;

        public object GetService(Type serviceType) => null;
    }
}
