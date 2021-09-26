// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
namespace Suity.NodeQuery
{
    public interface INodeWriter
    {
        void AddElement(string name, Action<INodeWriter> action);

        void SetValue(string value);

        void SetAttribute(string name, object valueToString);
    }
}