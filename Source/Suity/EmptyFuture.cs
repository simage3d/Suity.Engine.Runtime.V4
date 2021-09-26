// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public sealed class EmptyFuture : IFuture
    {
        public static readonly EmptyFuture Empty = new EmptyFuture();


        private EmptyFuture() { }

        public IFuture OnResult(Action<object> onComplete)
        {
            return this;
        }

        public IFuture OnError(Action<ErrorResult> onError)
        {
            return this;
        }

        public IFuture OnProgress(Action<object> onProgress)
        {
            return this;
        }
    }

    public sealed class EmptyFuture<T> : IFuture<T>
    {
        public static readonly EmptyFuture<T> Empty = new EmptyFuture<T>();

        private EmptyFuture() { }

        public IFuture<T> OnError(Action<ErrorResult> onError)
        {
            return this;
        }

        public IFuture<T> OnProgress(Action<object> onProgress)
        {
            return this;
        }

        public IFuture<T> OnResult(Action<T> onResult)
        {
            return this;
        }

        IFuture IFuture.OnResult(Action<object> onResult)
        {
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
    }
}
