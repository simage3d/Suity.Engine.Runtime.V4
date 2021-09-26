// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using Suity.Networking;
using Suity.Helpers;

namespace Suity.Engine.Security
{
    public class RemotingOperationLogRequester : BaseRemotingRequester<INetworkOperationLog>, INetworkOperationLog
    {
        public override string Icon => "*CoreIcon|Log";

        #region INetworkOperationLog
        public void AddOperationLog(int level, string category, string userId, string ip, string data, bool successful)
        {
            Proxy?.AddOperationLog(level, category, userId, ip, data, successful);
        } 
        #endregion
    }
}
