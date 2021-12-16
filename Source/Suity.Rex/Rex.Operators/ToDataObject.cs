// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class ToDataObject<TResult> : RexListenerBase<string, TResult> where TResult : class
    {
        public ToDataObject(IRexListener<string> source)
            : base(source)
        {
            source.Subscribe(key =>
            {
                TResult result = DataStorage.GetObject<TResult>(key);
                HandleCallBack(result);
            });
        }
    }
}
