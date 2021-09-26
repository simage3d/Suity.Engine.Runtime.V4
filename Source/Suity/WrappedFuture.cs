// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public class WrappedFuture<T> : IFuture<T>
    {
        readonly IFuture _inner;
        Action<ErrorResult> _onError;

        public WrappedFuture(IFuture inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public IFuture<T> OnError(Action<ErrorResult> onError)
        {
            _inner.OnError(onError);
            return this;
        }

        public IFuture<T> OnProgress(Action<object> onProgress)
        {
            _inner.OnProgress(onProgress);
            return this;
        }

        public IFuture<T> OnResult(Action<T> onResult)
        {
            _inner.OnResult(o => 
            {
                if (o is T t)
                {
                    onResult(t);
                }
                else
                {
                    _onError?.Invoke(new ErrorResult
                    {
                        StatusCode = nameof(StatusCodes.InvalidCast),
                        Message = $"Can not cast {o?.GetType().Name ?? "null"} to {typeof(T).Name}.",
                    });
                }
            });
            return this;
        }

        IFuture IFuture.OnResult(Action<object> onResult)
        {
            _inner.OnResult(onResult);
            return this;
        }

        IFuture IFuture.OnError(Action<ErrorResult> onError)
        {
            _onError = onError;

            _inner.OnError(onError);
            return this;
        }

        IFuture IFuture.OnProgress(Action<object> onProgress)
        {
            _inner.OnProgress(onProgress);
            return this;
        }
    }
}
