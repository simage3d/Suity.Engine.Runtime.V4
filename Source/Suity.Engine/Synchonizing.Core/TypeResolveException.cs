// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing.Core
{
    [Serializable]
    public class TypeResolveException : Exception
    {
        public TypeResolveException() { }
        public TypeResolveException(string message) : base(message) { }
        public TypeResolveException(string message, Exception inner) : base(message, inner) { }
        protected TypeResolveException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
