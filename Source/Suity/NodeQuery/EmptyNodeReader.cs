// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
#if !BRIDGE
using System;
using System.Collections.Generic;
using Suity.Collections;

namespace Suity.NodeQuery
{
    public class EmptyNodeReader : MarshalByRefObject, INodeReader
    {
        private static readonly EmptyNodeReader InternalInstace = new EmptyNodeReader();
        public static EmptyNodeReader Empty { get { return InternalInstace; } }

        public bool Exist => false;
        public string NodeName => string.Empty;
        public int ChildCount => 0;
        public string NodeValue => string.Empty;

        public INodeReader Node(int index) => this;

        public INodeReader Node(string name) => this;

        public string GetAttribute(string name) => string.Empty;

        public IEnumerable<INodeReader> Nodes(string name) => EmptyArray<INodeReader>.Empty;

        public IEnumerable<INodeReader> Nodes() => EmptyArray<INodeReader>.Empty;

        public IEnumerable<string> NodeNames => EmptyArray<string>.Empty;

        public IEnumerable<KeyValuePair<string, string>> Attributes => EmptyArray<KeyValuePair<string, string>>.Empty;

        public override string ToString()
        {
            return string.Empty;
        }

    }
}
#endif