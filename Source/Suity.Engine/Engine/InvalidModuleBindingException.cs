// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Engine
{

    [Serializable]
    public class ModuleException : Exception
    {
        public ModuleException() { }
        public ModuleException(string message) : base(message) { }
        public ModuleException(string message, Exception inner) : base(message, inner) { }

#if !BRIDGE
        protected ModuleException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
    }
}
