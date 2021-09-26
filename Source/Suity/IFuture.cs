// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    /// <summary>
    /// 表示一个未来等待
    /// </summary>
    public interface IFuture
    {
        IFuture OnResult(Action<object> onResult);
        IFuture OnError(Action<ErrorResult> onError);
        IFuture OnProgress(Action<object> onProgress);
    }

    /// <summary>
    /// 表示一个未来等待
    /// </summary>
    /// <typeparam name="T">需等待的对象</typeparam>
    public interface IFuture<T> : IFuture
    {
        IFuture<T> OnResult(Action<T> onResult);
        new IFuture<T> OnError(Action<ErrorResult> onError);
        new IFuture<T> OnProgress(Action<object> onProgress);
    }
}
