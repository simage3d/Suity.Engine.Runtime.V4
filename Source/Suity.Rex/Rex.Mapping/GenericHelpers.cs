// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Mapping
{
    interface IRexMappingGenericObjectGetter
    {
        object GetObject(RexMapper mapper);
        IEnumerable<object> GetObjects(RexMapper mapper);
    }
    class RexMappingGenericObjectGetter<T> : IRexMappingGenericObjectGetter where T : class
    {
        public object GetObject(RexMapper mapper)
        {
            return mapper.Get<T>();
        }
        public IEnumerable<object> GetObjects(RexMapper mapper)
        {
            return mapper.GetMany<T>().OfType<object>();
        }
    }
}