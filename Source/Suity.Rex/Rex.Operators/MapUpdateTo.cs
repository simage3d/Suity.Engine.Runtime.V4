// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Rex.VirtualDom;

namespace Suity.Rex.Operators
{
    class MapUpdateTo<T> : RexListenerBase<T, T>
    {
        public MapUpdateTo(IRexListener<T> source, RexTree engine, Func<T, RexPath> pathFunc)
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

            source.Subscribe(o => 
            {
                var path = pathFunc(o);
                if (path == null)
                {
                    throw new NullReferenceException();
                }
                engine.UpdateData(path);
                HandleCallBack(o);
            });
        }
    }

}
