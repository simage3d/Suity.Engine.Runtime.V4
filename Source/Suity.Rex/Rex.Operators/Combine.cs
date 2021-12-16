// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class Combine<TSource1, TSource2, TResult> : RexListenerBase<TSource1, TSource2, TResult>
    {
        bool _signal1;
        bool _signal2;

        TSource1 _value1;
        TSource2 _value2;

        public Combine(
            IRexListener<TSource1> source1, 
            IRexListener<TSource2> source2, 
            Func<TSource1, TSource2, bool> predicate, 
            Func<TSource1, TSource2, TResult> selector
            )
            : base(source1, source2)
        {
            if (selector == null)
            {
                throw new ArgumentNullException();
            }

            source1.Subscribe(o => 
            {
                _signal1 = true;
                _value1 = o;
                if (_signal1 && _signal2 && (predicate == null || predicate(_value1, _value2)))
                {
                    TResult result = selector(_value1, _value2);
                    HandleCallBack(result);
                }
            });
            source2.Subscribe(o =>
            {
                _signal2 = true;
                _value2 = o;
                if (_signal1 && _signal2 && (predicate == null || predicate(_value1, _value2)))
                {
                    TResult result = selector(_value1, _value2);
                    HandleCallBack(result);
                }
            });
        }
    }


    class Combine<TSource1, TSource2, TSource3, TResult> : RexListenerBase<TSource1, TSource2, TSource3, TResult>
    {
        bool _signal1;
        bool _signal2;
        bool _signal3;

        TSource1 _value1;
        TSource2 _value2;
        TSource3 _value3;

        public Combine(
            IRexListener<TSource1> source1,
            IRexListener<TSource2> source2,
            IRexListener<TSource3> source3,
            Func<TSource1, TSource2, TSource3, bool> predicate,
            Func<TSource1, TSource2, TSource3, TResult> selector
            )
            : base(source1, source2, source3)
        {
            if (selector == null)
            {
                throw new ArgumentNullException();
            }

            source1.Subscribe(o =>
            {
                _signal1 = true;
                _value1 = o;
                if (_signal1 && _signal2 && _signal3 && (predicate == null || predicate(_value1, _value2, _value3)))
                {
                    TResult result = selector(_value1, _value2, _value3);
                    HandleCallBack(result);
                }
            });
            source2.Subscribe(o =>
            {
                _signal2 = true;
                _value2 = o;
                if (_signal1 && _signal2 && _signal3 && (predicate == null || predicate(_value1, _value2, _value3)))
                {
                    TResult result = selector(_value1, _value2, _value3);
                    HandleCallBack(result);
                }
            });
            source3.Subscribe(o =>
            {
                _signal3 = true;
                _value3 = o;
                if (_signal1 && _signal2 && _signal3 && (predicate == null || predicate(_value1, _value2, _value3)))
                {
                    TResult result = selector(_value1, _value2, _value3);
                    HandleCallBack(result);
                }
            });
        }
    }
}
