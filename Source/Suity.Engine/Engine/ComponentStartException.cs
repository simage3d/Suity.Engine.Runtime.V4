// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Engine
{

    [Serializable]
    public class ComponentStartException : Exception
    {
        public ComponentStartException() { }
        public ComponentStartException(string message) : base(message) { }
        public ComponentStartException(string message, Exception inner) : base(message, inner) { }

#if !BRIDGE
        protected ComponentStartException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
    }
}
