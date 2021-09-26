using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    public class NetworkSessionEventArgs : EventArgs
    {
        public NetworkSession Session { get; }
        public string Reason { get; }

        public NetworkSessionEventArgs(NetworkSession session, string reason)
        {
            Session = session;
            Reason = reason;
        }
    }
}
