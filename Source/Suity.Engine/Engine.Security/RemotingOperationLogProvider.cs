// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using Suity.Networking;
using Suity.Synchonizing;


namespace Suity.Engine.Security
{
    public class RemotingOperationLogProvider : BaseRemotingProvider<INetworkOperationLog, RemotingOperationLogProvider.Proxy>
    {
        public override string Icon => "*CoreIcon|Log";

        public class Proxy : MarshalByRefObject, INetworkOperationLog
        {
            readonly INetworkOperationLog _inner;


            public Proxy()
            {
                _inner = Suity.Environment.GetService<INetworkOperationLog>();
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }


            #region INetworkOperationLog
            public void AddOperationLog(int level, string category, string userId, string ip, string data, bool successful)
            {
                _inner?.AddOperationLog(level, category, userId, ip, data, successful);
            } 
            #endregion
        }
    }
}
