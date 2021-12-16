// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Rex.Operators;

namespace Suity.Rex
{
    public static class RexExtensions
    {
        #region Event

        public static IRexListener<object> AsRexListener(this IRexEvent rexEvent)
        {
            return new EventListener(rexEvent);
        }

        public static IRexListener<T> AsRexListener<T>(this IRexEvent<T> rexEvent)
        {
            return new EventListener<T>(rexEvent);
        }

        public static IDisposable MapTo<T>(this IRexListener<T> listener, RexEventHandle rexEventHandle)
        {
            listener.Subscribe(o => rexEventHandle.Invoke());
            return listener;
        }

        public static IDisposable MapTo<T>(this IRexListener<T> listener, RexEventHandle<T> rexEventHandle)
        {
            listener.Subscribe(o => rexEventHandle.Invoke(o));
            return listener;
        }

        #endregion

        #region Value

        public static IRexListener<T> AsRexListener<T>(this IRexValue<T> rexValue)
        {
            return new RexValueListener<T>(rexValue);
        }

        #endregion

        #region Operator
        public static IRexListener<TResult> SelectIf<TResult>(this IRexListener<bool> source, TResult truePart, TResult falsePart)
        {
            return new SelectIf<TResult>(source, truePart, falsePart);
        }
        public static IRexListener<TResult> IfHasValue<TResult>(this IRexListener<object> source, TResult truePart, TResult falsePart)
        {
            return new IfHasValue<TResult>(source, truePart, falsePart);
        }
        public static IRexListener<T> Where<T>(this IRexListener<T> source, Predicate<T> predicate)
        {
            return new Where<T>(source, predicate);
        }
        public static IRexListener<TResult> Select<TSource, TResult>(this IRexListener<TSource> source, Func<TSource, TResult> selector)
        {
            return new Select<TSource, TResult>(source, selector);
        }
        public static IRexListener<TResult> SelectMany<TSource, TResult>(this IRexListener<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            return new SelectMany<TSource, TResult>(source, selector);
        }
        public static IRexListener<TResult> OfType<TSource, TResult>(this IRexListener<TSource> source) where TResult : class
        {
            return new OfType<TSource, TResult>(source);
        }
        public static IRexListener<T> NotNull<T>(this IRexListener<T> source) where T : class
        {
            return new NotNull<T>(source);
        }

        public static IRexListener<TResult> Cast<TSource, TResult>(this IRexListener<TSource> source)
        {
            return new Cast<TSource, TResult>(source);
        }
        public static IRexListener<T> Each<T>(this IRexListener<IEnumerable<T>> source)
        {
            return new Each<T>(source);
        }
        public static IRexListener<T> Each<T>(this IRexListener<List<T>> source)
        {
            return new Each<T>(source.Select(o => (IEnumerable<T>)o));
        }
        public static IRexListener<T> Each<T>(this IRexListener<IList<T>> source)
        {
            return new Each<T>(source.Select(o => (IEnumerable<T>)o));
        }
        public static IRexListener<T> Each<T>(this IRexListener<T[]> source)
        {
            return new Each<T>(source.Select(o => (IEnumerable<T>)o));
        }
        public static IRexListener<T> First<T>(this IRexListener<IEnumerable<T>> source)
        {
            return new First<T>(source);
        }
        public static IRexListener<T> FirstOrDefault<T>(this IRexListener<IEnumerable<T>> source)
        {
            return new FirstOrDefault<T>(source);
        }
        public static IRexListener<IEnumerable<T>> Take<T>(this IRexListener<IEnumerable<T>> source, int count)
        {
            return new Take<T>(source, count);
        }
        public static IRexListener<IEnumerable<T>> Skip<T>(this IRexListener<IEnumerable<T>> source, int count)
        {
            return new Skip<T>(source, count);
        }


        public static IRexListener<TResult> ToDataObject<TResult>(this IRexListener<string> source) where TResult : class
        {
            return new ToDataObject<TResult>(source);
        }
        public static IRexListener<string> ToDataId<TSource>(this IRexListener<TSource> source) where TSource : class
        {
            return new ToDataId<TSource>(source);
        }


        public static IRexListener<T> Queued<T>(this IRexListener<T> source)
        {
            return new Queued<T>(source);
        }

        public static IRexHandle Subscribe<T>(this IRexListener<T> source, Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            return source.Subscribe(_ => action());
        }

        public static IRexListener<TResult> Combine<TSource1, TSource2, TResult>(
            this IRexListener<TSource1> source1,
            IRexListener<TSource2> source2,
            Func<TSource1, TSource2, bool> predicate,
            Func<TSource1, TSource2, TResult> selector
            )
        {
            return new Combine<TSource1, TSource2, TResult>(source1, source2, predicate, selector);
        }
        public static IRexListener<TResult> Combine<TSource1, TSource2, TSource3, TResult>(
            this IRexListener<TSource1> source1,
            IRexListener<TSource2> source2,
            IRexListener<TSource3> source3,
            Func<TSource1, TSource2, TSource3, bool> predicate,
            Func<TSource1, TSource2, TSource3, TResult> selector
            )
        {
            return new Combine<TSource1, TSource2, TSource3, TResult>(source1, source2, source3, predicate, selector);
        }
        public static IRexListener<TResult> Combine<TSource1, TSource2, TResult>(
            this IRexListener<TSource1> source1,
            IRexListener<TSource2> source2,
            Func<TSource1, TSource2, TResult> selector
            )
        {
            return new Combine<TSource1, TSource2, TResult>(source1, source2, null, selector);
        }
        public static IRexListener<TResult> Combine<TSource1, TSource2, TSource3, TResult>(
            this IRexListener<TSource1> source1,
            IRexListener<TSource2> source2,
            IRexListener<TSource3> source3,
            Func<TSource1, TSource2, TSource3, TResult> selector
            )
        {
            return new Combine<TSource1, TSource2, TSource3, TResult>(source1, source2, source3, null, selector);
        }


        public static IRexListener<bool> And(this IRexListener<bool> source1, IRexListener<bool> source2)
        {
            return new And(source1, source2);
        }
        public static IRexListener<bool> Or(this IRexListener<bool> source1, IRexListener<bool> source2)
        {
            return new Or(source1, source2);
        }

        public static IRexListener<string> Format<T>(this IRexListener<T> source, string format)
        {
            return new Format<T>(source, format);
        }
        public static IRexListener<string> Format<T1, T2>(this IRexListener<T1> source1, IRexListener<T2> source2, string format)
        {
            return new Format2<T1, T2>(source1, source2, format);
        }
        public static IRexListener<string> Format<T1, T2, T3>(this IRexListener<T1> source1, IRexListener<T2> source2, IRexListener<T3> source3, string format)
        {
            return new Format3<T1, T2, T3>(source1, source2, source3, format);
        }

        #endregion

        #region Snippets

        public static IRexListener<bool> If(this IRexListener<bool> source, Action trueAction = null, Action falseAction = null)
        {
            source.Subscribe(v =>
            {
                if (v)
                {
                    trueAction?.Invoke();
                }
                else
                {
                    falseAction?.Invoke();
                }
            });
            return source;
        }
        public static IRexListener<bool> IfTrue(this IRexListener<bool> source, Action action)
        {
            source.Subscribe(v => 
            {
                if (v)
                {
                    action();
                }
            });
            return source;
        }
        public static IRexListener<bool> IfFalse(this IRexListener<bool> source, Action action)
        {
            source.Subscribe(v =>
            {
                if (!v)
                {
                    action();
                }
            });
            return source;
        }

        #endregion

        #region Dispose

        public static IDisposable OnDispose(this IDisposable disposable, Action callBack)
        {
            return new DisposableWrapper(disposable, callBack);
        }

        public static IRexHandle OnRexDispose(this IRexHandle handle, Action disposeCallBack)
        {
            return new RexHandleWrapper(handle, disposeCallBack);
        }

        #endregion
    }
}
