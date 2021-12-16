// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Rex.Operators;

namespace Suity.Rex.VirtualDom
{
    public static class RexTreeExtensions
    {
        #region Listener
        public static IRexListener<T> AsRexListener<T>(this RexTree engine, RexPath path, string tag = null)
        {
            return new RexTreeListener<T>(engine, path, tag);
        }
        public static IRexListener<IEnumerable<T>> AsRexListeners<T>(this RexTree engine, RexPath path, string tag = null)
        {
            return new RexTreeListener<IEnumerable<T>>(engine, path, tag);
        }

        public static IRexListener<T> AsRexBeforeListener<T>(this RexTree engine, RexPath path, string tag = null)
        {
            return new RexTreeBeforeListener<T>(engine, path, tag);
        }
        public static IRexListener<IEnumerable<T>> AsRexBeforeListeners<T>(this RexTree engine, RexPath path, string tag = null)
        {
            return new RexTreeBeforeListener<IEnumerable<T>>(engine, path, tag);
        }

        public static IRexListener<T> AsRexAfterListener<T>(this RexTree engine, RexPath path, string tag = null)
        {
            return new RexTreeAfterListener<T>(engine, path, tag);
        }
        public static IRexListener<IEnumerable<T>> AsRexAfterListeners<T>(this RexTree engine, RexPath path, string tag = null)
        {
            return new RexTreeAfterListener<IEnumerable<T>>(engine, path, tag);
        }

        #endregion

        #region Define

        public static T GetData<T>(this RexTree model, RexPropertyDefine<T> property)
        {
            return model.GetData<T>(property.Path);
        }
        public static void SetData<T>(this RexTree model, RexPropertyDefine<T> property, T value)
        {
            model.SetData<T>(property.Path, value);
        }
        public static void SetDataDeep<T>(this RexTree model, RexPropertyDefine<T> property, T value)
        {
            model.SetDataDeep<T>(property.Path, value);
        }
        public static void SetDefaultData<T>(this RexTree model, RexPropertyDefine<T> property, T value)
        {
            model.SetDefaultData<T>(property.Path, value);
        }
        public static void UpdateData<T>(this RexTree model, RexPropertyDefine<T> property)
        {
            model.UpdateData(property.Path);
        }
        public static void DoAction(this RexTree model, RexActionDefine action)
        {
            model.DoAction(action.Path);
        }
        public static void DoAction<T>(this RexTree model, RexActionDefine<T> action, T arg)
        {
            model.DoAction<T>(action.Path, arg);
        }
        public static void DoAction<T1, T2>(this RexTree model, RexActionDefine<T1, T2> action, T1 arg1, T2 arg2)
        {
            model.DoAction<T1, T2>(action.Path, arg1, arg2);
        }
        public static void DoAction<T1, T2, T3>(this RexTree model, RexActionDefine<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            model.DoAction<T1, T2, T3>(action.Path, arg1, arg2, arg3);
        }
        public static void DoAction<T1, T2, T3, T4>(this RexTree model, RexActionDefine<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            model.DoAction<T1, T2, T3, T4>(action.Path, arg1, arg2, arg3, arg4);
        }

        #endregion

        #region IRexInstance
        public static IRexListener<T> AsRexListener<T>(this IRexTreeInstance<T> property, string tag = null)
        {
            return new RexTreeListener<T>(property.Tree, property.Path, tag);
        }
        public static IRexListener<IEnumerable<T>> AsRexListeners<T>(this IRexTreeInstance<IEnumerable<T>> property, string tag = null)
        {
            return new RexTreeListener<IEnumerable<T>>(property.Tree, property.Path, tag);
        }

        public static IRexListener<T> AsRexBeforeListener<T>(this IRexTreeInstance<object> property, string tag = null)
        {
            return new RexTreeBeforeListener<T>(property.Tree, property.Path, tag);
        }
        public static IRexListener<IEnumerable<T>> AsRexBeforeListeners<T>(this IRexTreeInstance<object> property, string tag = null)
        {
            return new RexTreeBeforeListener<IEnumerable<T>>(property.Tree, property.Path, tag);
        }

        public static IRexListener<T> AsRexAfterListener<T>(this IRexTreeInstance<object> property, string tag = null)
        {
            return new RexTreeAfterListener<T>(property.Tree, property.Path, tag);
        }
        public static IRexListener<IEnumerable<T>> AsRexAfterListeners<T>(this IRexTreeInstance<object> property, string tag = null)
        {
            return new RexTreeAfterListener<IEnumerable<T>>(property.Tree, property.Path, tag);
        }
        #endregion

        #region IRexDefine
        public static IRexListener<T> AsRexListener<T>(this IRexTreeDefine<T> property, RexTree tree, string tag = null)
        {
            return new RexTreeListener<T>(tree, property.Path, tag);
        }
        public static IRexListener<IEnumerable<T>> AsRexListeners<T>(this IRexTreeDefine<IEnumerable<T>> property, RexTree tree, string tag = null)
        {
            return new RexTreeListener<IEnumerable<T>>(tree, property.Path, tag);
        }

        public static IRexListener<T> AsRexBeforeListener<T>(this IRexTreeDefine<object> property, RexTree tree, string tag = null)
        {
            return new RexTreeBeforeListener<T>(tree, property.Path, tag);
        }
        public static IRexListener<IEnumerable<T>> AsRexBeforeListeners<T>(this IRexTreeDefine<object> property, RexTree tree, string tag = null)
        {
            return new RexTreeBeforeListener<IEnumerable<T>>(tree, property.Path, tag);
        }

        public static IRexListener<T> AsRexAfterListener<T>(this IRexTreeDefine<object> property, RexTree tree, string tag = null)
        {
            return new RexTreeAfterListener<T>(tree, property.Path, tag);
        }
        public static IRexListener<IEnumerable<T>> AsRexAfterListeners<T>(this IRexTreeDefine<object> property, RexTree tree, string tag = null)
        {
            return new RexTreeAfterListener<IEnumerable<T>>(tree, property.Path, tag);
        }
        #endregion

        #region Operation
        public static IRexListener<T> WhenData<T, TData>(this IRexListener<T> source, RexTree model, RexPath path, Predicate<TData> predicate)
        {
            return new WhenData<T, TData>(source, model, path, predicate);
        }
        public static IRexListener<T> WhenProperty<T, TData>(this IRexListener<T> source, IRexProperty<TData> property, Predicate<TData> predicate)
        {
            return new WhenProperty<T, TData>(source, property, predicate);
        }

        public static IRexListener<TResult> SetDataTo<TSource, TResult>(this IRexListener<TSource> source, RexTree model, Func<TSource, RexPath> pathFunc, Func<TSource, TResult> dataFunc)
        {
            return new SetDataTo<TSource, TResult>(source, model, pathFunc, dataFunc);
        }
        public static IRexListener<TResult> SetDataTo<TSource, TResult>(this IRexListener<TSource> source, RexTree model, RexPath path, Func<TSource, TResult> dataFunc)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            return new SetDataTo<TSource, TResult>(source, model, o => path, dataFunc);
        }
        public static IRexListener<TResult> SetDataTo<TSource, TResult>(this IRexListener<TSource> source, IRexTreeInstance<TResult> target, Func<TSource, TResult> dataFunc)
        {
            return new SetDataTo<TSource, TResult>(source, target.Tree, o => target.Path, dataFunc);
        }
        public static IRexListener<T> SetDataTo<T>(this IRexListener<T> source, RexTree model, Func<T, RexPath> pathFunc)
        {
            return new SetDataTo<T, T>(source, model, pathFunc, o => o);
        }
        public static IRexListener<T> SetDataTo<T>(this IRexListener<T> source, RexTree model, RexPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            return new SetDataTo<T, T>(source, model, o => path, o => o);
        }
        public static IRexListener<T> SetDataTo<T>(this IRexListener<T> source, IRexTreeInstance<T> target)
        {
            return new SetDataTo<T, T>(source, target.Tree, o => target.Path, o => o);
        }


        public static IRexListener<T> MapUpdateTo<T>(this IRexListener<T> source, RexTree model, Func<T, RexPath> pathFunc)
        {
            return new MapUpdateTo<T>(source, model, pathFunc);
        }
        public static IRexListener<T> MapUpdateTo<T>(this IRexListener<T> source, RexTree model, RexPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            return new MapUpdateTo<T>(source, model, o => path);
        }
        public static IRexListener<T> MapUpdateTo<T>(this IRexListener<T> source, IRexTreeInstance<T> target)
        {
            return new MapUpdateTo<T>(source, target.Tree, o => target.Path);
        }

        public static IRexListener<T> MapActionTo<T>(this IRexListener<T> source, RexTree model, RexPath path) where T : ActionArguments
        {
            return new MapActionTo<T>(source, model, path);
        }
        public static IRexListener<T> MapActionTo<T>(this IRexListener<T> source, IRexTreeInstance<T> action) where T : ActionArguments
        {
            return new MapActionTo<T>(source, action.Tree, action.Path);
        } 
        #endregion
    }
}
