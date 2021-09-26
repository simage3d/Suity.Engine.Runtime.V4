// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    public class PackageEventArgs : EventArgs
    {
        public object Package { get; }

        public PackageEventArgs(object package)
        {
            Package = package;
        }
    }

    public class ChannelPackageEventArgs : PackageEventArgs
    {
        public int Channel { get; }

        public ChannelPackageEventArgs(object package, int channel)
            : base(package)
        {
            Channel = channel;
        }
    }

    public class NetworkLogEventArgs : EventArgs
    {
        public LogMessageType Type { get; }
        public NetworkDirection Direction { get; }
        public string SessionId { get; }
        public string ChannelId { get; }
        public object Message { get; }

        public NetworkLogEventArgs(LogMessageType type, NetworkDirection direction, string sessionId, string channelId, object message)
        {
            Type = type;
            Direction = direction;
            SessionId = sessionId;
            ChannelId = channelId;
            Message = message;
        }
    }

}
