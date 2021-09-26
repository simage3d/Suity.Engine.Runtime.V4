// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.NodeQuery
{
    public sealed class EmptyNodeWriter : INodeWriter
    {
        public static readonly EmptyNodeWriter Empty = new EmptyNodeWriter();

        private EmptyNodeWriter()
        {

        }

        public void AddElement(string name, Action<INodeWriter> action)
        {
        }

        public void SetAttribute(string name, object valueToString)
        {
        }

        public void SetValue(string value)
        {
        }
    }
}
