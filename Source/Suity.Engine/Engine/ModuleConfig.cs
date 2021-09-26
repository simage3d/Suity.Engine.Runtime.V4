// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using Suity.Collections;
using Suity.Helpers;

namespace Suity.Engine
{
    public class ModuleConfig : IServiceProvider
    {
        readonly object _owner;
        readonly string _name;
        readonly IServiceProvider _serviceProvider;
        readonly Dictionary<string, object> _items = new Dictionary<string, object>();

        public object Owner => _owner;
        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(_name))
                {
                    return _name;
                }
                return _owner?.ToString() ?? string.Empty;
            }
        }

        public ModuleConfig()
        {
        }
        public ModuleConfig(object owner, string name, IServiceProvider serviceProvider = null)
        {
            _owner = owner;
            _name = name;
            _serviceProvider = serviceProvider;
        }
        public ModuleConfig(NodeComponent component, IServiceProvider serviceProvider = null)
            : this(component, component.Description, serviceProvider)
        {
        }

        public object GetService(Type serviceType)
        {
            if (_owner != null && serviceType.IsAssignableFrom(_owner.GetType()))
            {
                return _owner;
            }
            if (_serviceProvider != null)
            {
                return _serviceProvider.GetService(serviceType);
            }

            return Suity.Environment.GetService(serviceType);
        }

        public T GetItem<T>(string name, T defaultValue = default(T))
        {
            object value = _items.GetValueOrDefault(name);
            if (value == null && _owner != null)
            {
                value = ObjectType.GetProperty(_owner, name);
            }

            if (value is T t)
            {
                return t;
            }
            else
            {
                return defaultValue;
            }
        }
        public void SetItem<T>(string name, T value)
        {
            _items[name] = value;
        }

        public T GetItem<T>(ModuleConfigParameter<T> parameter, T defaultValue = default(T))
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            return GetItem<T>(parameter.Name, defaultValue);
        }
        public void SetItem<T>(ModuleConfigParameter<T> parameter, T value)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            _items[parameter.Name] = value;
        }

        public IEnumerable<string> ItemNames => _items.Keys;
    }

    public class ModuleConfigParameter<T>
    {
        public string Name { get; }
        public ModuleConfigParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("message", nameof(name));
            }

            Name = name;
        }
    }
}
