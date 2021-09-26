// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Suity.Controlling
{
    public class TriggerCollection : Suity.Object
    {
        readonly Dictionary<string, Trigger> _triggers = new Dictionary<string, Trigger>();
        readonly Dictionary<string, HashSet<Trigger>> _triggersByEvent = new Dictionary<string, HashSet<Trigger>>();

        public TriggerCollection()
        {
        }

        public void AddTrigger(Trigger trigger)
        {
            if (trigger == null)
            {
                throw new ArgumentNullException();
            }
            if (_triggers.ContainsKey(trigger.Name))
            {
                throw new InvalidOperationException();
            }

            trigger._collection = this;
            _triggers.Add(trigger.Name, trigger);

            foreach (string eventKey in trigger.Events)
            {
                EnsureTriggerSet(eventKey).Add(trigger);
            }
        }

        public Trigger GetTrigger(string key)
        {
            if (_triggers.TryGetValue(key, out Trigger trigger))
            {
                return trigger;
            }
            else
            {
                return null;
            }
        }

        public void DoAction(FunctionContext context)
        {
            if (context == null) 
			{
			    throw new ArgumentNullException();
			}

            if (string.IsNullOrEmpty(context.EventKey))
            {
                return;
            }

            if (_triggersByEvent.TryGetValue(context.EventKey, out HashSet<Trigger> triggers))
            {
                foreach (Trigger trigger in triggers)
                {
                    trigger.DoAction(context);
                }
            }
        }

        public IEnumerable<string> HandledEvents
        {
            get { return _triggersByEvent.Keys.Select(key => key); }
        }

        private HashSet<Trigger> EnsureTriggerSet(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
            {
                throw new ArgumentNullException();
            }

            if (!_triggersByEvent.TryGetValue(eventKey, out HashSet<Trigger> triggerSet))
            {
                triggerSet = new HashSet<Trigger>();
                _triggersByEvent.Add(eventKey, triggerSet);
            }
            return triggerSet;
        }
    }
}
