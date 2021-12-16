// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Rex.VirtualDom;

namespace Suity.Rex.Operators
{
    class SetDataTo<TSource, TResult> : RexListenerBase<TSource, TResult>
    {
        public SetDataTo(IRexListener<TSource> source, RexTree engine, Func<TSource, RexPath> pathFunc, Func<TSource, TResult> dataFunc)
            : base(source)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (pathFunc == null)
            {
                throw new ArgumentNullException();
            }
            if (dataFunc == null)
            {
                throw new ArgumentNullException();
            }

            source.Subscribe(o => 
            {
                var path = pathFunc(o);
                if (path == null)
                {
                    throw new NullReferenceException();
                }
                var data = dataFunc(o);

                engine.SetData(path, data);
                HandleCallBack(data);
            });
        }
    }

}
