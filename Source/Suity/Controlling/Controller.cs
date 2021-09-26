// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Controlling
{
    /// <summary>
    /// 控制器
    /// </summary>
    [AssetDefinitionType(AssetDefinitionCodes.Controller)]
    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public abstract class Controller : Suity.ResourceObject, IController
    {
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnEnabledChanged();
            }
        }

        public Controller()
        {
        }

        public virtual void Start(FunctionContext ctx)
        {
        }

        public virtual void Stop(FunctionContext ctx)
        {
        }

        public virtual void Enter(FunctionContext ctx)
        {
        }

        public virtual void Exit(FunctionContext ctx)
        {
        }

        public virtual void Update(FunctionContext ctx)
        {
        }

        public virtual void DoAction(FunctionContext ctx)
        {
        }

        public virtual Controller GetController(string name)
        {
            return null;
        }
        public virtual Trigger GetTrigger(string name)
        {
            return null;
        }
        public virtual StateMachine GetStateMachine(string name)
        {
            return null;
        }
        public virtual State GetState(string name)
        {
            return null;
        }

        public void SetTriggerEnabled(string name, bool enabled)
        {
            Trigger trigger = GetTrigger(name);
            if (trigger != null)
            {
                trigger.IsEnabled = enabled;
            }
        }
        public bool GetTriggerEnabled(string name)
        {
            return GetTrigger(name)?.IsEnabled ?? false;
        }

        protected virtual void OnEnabledChanged()
        {

        }
    }
}
