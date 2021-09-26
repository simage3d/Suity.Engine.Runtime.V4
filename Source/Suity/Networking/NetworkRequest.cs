// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    [MultiThreadSecurity(MultiThreadSecurityMethods.LockedSecure)]
    public abstract class NetworkRequest : Suity.Object
    {
        public event EventHandler<PackageEventArgs> PackageReceived;

        public abstract bool Push(object obj, NetworkDeliveryMethods method, int channel);
        public IFuture Send(object obj, NetworkDeliveryMethods method, int channel)
        {
            return Send<object, object>(obj, method, channel);
        }
        public abstract IFuture<TResult> Send<TRequest, TResult>(TRequest obj, NetworkDeliveryMethods method, int channel);


        protected virtual void OnPackageReceived(object package)
        {
            PackageReceived?.Invoke(this, new PackageEventArgs(package));
        }
    }
}
