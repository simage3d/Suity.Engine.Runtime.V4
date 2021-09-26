// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Suity.Collections;
using Suity.Helpers;

namespace Suity.Engine
{
    public sealed class NodeObject : Suity.SystemObject
    {
        readonly HashSet<NodeComponent> _components = new HashSet<NodeComponent>();
        readonly UniqueMultiDictionary<Type, NodeComponent> _componentByServiceTypes = new UniqueMultiDictionary<Type, NodeComponent>();
        readonly UniqueMultiDictionary<string, NodeComponent> _componentByEvents = new UniqueMultiDictionary<string, NodeComponent>();
        readonly Dictionary<Type, object> _innerServices = new Dictionary<Type, object>();

        private NodeObjectState _state = NodeObjectState.None;
        private readonly object _sync = new object();

        public NodeObjectState State { get { return _state; } }

        public NodeObject()
        {
            _state = NodeObjectState.Initialize;
        }

        public T AddComponent<T>() where T : NodeComponent
        {
            return (T)AddComponent(typeof(T));
        }
        public NodeComponent AddComponent(Type componentType)
        {
            if (componentType == null)
            {
                throw new ArgumentNullException(nameof(componentType));
            }
            if (!typeof(NodeComponent).IsAssignableFrom(componentType))
            {
                throw new ArgumentException("ComponentType is invalid : " + componentType.Name, nameof(componentType));
            }

            NodeComponent component = (NodeComponent)Activator.CreateInstance(componentType);

            if (component == null)
            {
                throw new NullReferenceException("Create component failed : " + componentType.Name);
            }

            lock (_sync)
            {
                if (State != NodeObjectState.Initialize)
                {
                    throw new SecurityException();
                }

                if (_components.Contains(component))
                {
                    return null;
                }

                List<Type> serviceTypes = new List<Type>();
                foreach (var attr in component.GetType().GetAttributesCached<NodeServiceAttribute>())
                {
                    if (!attr.ServiceType.IsAssignableFrom(component.GetType()))
                    {
                        throw new InvalidOperationException("Service type is not assignable from component type : " + attr.ServiceType);
                    }
                    serviceTypes.Add(attr.ServiceType);
                }

                _components.Add(component);
                _componentByServiceTypes.Add(component.GetType(), component);
                foreach (var serviceType in serviceTypes)
                {
                    _componentByServiceTypes.Add(serviceType, component);
                }
                foreach (var eventKey in component.HandledEvents)
                {
                    _componentByEvents.Add(eventKey, component);
                }
                component.ParentObject = this;

                return component;
            }
        }
        public T GetComponent<T>() where T : class
        {
            lock (_sync)
            {
                if (_state == NodeObjectState.None || _state == NodeObjectState.Destroyed)
                {
                    return null;
                }

                NodeComponent component = _componentByServiceTypes[typeof(T)].FirstOrDefault();
                if (component == null)
                {
                    component = _components.FirstOrDefault(o => o is T);
                }
                
                if (component != null && component._state != NodeObjectState.Destroyed)
                {
                    InternalStartComponent(component);
                    return component as T;
                }

                return null;
            }
        }
        public T GetComponent<T>(string name) where T : class
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            lock (_sync)
            {
                if (_state == NodeObjectState.None || _state == NodeObjectState.Destroyed)
                {
                    return null;
                }

                var component = _componentByServiceTypes[typeof(T)].FirstOrDefault(o => o.Name == name);
                if (component == null)
                {
                    component = _components.FirstOrDefault(o => o is T && o.Name == name);
                }

                if (component != null && component._state != NodeObjectState.Destroyed)
                {
                    InternalStartComponent(component);
                    return component as T;
                }

                return null;
            }
        }
        public T[] GetComponents<T>() where T : class
        {
            lock (_sync)
            {
                if (_state == NodeObjectState.None || _state == NodeObjectState.Destroyed)
                {
                    return EmptyArray<T>.Empty;
                }

                var components = _components.Where(o => o is T && o._state != NodeObjectState.Destroyed);

                foreach (var component in components)
                {
                    InternalStartComponent(component);
                }
                return components.OfType<T>().ToArray();
            }
        }
        public IEnumerable<NodeComponent> Components => _components.Select(o => o);

        public NodeComponent GetComponent(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            lock (_sync)
            {
                if (_state == NodeObjectState.None || _state == NodeObjectState.Destroyed)
                {
                    return null;
                }

                NodeComponent component = _componentByServiceTypes[serviceType].FirstOrDefault();
                if (component == null)
                {
                    component = _components.FirstOrDefault(o => serviceType.IsAssignableFrom(o.GetType()));
                }

                if (component != null && component._state != NodeObjectState.Destroyed)
                {
                    InternalStartComponent(component);
                    return component;
                }

                return null;
            }
        }
        public NodeComponent GetComponent(Type serviceType, string name)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (string.IsNullOrEmpty(name))
            {
                return GetComponent(serviceType);
            }

            lock (_sync)
            {
                if (_state == NodeObjectState.None || _state == NodeObjectState.Destroyed)
                {
                    return null;
                }

                var component = _componentByServiceTypes[serviceType].FirstOrDefault(o => o.Name == name);
                if (component == null)
                {
                    component = _components.FirstOrDefault(o => serviceType.IsAssignableFrom(o.GetType()) && o.Name == name);
                }

                if (component != null && component._state != NodeObjectState.Destroyed)
                {
                    InternalStartComponent(component);
                    return component;
                }

                return null;
            }
        }
        public NodeComponent[] GetComponents(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            lock (_sync)
            {
                if (_state == NodeObjectState.None || _state == NodeObjectState.Destroyed)
                {
                    return EmptyArray<NodeComponent>.Empty;
                }

                var components = _components.Where(o => serviceType.IsAssignableFrom(o.GetType()) && o._state != NodeObjectState.Destroyed);

                foreach (var component in components)
                {
                    InternalStartComponent(component);
                }
                return components.ToArray();
            }
        }
        public NodeComponent[] GetAllComponents()
        {
            lock (_sync)
            {
                return _components.ToArray();
            }
        }

        public object GetService(Type serviceType)
        {
            var component = GetComponent(serviceType);
            if (component != null)
            {
                return component;
            }

            lock (_sync)
            {
                object obj = _innerServices.GetValueOrDefault(serviceType);
                if (obj != null)
                {
                    return obj;
                }

                foreach (var comp in _components)
                {
                    obj = comp.GetService(serviceType);
                    if (obj != null)
                    {
                        _innerServices.Add(serviceType, obj);
                        return obj;
                    }
                }
            }

            return null;
        }
        public object[] GetServices(Type serviceType)
        {
            lock (_sync)
            {
                if (_state == NodeObjectState.None || _state == NodeObjectState.Destroyed)
                {
                    return EmptyArray<NodeComponent>.Empty;
                }

                List<object> result = new List<object>();

                foreach (var component in _components)
                {
                    InternalStartComponent(component);

                    if (serviceType.IsAssignableFrom(component.GetType()) && component._state != NodeObjectState.Destroyed)
                    {
                        result.Add(component);
                        continue;
                    }

                    var content = component.GetService(serviceType);
                    if (content != null)
                    {
                        result.Add(content);
                    }
                }

                return result.ToArray();
            }
        }
        public T GetService<T>() where T :class
        {
            return GetService(typeof(T)) as T;
        }
        public T[] GetServices<T>()
        {
            return GetServices(typeof(T)).OfType<T>().ToArray();
        }

        public void SendMessage(FunctionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (string.IsNullOrEmpty(context.EventKey))
            {
                throw new ArgumentException("context.EventKey is not defined.");
            }
            foreach (var component in _componentByEvents[context.EventKey])
            {
                component.DoAction(context);
            }
        }
        public void SendMessage(string eventKey)
        {
            SendMessage(new FunctionContext(eventKey));
        }
        public void SendMessage(string eventKey, object value)
        {
            FunctionContext context = new FunctionContext(eventKey, value);
            SendMessage(context);
        }


        public override bool IsStarted => _state == NodeObjectState.Started;
        public override void Start()
        {
            lock (_sync)
            {
                if (_state == NodeObjectState.Started)
                {
                    return;
                }
                if (_state != NodeObjectState.Initialize)
                {
                    throw new InvalidOperationException();
                }

                _state = NodeObjectState.Started;
                foreach (var component in _components)
                {
                    try
                    {
                        component.Awake();
                    }
                    catch (Exception err)
                    {
                        Logs.LogError($"Component awake failed : {component.GetType().FullName}");
                        Logs.LogError(err);
                    }
                    
                }
                foreach (var component in _components)
                {
                    InternalStartComponent(component);
                }                
            }
        }
        public override void Stop()
        {
            lock (_sync)
            {
                if (_state == NodeObjectState.None)
                {
                    return;
                }

                foreach (var component in _components)
                {
                    component.Stop();
                }
                foreach (var component in _components)
                {
                    component._state = NodeObjectState.None;
                }
                _state = NodeObjectState.None;
            }
        }

        private void InternalStartComponent(NodeComponent component)
        {
            try
            {
                if (component._state != NodeObjectState.Started)
                {
                    component._state = NodeObjectState.Started;
                    component.Start();
                }
            }
            catch (Exception err)
            {
                component._state = NodeObjectState.Failed;
                //throw;

                Logs.LogError($"Component start failed : {component.GetType().FullName}");
                Logs.LogError(err);
            }
        }
    }
}
