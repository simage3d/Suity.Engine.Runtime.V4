// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public class ResultFuture<T> : IFuture<T>
    {
        readonly T _result;


        public ResultFuture(T result)
        {
            _result = result;
        }

        IFuture IFuture.OnResult(Action<object> onResult)
        {
            onResult?.Invoke(_result);
            return this;
        }

        IFuture IFuture.OnError(Action<ErrorResult> onError)
        {
            return this;
        }

        IFuture IFuture.OnProgress(Action<object> onProgress)
        {
            return this;
        }

        public IFuture<T> OnResult(Action<T> onResult)
        {
            onResult?.Invoke(_result);
            return this;
        }

        public IFuture<T> OnError(Action<ErrorResult> onError)
        {
            return this;
        }

        public IFuture<T> OnProgress(Action<object> onProgress)
        {
            return this;
        }
    }
}
