// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex
{
    public class RexListenerBase<TSource, TResult> : IRexListener<TResult>
    {
        internal readonly IRexListener<TSource> _source;
        internal Action<TResult> _callBack;

        public RexListenerBase(IRexListener<TSource> source)
        {
            _source = source ?? throw new ArgumentNullException();
        }

        public IRexHandle Subscribe(Action<TResult> callBack)
        {
            _callBack += callBack;
            return this;
        }

        internal void HandleCallBack(TResult result)
        {
            _callBack?.Invoke(result);
        }
        
        public void Dispose()
        {
            _callBack = null;
            _source.Dispose();
        }
        public IRexHandle Push()
        {
            _source.Push();
            return this;
        }

    }


    class RexListenerBase<TSource1, TSource2, TResult> : IRexListener<TResult>
    {
        internal readonly IRexListener<TSource1> _source1;
        internal readonly IRexListener<TSource2> _source2;
        internal Action<TResult> _callBack;

        public RexListenerBase(IRexListener<TSource1> source1, IRexListener<TSource2> source2)
        {
            _source1 = source1 ?? throw new ArgumentNullException();
            _source2 = source2 ?? throw new ArgumentNullException();
        }

        public IRexHandle Subscribe(Action<TResult> callBack)
        {
            _callBack += callBack;
            return this;
        }

        internal void HandleCallBack(TResult result)
        {
            _callBack?.Invoke(result);
        }

        public void Dispose()
        {
            _callBack = null;
            _source1.Dispose();
            _source2.Dispose();
        }
        public IRexHandle Push()
        {
            _source1.Push();
            _source2.Push();
            return this;
        }

    }


    class RexListenerBase<TSource1, TSource2, TSource3, TResult> : IRexListener<TResult>
    {
        internal readonly IRexListener<TSource1> _source1;
        internal readonly IRexListener<TSource2> _source2;
        internal readonly IRexListener<TSource3> _source3;
        internal Action<TResult> _callBack;

        public RexListenerBase(IRexListener<TSource1> source1, IRexListener<TSource2> source2, IRexListener<TSource3> source3)
        {
            if (source1 == null)
            {
                throw new ArgumentNullException();
            }
            if (source2 == null)
            {
                throw new ArgumentNullException();
            }
            if (source3 == null)
            {
                throw new ArgumentNullException();
            }
            _source1 = source1;
            _source2 = source2;
            _source3 = source3;
        }

        public IRexHandle Subscribe(Action<TResult> callBack)
        {
            _callBack += callBack;
            return this;
        }

        internal void HandleCallBack(TResult result)
        {
            _callBack?.Invoke(result);
        }

        public void Dispose()
        {
            _callBack = null;
            _source1.Dispose();
            _source2.Dispose();
            _source3.Dispose();
        }
        public IRexHandle Push()
        {
            _source1.Push();
            _source2.Push();
            _source3.Push();
            return this;
        }
    }
}
