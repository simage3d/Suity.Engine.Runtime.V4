// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class ToDataId<TSource> : RexListenerBase<TSource, string> where TSource : class
    {
        public ToDataId(IRexListener<TSource> source)
            : base(source)
        {
            source.Subscribe(o =>
            {
                string result = DataStorage.GetDataId(o);
                HandleCallBack(result);
            });
        }
    }
}
