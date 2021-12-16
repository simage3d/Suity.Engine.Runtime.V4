using System;
using System.Collections.Generic;
using System.Text;

namespace Suity.Rex.Mapping
{
    public interface IRexReducer<T>
    {
        T Reduce(T state, string name, object payload);
    }

    public delegate T RexReduceDelegate<T>(T state, string name, object payload);

    public class RexReducer<T> : IRexReducer<T>
    {
        readonly RexReduceDelegate<T> _reduce;

        public RexReducer(RexReduceDelegate<T> reduce)
        {
            _reduce = reduce ?? throw new ArgumentNullException(nameof(reduce));
        }

        public T Reduce(T state, string name, object payload)
        {
            return _reduce(state, name, payload);
        }
    }
}
