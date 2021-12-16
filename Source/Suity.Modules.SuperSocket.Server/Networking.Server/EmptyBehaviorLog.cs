using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Networking.Server
{
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
