// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Suity.Controlling
{
    public class Trigger : Suity.ResourceObject
    {
        private readonly string _name;
        private readonly HashSet<string> _events = new HashSet<string>();
        private readonly Action<FunctionContext> _action;
        
        internal TriggerCollection _collection;

        public Trigger(string name, Action<FunctionContext> action)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException();
            }

            _name = name;
            _action = action;
        }
        public Trigger(string name, string key, Action<FunctionContext> action)
            : this(name, action)
        {
            Key = key;
        }

        public bool IsEnabled { get; set; }

        public TriggerCollection Collection => _collection;
        public IEnumerable<string> Events => _events;

        protected override string GetName()
        {
            return _name;
        }

        public void AddEvent(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException();
            }

            if (_collection != null)
            {
                throw new InvalidOperationException();
            }

            _events.Add(key);
        }

        public void DoAction(FunctionContext context)
        {
            if (!IsEnabled)
            {
                return;
            }

            //MarkAccess($"Trigger do action : {_name}");
            MarkAccess();

            _action?.Invoke(context);
        }

    }
}
