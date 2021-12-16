// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex
{

    [Serializable]
    public class RexCancelException : Exception
    {
        public RexCancelException() { }
        public RexCancelException(string message) : base(message) { }
        public RexCancelException(string message, Exception inner) : base(message, inner) { }

#if !BRIDGE
        protected RexCancelException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
    }
}
