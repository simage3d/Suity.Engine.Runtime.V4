// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public class ErrorFuture<T> : IFuture<T>
    {
        readonly ErrorResult _error;

        public ErrorFuture(ErrorResult e)
        {
            _error = e;
        }
        public ErrorFuture(StatusCodes code)
        {
            _error = new ErrorResult
            {
                StatusCode = code.ToString(),
            };
        }
        public ErrorFuture(StatusCodes code, string message)
        {
            _error = new ErrorResult
            {
                StatusCode = code.ToString(),
                Message = message,
            };
        }
        public ErrorFuture(StatusCodes code, string message, string location)
        {
            _error = new ErrorResult
            {
                StatusCode = code.ToString(),
                Message = message,
                Location = location,
            };
        }

        public ErrorFuture(string code)
        {
            _error = new ErrorResult
            {
                StatusCode = code,
            };
        }
        public ErrorFuture(string code, string message)
        {
            _error = new ErrorResult
            {
                StatusCode = code,
                Message = message,
            };
        }
        public ErrorFuture(string code, string message, string location)
        {
            _error = new ErrorResult
            {
                StatusCode = code,
                Message = message,
                Location = location,
            };
        }

        #region IFuture 成员

        IFuture IFuture.OnResult(Action<object> onResult)
        {
            return this;
        }

        IFuture IFuture.OnError(Action<ErrorResult> onError)
        {
            onError?.Invoke(_error);
            return this;
        }

        IFuture IFuture.OnProgress(Action<object> onProgress)
        {
            return this;
        }

        public IFuture<T> OnResult(Action<T> onResult)
        {
            return this;
        }

        public IFuture<T> OnError(Action<ErrorResult> onError)
        {
            onError?.Invoke(_error);
            return this;
        }

        public IFuture<T> OnProgress(Action<object> onProgress)
        {
            return this;
        }

        #endregion
    }
}
