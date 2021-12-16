// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Collections;
using Suity.Helpers;

namespace Suity.Rex.Mapping
{

    public sealed class RexMappingCollection
    {
        readonly Type _requestType;
        readonly LinkedList<RexMappingInfo> _infos = new LinkedList<RexMappingInfo>();
        readonly Dictionary<object, LinkedListNode<RexMappingInfo>> _objToInfo = new Dictionary<object, LinkedListNode<RexMappingInfo>>();
        RexMappingInfo _externalResolve;

        Predicate<RexMappingInfo> _filter;

        public Type RequestType { get { return _requestType; } }

        internal Predicate<RexMappingInfo> Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        internal RexMappingCollection(Type requestType)
        {
            _requestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
        }
        internal RexMappingCollection(Type requestType, Predicate<RexMappingInfo> filter)
            : this(requestType)
        {
            _filter = filter;
        }

        internal void Add(RexMappingInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            LinkedListNode<RexMappingInfo> node;

            switch (info.MappingMode)
            {
                case RexMappingMode.Preset:
                    if (info.PresetObject == null)
                    {
                        throw new NullReferenceException(nameof(info.PresetObject));
                    }
                    if (_objToInfo.ContainsKey(info.PresetObject))
                    {
                        throw new InvalidOperationException("Object exist.");
                    }
                    node = SortedAdd(info);
                    _objToInfo.Add(info.PresetObject, node);
                    break;
                case RexMappingMode.Singleton:
                case RexMappingMode.Instance:
                    if (_objToInfo.ContainsKey(info.ImplementType))
                    {
                        throw new InvalidOperationException("Object exist.");
                    }
                    node = SortedAdd(info);
                    _objToInfo.Add(info.ImplementType, node);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
        internal bool Remove(object obj)
        {
            LinkedListNode<RexMappingInfo> node = _objToInfo.RemoveAndGet(obj);
            if (node != null)
            {
                _infos.Remove(node);
                return true;
            }
            else
            {
                return false;
            }
        }
        internal void Clear()
        {
            _infos.Clear();
            _objToInfo.Clear();
        }
        internal RexMappingInfo IncreaseExternalResolved()
        {
            (_externalResolve ?? (_externalResolve = new RexMappingInfo(_requestType))).IncreaseCounter();
            return _externalResolve;
        }
        public bool Contains(object obj)
        {
            return _objToInfo.ContainsKey(obj);
        }

        public RexMappingInfo First()
        {
            if (_filter != null)
            {
                var node = _infos.First;
                while (node != null)
                {
                    if (_filter(node.Value))
                    {
                        return node.Value;
                    }
                    node = node.Next;
                }
                return null;
            }
            else
            {
                var first = _infos.First;
                return first?.Value;
            }
        }
        public IEnumerable<RexMappingInfo> Infos
        {
            get
            {
                IEnumerable<RexMappingInfo> infos;

                if (_filter != null)
                {
                    infos = _infos.Where(o => _filter(o));
                }
                else
                {
                    infos = _infos.Select(o => o);
                }

                if (_externalResolve != null)
                {
                    return infos.ConcatOne(_externalResolve);
                }
                else
                {
                    return infos;
                }
            }
        }


        private LinkedListNode<RexMappingInfo> SortedAdd(RexMappingInfo info)
        {
            var node = _infos.First;

            while (node != null)
            {
                if (info.MappingMode > node.Value.MappingMode)
                {
                    return _infos.AddBefore(node, info);
                }
                node = node.Next;
            }

            return _infos.AddLast(info);
        }
    }
}
