// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    public abstract class NetworkSender : Suity.Object
    {
        public NetworkRequest Request { get; private set; }

        readonly Dictionary<Type, Action<object>> _receivers = new Dictionary<Type, Action<object>>();

        public void SetRequester(NetworkRequest request)
        {
            if (request == Request)
            {
                return;
            }

            if (Request != null)
            {
                Request.PackageReceived -= Request_PackageReceived;
            }

            Request = request;

            if (Request != null)
            {
                Request.PackageReceived += Request_PackageReceived;
            }
        }


        private void Request_PackageReceived(object sender, PackageEventArgs e)
        {
            if (_receivers.TryGetValue(e.Package.GetType(), out Action<object> receive))
            {
                receive(e.Package);
            }
        }


        protected void RegisterReceiver(Type type, Action<object> action)
        {
            _receivers.Add(type, action);
        }
    }
}
