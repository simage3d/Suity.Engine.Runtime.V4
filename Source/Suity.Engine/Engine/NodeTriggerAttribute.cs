// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Engine
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class NodeTriggerAttribute : Attribute
    {
        internal readonly string[] _events;

        // This is a positional argument
        public NodeTriggerAttribute(params string[] events)
        {
            _events = events;
        }
    }
}
