// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Mapping
{
    public interface IRexProducer<T>
    {
        T Produce(string name);
        bool Recycle(string name, T product);
    }

    public delegate T RexProduceDelegate<T>(string name);
    public delegate bool RexRecycleDelegate<T>(string name, T product);

    public class RexProducer<T> : IRexProducer<T>
    {
        readonly RexProduceDelegate<T> _produce;
        readonly RexRecycleDelegate<T> _recycle;

        public RexProducer(RexProduceDelegate<T> produce, RexRecycleDelegate<T> recycle = null)
        {
            _produce = produce ?? throw new ArgumentNullException(nameof(produce));
            _recycle = recycle;
        }

        public T Produce(string name)
        {
            return _produce(name);
        }

        public bool Recycle(string name, T product)
        {
            return _recycle?.Invoke(name, product) ?? false;
        }
    }
}
