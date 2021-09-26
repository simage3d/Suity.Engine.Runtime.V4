// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity
{
    /// <summary>
    /// 空结果
    /// </summary>
    public class EmptyResult
    {
        public static readonly EmptyResult Empty = new EmptyResult();
        public override string ToString()
        {
            return string.Empty;
        }
    }
    /// <summary>
    /// 错误结果
    /// </summary>
    public class ErrorResult
    {
        public string StatusCode;
        
        public string Message;

        public string Location;

        public override string ToString()
        {
            return $"Status:{StatusCode} Message:{Message} Location:{Location}";
        }
    }
    public class LocalExceptionErrorResult : ErrorResult
    {
        public Exception ExceptionObject;

        public LocalExceptionErrorResult()
        {
        }
        public LocalExceptionErrorResult(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }

        public override string ToString()
        {
            return ExceptionObject?.Message ?? base.ToString();
        }
    }

}
