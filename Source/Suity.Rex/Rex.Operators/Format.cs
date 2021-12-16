// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Operators
{
    class Format<T> : RexListenerBase<T, string>
    {
        public Format(IRexListener<T> source, string format)
            : base(source)
        {
            if (format == null)
            {
                throw new ArgumentNullException();
            }

            source.Subscribe(o => 
            {
                string result = string.Format(format, o);
                HandleCallBack(result);
            });
        }
    }

    class Format2<T1, T2> : RexListenerBase<T1, T2, string>
    {
        bool _signal1;
        bool _signal2;

        T1 _value1;
        T2 _value2;

        public Format2(IRexListener<T1> source1, IRexListener<T2> source2, string format)
            : base(source1, source2)
        {
            source1.Subscribe(o =>
            {
                _signal1 = true;
                _value1 = o;
                if (_signal1 && _signal2)
                {
                    HandleCallBack(string.Format(format, _value1, _value2));
                }
            });
            source2.Subscribe(o =>
            {
                _signal2 = true;
                _value2 = o;
                if (_signal1 && _signal2)
                {
                    HandleCallBack(string.Format(format, _value1, _value2));
                }
            });
        }
    }

    class Format3<T1, T2, T3> : RexListenerBase<T1, T2, T3, string>
    {
        bool _signal1;
        bool _signal2;
        bool _signal3;

        T1 _value1;
        T2 _value2;
        T3 _value3;

        public Format3(IRexListener<T1> source1, IRexListener<T2> source2, IRexListener<T3> source3, string format)
            : base(source1, source2, source3)
        {
            source1.Subscribe(o =>
            {
                _signal1 = true;
                _value1 = o;
                if (_signal1 && _signal2 && _signal3)
                {
                    HandleCallBack(string.Format(format, _value1, _value2, _value3));
                }
            });
            source2.Subscribe(o =>
            {
                _signal2 = true;
                _value2 = o;
                if (_signal1 && _signal2 && _signal3)
                {
                    HandleCallBack(string.Format(format, _value1, _value2, _value3));
                }
            });
            source3.Subscribe(o =>
            {
                _signal3 = true;
                _value3 = o;
                if (_signal1 && _signal2 && _signal3)
                {
                    HandleCallBack(string.Format(format, _value1, _value2, _value3));
                }
            });
        }
    }
}
