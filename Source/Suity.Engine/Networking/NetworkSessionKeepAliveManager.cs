using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    [MultiThreadSecurity(MultiThreadSecurityMethods.LockedSecure)]
    public class NetworkSessionKeepAliveManager
    {
        public TimeSpan KeepAliveDuration { get; set; } = TimeSpan.FromSeconds(120);
        public bool AutoCloseTimeOutSession { get; set; } = true;

        HashSet<NetworkSession> _sessions = new HashSet<NetworkSession>();

        public event EventHandler<NetworkSessionEventArgs> SessionTimeOut;


        public NetworkSessionKeepAliveManager()
        {
        }

        public bool AddSession(NetworkSession session)
        {
            if (session == null)
            {
                return false;
            }
            if (session.KeepAlive != KeepAliveModes.KeepAlive)
            {
                return false;
            }

            lock (_sessions)
            {
                return _sessions.Add(session);
            }
        }
        public bool RemoveSession(NetworkSession session)
        {
            if (session == null)
            {
                return false;
            }

            lock (_sessions)
            {
                return _sessions.Remove(session);
            }
        }

        public int Count => _sessions.Count;

        public void Clear()
        {
            lock (_sessions)
            {
                _sessions.Clear();
            }
        }

        public void AliveCheck()
        {
            List<NetworkSession> removes = null;

            TimeSpan duration = KeepAliveDuration;
            DateTime now = DateTime.UtcNow;

            lock (_sessions)
            {
                foreach (var session in _sessions)
                {
                    if (!session.Connected || now - session.LastActiveTime > duration)
                    {
                        (removes ?? (removes = new List<NetworkSession>())).Add(session);
                    }
                }

                if (removes != null)
                {
                    foreach (var session in removes)
                    {
                        _sessions.Remove(session);
                    }
                }
            }

            if (removes != null)
            {
                foreach (var session in removes)
                {
                    OnSessionTimeOut(session);
                }
            }
        }

        protected virtual void OnSessionTimeOut(NetworkSession session)
        {
            if (AutoCloseTimeOutSession)
            {
                session.Close();
            }

            SessionTimeOut?.Invoke(this, new NetworkSessionEventArgs(session, "Time out"));
        }
    }
}
