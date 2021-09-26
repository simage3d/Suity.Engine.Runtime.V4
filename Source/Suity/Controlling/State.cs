// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Suity.Controlling
{
    public class State : Suity.ResourceObject
    {
        readonly string _name;
        readonly Action<FunctionContext> _enter;
        readonly Action<FunctionContext> _exit;
        readonly Action<FunctionContext> _update;
        TriggerCollection _triggers;
        List<Controller> _controllers;
        List<StateMachine> _stateMachines;

        internal StateMachine _stateMachine;
        internal int _index;

        bool _entered;

        public State(string name, Action<FunctionContext> enter, Action<FunctionContext> exit, Action<FunctionContext> update)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException();
            }

            _name = name;
            _enter = enter;
            _exit = exit;
            _update = update;
        }
        public State(string name, string key, Action<FunctionContext> enter, Action<FunctionContext> exit, Action<FunctionContext> update)
            : this(name, enter, exit, update)
        {
            Key = key;
        }

        public StateMachine StateMachine { get { return _stateMachine; } }
        public int Index { get { return _index; } }
        public bool IsEntered { get { return _entered; } }
        public float AutoChangeToNextState { get; set; }

        protected override string GetName()
        {
            return _name;
        }

        public void AddTrigger(Trigger trigger)
        {
            if (trigger == null)
            {
                throw new ArgumentNullException();
            }
            (_triggers ?? (_triggers = new TriggerCollection())).AddTrigger(trigger);
        }
        public void AddController(Controller controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException();
            }
            (_controllers ?? (_controllers = new List<Controller>())).Add(controller);
        }
        public void AddStateMachine(StateMachine stateMachine)
        {
            if (stateMachine == null)
            {
                throw new ArgumentNullException();
            }
            (_stateMachines ?? (_stateMachines = new List<StateMachine>())).Add(stateMachine);
        }

        internal void Enter(FunctionContext ctx)
        {
            if (_entered)
            {
                return;
            }

            _entered = true;

            MarkAccess($"State entered : {_name}");

            if (AutoChangeToNextState > 0 && _stateMachine != null)
            {
                _stateMachine.ChangeStateDelayed(ctx, _stateMachine.GetStateByIndex(_index + 1), AutoChangeToNextState);
            }
            _enter?.Invoke(ctx);
            if (_stateMachines != null)
            {
                foreach (var stateMachine in _stateMachines)
                {
                    stateMachine.Enter(ctx);
                }
            }
            if (_controllers != null)
            {
                foreach (var controller in _controllers)
                {
                    controller.Enter(ctx);
                }
            }
        }
        internal void Exit(FunctionContext ctx)
        {
            if (!_entered)
            {
                return;
            }

            _entered = false;

            _exit?.Invoke(ctx);
            if (_stateMachines != null)
            {
                foreach (var stateMachine in _stateMachines)
                {
                    stateMachine.Exit(ctx);
                }
            }
            if (_controllers != null)
            {
                foreach (var controller in _controllers)
                {
                    controller.Exit(ctx);
                }
            }
        }
        internal void Update(FunctionContext ctx)
        {
            if (!_entered)
            {
                return;
            }
            _update?.Invoke(ctx);
            if (_stateMachines != null)
            {
                foreach (var stateMachine in _stateMachines)
                {
                    stateMachine.Update(ctx);
                }
            }
            if (_controllers != null)
            {
                foreach (var controller in _controllers)
                {
                    controller.Update(ctx);
                }
            }
        }
        internal void DoAction(FunctionContext ctx)
        {
            if (!_entered)
            {
                return;
            }
            _triggers?.DoAction(ctx);
            if (_stateMachines != null)
            {
                foreach (var stateMachine in _stateMachines)
                {
                    stateMachine.DoAction(ctx);
                }
            }
            if (_controllers != null)
            {
                foreach (var controller in _controllers)
                {
                    controller.DoAction(ctx);
                }
            }
        }

        public Trigger GetTrigger(string name)
        {
            if (_triggers != null)
            {
                return _triggers.GetTrigger(name);
            }
            else
            {
                return null;
            }
        }
    }
}
