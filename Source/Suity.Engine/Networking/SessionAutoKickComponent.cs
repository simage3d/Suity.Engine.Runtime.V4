using Suity.Engine;
using Suity.Synchonizing;
using Suity.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    public class SessionAutoKickComponent : NodeComponent, IViewObject
    {
        public int TimeOutDurationSec { get; set; } = 180;
        public int AliveCheckDurationSec { get; set; } = 60;

        readonly NetworkSessionKeepAliveManager _aliveManager = new NetworkSessionKeepAliveManager();

        public SessionAutoKickComponent()
        {
            _aliveManager.AutoCloseTimeOutSession = true;
        }

        public override string Icon => "*CoreIcon|Network";

        protected override void OnStart()
        {
            base.OnStart();

            StartTimer(OnTimer, TimeSpan.FromSeconds(AliveCheckDurationSec), TimeSpan.FromSeconds(AliveCheckDurationSec));
        }

        public NetworkSessionKeepAliveManager AliveManager => _aliveManager;

        public void Sync(IPropertySync sync, ISyncContext context)
        {
            TimeOutDurationSec = sync.Sync(nameof(TimeOutDurationSec), TimeOutDurationSec);
            AliveCheckDurationSec = sync.Sync(nameof(AliveCheckDurationSec), AliveCheckDurationSec);
            sync.Sync("SessionCount", _aliveManager.Count, SyncFlag.ReadOnly);

            if (sync.IsSetter())
            {
                if (TimeOutDurationSec < 5)
                {
                    TimeOutDurationSec = 5;
                }
                if (AliveCheckDurationSec < 3)
                {
                    AliveCheckDurationSec = 3;
                }

                _aliveManager.KeepAliveDuration = TimeSpan.FromSeconds(TimeOutDurationSec);
            }
        }

        public void SetupView(IViewObjectSetup setup)
        {
            setup.InspectorField(TimeOutDurationSec, new ViewProperty(nameof(TimeOutDurationSec)));
            setup.InspectorField(AliveCheckDurationSec, new ViewProperty(nameof(AliveCheckDurationSec)));

            if (this.ParentObject != null)
            {
                setup.InspectorField(_aliveManager.Count, new ViewProperty("SessionCount").WithReadOnly());
            }
        }



        [NodeTrigger(NetworkConfigs.Event_SessionConnected)]
        private void OnSessionAdded(NetworkSession session)
        {
            _aliveManager.AddSession(session);
        }

        [NodeTrigger(NetworkConfigs.Event_SessionClosed)]
        private void OnSessionRemoved(NetworkSession session)
        {
            _aliveManager.RemoveSession(session);
        }

        private void OnTimer()
        {
            _aliveManager.AliveCheck();
        }

    }
}
