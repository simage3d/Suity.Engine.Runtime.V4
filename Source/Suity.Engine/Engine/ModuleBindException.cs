// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Engine
{

    [Serializable]
    public class ModuleBindException : Exception
    {
        public ModuleBindException() { }
        public ModuleBindException(string message) : base(message) { }
        public ModuleBindException(string message, Exception inner) : base(message, inner) { }

#if !BRIDGE
        protected ModuleBindException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
    }
}
