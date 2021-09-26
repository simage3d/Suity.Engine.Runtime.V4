// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections.Generic;

namespace Suity.NodeQuery
{
    public interface INodeReader
    {
        bool Exist { get; }

        string NodeName { get; }

        int ChildCount { get; }

        string NodeValue { get; }

        INodeReader Node(int index);

        INodeReader Node(string name);

        IEnumerable<INodeReader> Nodes(string name);

        IEnumerable<INodeReader> Nodes();

        IEnumerable<string> NodeNames { get; }

        IEnumerable<KeyValuePair<string, string>> Attributes { get; }

        string GetAttribute(string name);
    }
}