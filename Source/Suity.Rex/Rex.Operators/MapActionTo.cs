// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Rex.VirtualDom;

namespace Suity.Rex.Operators
{
    class MapActionTo<T> : RexListenerBase<T, T> where T : ActionArguments
    {
        public MapActionTo(IRexListener<T> source, RexTree engine, RexPath path)
            : base(source)
        {
            if (engine == null)
            {
                throw new ArgumentNullException();
            }
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            source.Subscribe(o =>
            {
                engine.DoAction(path, (ActionArguments)o);
                HandleCallBack(o);
            });
        }
    }
}
