// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Engine
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class NodeServiceAttribute : Attribute
    {
        public Type ServiceType { get; }
        public NodeServiceAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }
    }
}
