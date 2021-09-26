// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Suity.Networking
{
    public abstract class NetworkServer : SystemObject
    {
        public event EventHandler<NetworkSessionEventArgs> SessionOpened;
        public event EventHandler<NetworkSessionEventArgs> SessionClosed;


        protected virtual void OnSessionOpened(NetworkSession session, string reason)
        {
            SessionOpened?.Invoke(this, new NetworkSessionEventArgs(session, reason));
        }
        protected virtual void OnSessionClosed(NetworkSession session, string reason)
        {
            SessionClosed?.Invoke(this, new NetworkSessionEventArgs(session, reason));
        }

    }
}
