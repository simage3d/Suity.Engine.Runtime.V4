// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Suity.Collections
{
    public class ActionQueue
    {
        readonly List<Action> _actions = new List<Action>();

        public void QueueAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }

            lock (_actions)
            {
                _actions.Add(action);
            }
        }

        public void Update()
        {
            lock (_actions)
            {
                if (_actions.Count > 0)
                {
                    var actions = _actions.ToArray();
                    _actions.Clear();

                    foreach (var action in actions)
                    {
                        action();
                    }
                }
            }
        }
    }
}
