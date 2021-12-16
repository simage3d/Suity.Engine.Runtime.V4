// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Mapping
{
    public interface IRexAssembler<T>
    {
        T Assemble(object target, string name);
    }

    public delegate T RexAssembleDelegate<T>(object target, string name);

    public class RexAssembler<T> : IRexAssembler<T>
    {
        readonly RexAssembleDelegate<T> _assemble;

        public RexAssembler(RexAssembleDelegate<T> assemble)
        {
            _assemble = assemble ?? throw new ArgumentNullException(nameof(assemble));
        }

        public T Assemble(object target, string name)
        {
            return _assemble(target, name);
        }
    }
}
