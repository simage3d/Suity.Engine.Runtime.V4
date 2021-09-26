// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Suity.Collections;
using Suity.Controlling;
using Suity.Helpers;

namespace Suity.Engine
{
    public abstract class NodeComponent : Suity.Object
    {
        abstract class TriggerProxy
        {
            public abstract void DoAction(FunctionContext context);
        }
        class TriggerActionProxy : TriggerProxy
        {
            readonly Action _action;
            public TriggerActionProxy(object obj, MethodInfo method)
            {
                _action = (Action)Delegate.CreateDelegate(typeof(Action), obj, method);
            }
            public override void DoAction(FunctionContext context)
            {
                _action();
            }
        }
        class TriggerActionProxy<T> : TriggerProxy
        {
            readonly Action<T> _action;
            public TriggerActionProxy(object obj, MethodInfo method)
            {
                _action = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), obj, method);
            }
            public override void DoAction(FunctionContext context)
            {
                T value = context.Value is T ? (T)context.Value : default(T);
                _action(value);
            }
        }

        internal NodeObjectState _state = NodeObjectState.None;
        internal TriggerCollection _triggers;
        private string _name;

        protected DisposeCollector Listeners;

        public NodeObjectState State { get { return _state; } }


        public virtual string Description
        {
            get
            {
                string name = Name;
                if (!string.IsNullOrEmpty(name))
                {
                    return $"[{this.GetType().Name}:{name}]";
                }
                else
                {
                    return $"[{this.GetType().Name}]";
                }
            }
        }
        public virtual string Icon
        {
            get { return "*CoreIcon|Component"; }
        }

        public NodeObject ParentObject { get; internal set; }

        public NodeComponent()
        {
            Name = string.Empty;
            BindTriggers();
        }

        protected override string GetName() => _name;
        protected override void SetName(string name)
        {
            if (name == null)
            {
                name = string.Empty;
            }
            _name = name.Trim();
        }

        private void BindTriggers()
        {
#if BRIDGE
            var methods = this.GetType()
                .GetMethods().Where(o => !o.IsStatic && o.HasAttributeCached<NodeTriggerAttribute>());
#else
            var methods = this.GetType()
                .FindMembers(MemberTypes.Method,
                             BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                             InterfaceMethodFilter,
                             typeof(NodeTriggerAttribute))
                .Cast<MethodInfo>();
#endif

            foreach (var method in methods)
            {
                NodeTriggerAttribute attr = method.GetCustomAttributes(typeof(NodeTriggerAttribute), true).OfType<NodeTriggerAttribute>().FirstOrDefault();
                if (attr == null)
                {
                    continue;
                }
                if (method.ReturnType != typeof(void))
                {
                    continue;
                }
                var parameters = method.GetParameters();

                Trigger trigger;

                if (parameters.Length == 0)
                {
                    TriggerProxy proxy = (TriggerProxy)Activator.CreateInstance(typeof(TriggerActionProxy), this, method);
                    trigger = new Trigger(method.Name, proxy.DoAction);
                }
                else if (parameters.Length == 1)
                {
                    if (parameters[0].ParameterType == typeof(FunctionContext))
                    {
                        Action<FunctionContext> d = (Action<FunctionContext>)Delegate.CreateDelegate(typeof(Action<FunctionContext>), this, method);
                        trigger = new Trigger(method.Name, d);
                    }
                    else
                    {
                        Type proxyType = typeof(TriggerActionProxy<>).MakeGenericType(new Type[] { parameters[0].ParameterType });
                        TriggerProxy proxy = (TriggerProxy)Activator.CreateInstance(proxyType, this, method);
                        trigger = new Trigger(method.Name, proxy.DoAction);
                    }
                }
                else
                {
                    continue;
                }

                foreach (var e in attr._events)
                {
                    if (!string.IsNullOrEmpty(e))
                    {
                        trigger.AddEvent(e);
                    }
                }

                trigger.IsEnabled = true;

                (_triggers ?? (_triggers = new TriggerCollection())).AddTrigger(trigger);
            }
        }
        private static bool InterfaceMethodFilter(MemberInfo memberInfo, object filterCriteria)
        { 
            return memberInfo.GetCustomAttributes((Type)filterCriteria, true).Any();
        }

        public override string ToString()
        {
            string name = this.Name;
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }
            else
            {
                return base.ToString();
            }
        }

        #region Start Stop
        internal virtual void Awake()
        {
            OnAwake();
        }
        internal virtual void Start()
        {
            OnStart();
        }
        internal virtual void Stop()
        {
            OnStop();

            Listeners?.Dispose();
            Listeners = null;
        }
        protected virtual void OnAwake()
        {
        }
        protected virtual void OnStart()
        {
        }
        protected virtual void OnStop()
        {
        }
        internal protected virtual void OnDataReloaded()
        {
        } 
        #endregion

        #region GetCompoent
        public T GetComponent<T>() where T : class
        {
            NodeObject parent = ParentObject;
            if (parent != null)
            {
                return parent.GetComponent<T>();
            }
            else
            {
                return null;
            }
        }
        public T GetComponent<T>(string name) where T : class
        {
            NodeObject parent = ParentObject;
            if (parent != null)
            {
                return parent.GetComponent<T>(name);
            }
            else
            {
                return null;
            }
        }
        public T[] GetComponents<T>() where T : class
        {
            NodeObject parent = ParentObject;
            if (parent != null)
            {
                return parent.GetComponents<T>();
            }
            else
            {
                return EmptyArray<T>.Empty;
            }
        }
        public NodeComponent GetComponent(Type serviceType)
        {
            NodeObject parent = ParentObject;
            if (parent != null)
            {
                return parent.GetComponent(serviceType);
            }
            else
            {
                return null;
            }
        }
        public NodeComponent[] GetComponents(Type serviceType)
        {
            NodeObject parent = ParentObject;
            if (parent != null)
            {
                return parent.GetComponents(serviceType);
            }
            else
            {
                return EmptyArray<NodeComponent>.Empty;
            }
        }
        #endregion

        #region GetContent
        public virtual object GetService(Type serviceType)
        {
            return null;
        }
        #endregion

        #region SendMessage
        public void SendMessage(FunctionContext context)
        {
            ParentObject.SendMessage(context);
        }
        public void SendMessage(string eventKey)
        {
            ParentObject.SendMessage(eventKey);
        }
        public void SendMessage(string eventKey, object value)
        {
            ParentObject.SendMessage(eventKey, value);
        }

        internal IEnumerable<string> HandledEvents => _triggers?.HandledEvents ?? EmptyArray<string>.Empty;
        internal void DoAction(FunctionContext context)
        {
            _triggers?.DoAction(context);
        }
        #endregion

        #region Utils

        protected T BindModule<T>(string name, ModuleConfig config) where T : class
        {
            var moduleProvider = Suity.Environment.GetService<IModuleProvider>();
            if (moduleProvider == null)
            {
                Logs.LogError($"Can not bind module : {name}, IModuleProvider service not found.");
                return null;
            }
            return moduleProvider.Bind<T>(name, config);
        }

        protected void StartTimer(Action action, TimeSpan dueTime, TimeSpan period)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Timer timer = new Timer(o => 
            {
                try
                {
                    action();
                }
                catch (Exception err)
                {
                    Logs.LogError(err);
                }
            },
            null, dueTime, period);

            Listeners += timer;
        }

        #endregion
    }
}
