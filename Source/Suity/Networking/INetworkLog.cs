// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    public interface INetworkLog
    {
        void AddNetworkLog(LogMessageType type, NetworkDirection direction, string sessionId, string channelId, object message);
    }

    public sealed class EmptyNetworkLog : INetworkLog
    {
        public static readonly EmptyNetworkLog Empty = new EmptyNetworkLog();

        private EmptyNetworkLog() { }

        public void AddNetworkLog(LogMessageType type, NetworkDirection direction, string sessionId, string channelId, object message)
        {
        }
    }
}
