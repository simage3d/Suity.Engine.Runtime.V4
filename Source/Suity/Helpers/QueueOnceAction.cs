using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Helpers
{
    public sealed class QueueOnceAction
    {
        readonly Action _action;
        bool _inQueue;

        public QueueOnceAction(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void DoAction()
        {
            _action();
        }

        public void DoQueuedAction()
        {
            lock (_action)
            {
                if (_inQueue)
                {
                    return;
                }
                _inQueue = true;
                QueuedAction.Do(() =>
                {
                    _inQueue = false;
                    _action();
                });
            }
        }

    }
}
