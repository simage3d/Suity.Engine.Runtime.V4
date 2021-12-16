// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Collections;

namespace Suity.Rex.Mapping
{
    public enum RexMappingMode
    {
        External = 0,
        Preset = 1,
        Singleton = 2,
        Instance = 3,
    }

    public class RexNameRecord
    {
        readonly string _name;
        int _counterSuccess;
        int _counterFailed;

        public string Name { get { return _name; } }
        public int CounterSuccess { get { return _counterSuccess; } }
        public int CounterFailed { get { return _counterFailed; } }

        internal RexNameRecord(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            _name = name;
        }

        internal void Increase(bool success)
        {
            if (success)
            {
                _counterSuccess++;
            }
            else
            {
                _counterFailed++;
            }
        }
    }

    public sealed class RexMappingInfo
    {
        public Type ImplementType { get; }
        public RexMappingMode MappingMode { get; }
        public object PresetObject { get; }
        public object SingletonObject { get; internal set; }
        public int Counter { get { return _counter; } }
        public int NewCounter { get { return _newCounter; } }
        public IEnumerable<RexNameRecord> ResolvedNames { get { return _names != null ? _names.Values.Select(o => o) : (IEnumerable<RexNameRecord>)EmptyArray<RexNameRecord>.Empty; } }

        int _counter;
        int _newCounter;
        Dictionary<string, RexNameRecord> _names;

        internal RexMappingInfo(Type type)
        {
            ImplementType = type;
            MappingMode = RexMappingMode.External;
        }
        internal RexMappingInfo(Type implementType, bool singleton)
        {
            ImplementType = implementType ?? throw new ArgumentNullException(nameof(implementType));
            MappingMode = singleton ? RexMappingMode.Singleton : RexMappingMode.Instance;
        }
        internal RexMappingInfo(object presetObj)
        {
            if (presetObj == null)
            {
                throw new ArgumentNullException(nameof(presetObj));
            }

            ImplementType = presetObj.GetType();
            MappingMode = RexMappingMode.Preset;
            PresetObject = presetObj;
        }

        internal T Resolve<T>() where T : class
        {
            _counter++;

            switch (MappingMode)
            {
                case RexMappingMode.Preset:
                    return PresetObject as T;
                case RexMappingMode.Singleton:
                    if (SingletonObject is T t)
                    {
                        return t;
                    }
                    else
                    {
                        _newCounter++;
                        T result = (T)Activator.CreateInstance(ImplementType);
                        SingletonObject = result;
                        return result;
                    }
                case RexMappingMode.Instance:
                    _newCounter++;
                    return (T)Activator.CreateInstance(ImplementType);
                default:
                    return null;
            }
        }

        internal void IncreaseCounter()
        {
            _counter++;
        }
        internal void IncreaseNewCounter()
        {
            _counter++;
            _newCounter++;
        }
        internal void AddName(string name, bool success)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            if (_names == null)
            {
                _names = new Dictionary<string, RexNameRecord>();
            }
            _names.GetValueOrCreate(name, () => new RexNameRecord(name)).Increase(success);
        }

        public override string ToString()
        {
            if (MappingMode == RexMappingMode.External)
            {
                return "[External]";
            }
            else
            {
                return ImplementType.FullName;
            }
        }
    }
}
