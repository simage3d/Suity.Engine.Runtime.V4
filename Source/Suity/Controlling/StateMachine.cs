// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Suity.Controlling
{
    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class StateMachine : Suity.ResourceObject
    {
        readonly List<State> _list = new List<State>();
        readonly Dictionary<string, State> _states = new Dictionary<string, State>();

        State _currentState;
        State _targetState;

        float _stateChangingStartTime;
        float _stateChangingDelay;

        public StateMachine()
        {
        }
        public StateMachine(string key)
        {
            Key = key;
        }

        public bool AutoChangeToFirstState { get; set; }

        public void AddState(State state)
        {
            if (state == null)
            {
                throw new ArgumentNullException();
            }
            if (state._stateMachine != null)
            {
                throw new ArgumentException();
            }
            if (_states.ContainsKey(state.Name))
            {
                throw new InvalidOperationException("Name exists : " + state.Name);
            }

            state._stateMachine = this;
            state._index = _list.Count;

            _states.Add(state.Name, state);

            _list.Add(state);
        }

        public State CurrentState { get { return _currentState; } }
        public State TargetState { get { return _targetState; } }

        public void Enter(FunctionContext ctx)
        {
            MarkAccess();

            if (AutoChangeToFirstState)
            {
                ChangeState(ctx, GetStateByIndex(0));
            }
        }
        public void Exit(FunctionContext ctx)
        {
            _currentState?.Exit(ctx);
            _currentState = null;
            _targetState = null;
        }
        public void Update(FunctionContext ctx)
        {
            _currentState?.Update(ctx);
            if (_targetState != null)
            {
                if (LocalTime.Time - _stateChangingStartTime >= _stateChangingDelay)
                {
                    var state = _targetState;
                    ChangeState(ctx, state);
                }
            }
        }
        public void DoAction(FunctionContext ctx)
        {
            _currentState?.DoAction(ctx);
        }

        public void ResetState(FunctionContext ctx)
        {
            _currentState?.Exit(ctx);
            _currentState = null;
            _targetState = null;
            if (AutoChangeToFirstState)
            {
                ChangeState(ctx, GetStateByIndex(0));
            }
        }
        public bool ChangeState(FunctionContext ctx, State state)
        {
            if (state == null)
            {
                return false;
            }
            if (state._stateMachine != this)
            {
                return false;
            }
            if (_currentState == state)
            {
                return false;
            }

            _currentState?.Exit(ctx);
            _currentState = state;
            _targetState = null;
            _currentState?.Enter(ctx);
            return true;
        }
        public bool ChangeStateDelayed(FunctionContext ctx, State state, float delaySeconds)
        {
            if (state != null && state._stateMachine != this)
            {
                return false;
            }
            if (_currentState == state)
            {
                return false;
            }

            _targetState = state;
            _stateChangingDelay = delaySeconds;
            _stateChangingStartTime = LocalTime.Time;

            return true;
        }

        public bool ChangeToNextState(FunctionContext ctx)
        {
            if (_currentState == null)
            {
                return false;
            }

            int index = _currentState._index + 1;
            if (index < _list.Count)
            {
                return ChangeState(ctx, _list[index]);
            }
            else
            {
                return false;
            }
        }
        public bool ChangeToNextStateDelayed(FunctionContext ctx, float delaySeconds)
        {
            if (_currentState == null)
            {
                return false;
            }

            int index = _currentState._index + 1;
            if (index < _list.Count)
            {
                return ChangeStateDelayed(ctx, _list[index], delaySeconds);
            }
            else
            {
                return false;
            }
        }
        public bool ChangeToPreviousState(FunctionContext ctx)
        {
            if (_currentState == null)
            {
                return false;
            }

            int index = _currentState._index - 1;
            if (index >= 0)
            {
                return ChangeState(ctx, _list[index]);
            }
            else
            {
                return false;
            }
        }
        public bool ChangeToPreviousStateDelayed(FunctionContext ctx, float delaySeconds)
        {
            if (_currentState == null)
            {
                return false;
            }

            int index = _currentState._index - 1;
            if (index >= 0)
            {
                return ChangeStateDelayed(ctx, _list[index], delaySeconds);
            }
            else
            {
                return false;
            }
        }

        public State GetStateByIndex(int index)
        {
            if (index >= 0 && index < _list.Count)
            {
                return _list[index];
            }
            else
            {
                return null;
            }
        }
        public State GetStateByName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (_states.TryGetValue(name, out State state))
                {
                    return state;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public Trigger FindTrigger(string name)
        {
            foreach (var state in _list)
            {
                Trigger trigger = state.GetTrigger(name);
                if (trigger != null)
                {
                    return trigger;
                }
            }
            return null;
        }
    }
}
