// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    public interface IBehaviorLog
    {
        void LogSessionConnected(NetworkSession session);
        void LogSessionClosed(NetworkSession session, string reason);
        void LogIncomingPackage(NetworkSession session, object package, int channel);
        void LogOutGoingPackage(NetworkSession session, object package, int channel);
    }

    public class EmptyBehaviorLog : IBehaviorLog
    {
        public readonly static EmptyBehaviorLog Empty = new EmptyBehaviorLog();

        public void LogIncomingPackage(NetworkSession session, object package, int channel)
        {
        }

        public void LogOutGoingPackage(NetworkSession session, object package, int channel)
        {
        }

        public void LogSessionClosed(NetworkSession session, string reason)
        {
        }

        public void LogSessionConnected(NetworkSession session)
        {
        }
    }
}
