// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity
{
    [Serializable]
    public class ExecuteException : Exception
    {
        public string StatusCode { get; }


        public ExecuteException(StatusCodes statusCode)
            : base(statusCode.ToString())
        {
            StatusCode = statusCode.ToString();
        }
        public ExecuteException(StatusCodes statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode.ToString();
        }

        public ExecuteException(string statusCode) 
            : base(statusCode)
        {
            StatusCode = statusCode;
        }
        public ExecuteException(string statusCode, string message)
            : base(message) 
        {
            StatusCode = statusCode;
        }
        public ExecuteException(string statusCode, string message, Exception inner)
            : base(message, inner) 
        {
            StatusCode = statusCode;
        }
#if !BRIDGE
        protected ExecuteException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
}
