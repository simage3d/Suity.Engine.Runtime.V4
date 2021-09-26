// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
#if !BRIDGE
using System;
using System.Collections.Generic;
using System.Linq;
using Suity.Collections;

namespace Suity.NodeQuery
{
    [Serializable]
    public sealed class RawNode : INodeReader, IEquatable<RawNode>
    {
        public RawNode Parent { get; internal set; }

        public string NodeName { get; set; }
        public string NodeValue { get; set; }

        public bool Exist => true;

        public int ChildCount => _childNodes?.Count ?? 0;

        private List<RawNode> _childNodes;
        private Dictionary<string, string> _attributes;

        public RawNode()
        {
        }
        public RawNode(string nodeName)
        {
            NodeName = nodeName;
        }
        public RawNode(string nodeName, string nodeValue)
            : this(nodeName)
        {
            NodeValue = nodeValue;
        }

        public INodeReader Node(int index)
        {
            if (_childNodes != null && index >= 0 && index < _childNodes.Count)
            {
                return _childNodes[index];
            }
            else
            {
                return EmptyNodeReader.Empty;
            }
        }

        public INodeReader Node(string name)
        {
            if (_childNodes != null)
            {
                return _childNodes.Find(o => o.NodeName == name) ?? (INodeReader)EmptyNodeReader.Empty;
            }
            else
            {
                return EmptyNodeReader.Empty;
            }
        }

        public IEnumerable<INodeReader> Nodes(string name)
        {
            if (_childNodes != null)
            {
                return _childNodes.Where(o => o.NodeName == name).OfType<INodeReader>();
            }
            else
            {
                return EmptyArray<INodeReader>.Empty;
            }
        }

        public IEnumerable<INodeReader> Nodes()
        {
            if (_childNodes != null)
            {
                return _childNodes.OfType<INodeReader>();
            }
            else
            {
                return EmptyArray<INodeReader>.Empty;
            }
        }

        public IEnumerable<string> NodeNames
        {
            get
            {
                if (_childNodes != null)
                {
                    return _childNodes.Select(o => o.NodeName);
                }
                else
                {
                    return EmptyArray<string>.Empty;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Attributes
        {
            get
            {
                if (_attributes != null)
                {
                    return _attributes.Select(o => o);
                }
                else
                {
                    return EmptyArray<KeyValuePair<string, string>>.Empty;
                }
            }
        }

        public string GetAttribute(string name)
        {
            if (_attributes != null)
            {
                return _attributes.GetValueOrDefault(name);
            }
            else
            {
                return null;
            }
        }


        public int AttributeCount => _attributes?.Count ?? 0;


        public RawNode this[int index]
        {
            get => _childNodes?.GetListItemSafe(index);
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                value.Parent?.RemoveNode(value);

                if (_childNodes == null)
                {
                    _childNodes = new List<RawNode>();
                }
                while (_childNodes.Count < index)
                {
                    AddNode("Item");
                }

                RawNode current = _childNodes[index];
                if (current != null)
                {
                    current.Parent = null;
                }

                _childNodes[index] = value;
                value.Parent = this;
            }
        }

        public void AddNode(RawNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            if (node.Parent == this)
            {
                return;
            }
            node.Parent?.RemoveNode(node);
            node.Parent = this;
            (_childNodes ?? (_childNodes = new List<RawNode>())).Add(node);
        }
        public RawNode AddNode(string nodeName)
        {
            RawNode node = new RawNode(nodeName);
            AddNode(node);
            return node;
        }
        public bool RemoveNode(RawNode node)
        {
            if (_childNodes?.Remove(node) == true)
            {
                node.Parent = null;
                if (_childNodes.Count == 0)
                {
                    _childNodes = null;
                }
                return true;
            }
            return false;
        }
        public int RemoveAll(string nodeName)
        {
            if (_childNodes == null)
            {
                return 0;
            }

            int count = 0;
            for (int i = _childNodes.Count - 1; i >= 0; i--)
            {
                if (_childNodes[i].NodeName == nodeName)
                {
                    _childNodes[i].Parent = null;
                    _childNodes.RemoveAt(i);
                    count++;
                }
            }

            return count;
        }
        public void SetAttribute(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            if (_attributes == null)
            {
                _attributes = new Dictionary<string, string>();
            }
            _attributes[name] = value;
        }
        public void SetAttribute(string name, object objectToString)
        {
            if (objectToString != null)
            {
                SetAttribute(name, objectToString.ToString());
            }
            else
            {
                UnsetAttribute(name);
            }
        }
        public void UnsetAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (_attributes != null)
            {
                _attributes.Remove(name);

                if (_attributes.Count == 0)
                {
                    _attributes = null;
                }
            }
        }
        public void Clear()
        {
            if (_childNodes != null)
            {
                foreach (var node in _childNodes)
                {
                    node.Parent = null;
                    node.Clear();
                }
                _childNodes.Clear();
                _childNodes = null;
            }
            _attributes?.Clear();
            _attributes = null;
        }

        public void Read(INodeReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            Clear();
            NodeName = reader.NodeName;
            NodeValue = reader.NodeValue;
            foreach (var attr in reader.Attributes)
            {
                if (_attributes == null)
                {
                    _attributes = new Dictionary<string, string>
                    {
                        [attr.Key] = attr.Value
                    };
                }
            }
            foreach (var childReader in reader.Nodes())
            {
                if (_childNodes == null)
                {
                    _childNodes = new List<RawNode>();
                }
                RawNode childNode = new RawNode(childReader.NodeName);
                childNode.Read(childReader);
                _childNodes.Add(childNode);
            }
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(NodeName) ? NodeName : base.ToString();
        }

        public void ClonePropertyFrom(RawNode other)
        {
            if (other == null || other.Equals(this))
            {
                return;
            }

            _childNodes?.Clear();
            _attributes?.Clear();

            NodeName = other.NodeName;
            NodeValue = other.NodeValue;
            
            if (other._childNodes?.Count > 0)
            {
                if (_childNodes == null)
                {
                    _childNodes = new List<RawNode>();
                }
                foreach (var otherChildNode in other._childNodes)
                {
                    RawNode childNode = new RawNode();
                    childNode.ClonePropertyFrom(otherChildNode);
                    _childNodes.Add(childNode);
                }
            }
            if (other._attributes?.Count > 0)
            {
                if (_attributes == null)
                {
                    _attributes = new Dictionary<string, string>();
                }
                foreach (var pair in other._attributes)
                {
                    _attributes.Add(pair.Key, pair.Value);
                }
            }
        }
        public RawNode Clone()
        {
            RawNode node = new RawNode();
            node.ClonePropertyFrom(this);
            return node;
        }

        public static RawNode FromReader(INodeReader reader)
        {
            RawNode node = new RawNode();
            if (reader != null)
            {
                node.Read(reader);
            }
            return node;
        }

        public bool Equals(RawNode other)
        {
            if (other == null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (NodeName != other.NodeName)
            {
                return false;
            }
            if (NodeValue != other.NodeValue)
            {
                return false;
            }

            if ((_childNodes != null) != (other._childNodes != null))
            {
                return false;
            }
            if (_childNodes != null)
            {
                if (_childNodes.Count != other._childNodes.Count)
                {
                    return false;
                }

                for (int i = 0; i < _childNodes.Count; i++)
                {
                    if (!_childNodes[i].Equals(other._childNodes[i]))
                    {
                        return false;
                    }
                }
            }

            if ((_attributes != null) != (other._attributes != null))
            {
                return false;
            }
            if (_attributes != null)
            {
                if (_attributes.Count != other._attributes.Count)
                {
                    return false;
                }

                foreach (var pair in _attributes)
                {
                    if (pair.Value != other.GetAttribute(pair.Key))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public class RawNodeWriter : INodeWriter
    {
        RawNode _currentNode;

        public RawNode Result => _currentNode;

        public RawNodeWriter(string rootName)
        {
            _currentNode = new RawNode(rootName);
        }
        public RawNodeWriter(RawNode rootNode)
        {
            _currentNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
        }

        public void AddElement(string name, Action<INodeWriter> action)
        {
            RawNode childNode = new RawNode(name);
            RawNode lastNode = _currentNode;

            _currentNode.AddNode(childNode);
            _currentNode = childNode;
            try
            {
                action(this);
            }
            finally
            {
                _currentNode = lastNode;
            }
        }

        public void SetAttribute(string name, object valueToString)
        {
            if (valueToString != null)
            {
                _currentNode.SetAttribute(name, valueToString.ToString());
            }
            else
            {
                _currentNode.UnsetAttribute(name);
            }
        }

        public void SetValue(string value)
        {
            _currentNode.NodeValue = value;
        }

    }

}
#endif