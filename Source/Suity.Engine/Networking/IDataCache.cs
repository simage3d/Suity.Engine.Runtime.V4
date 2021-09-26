// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    [Obsolete]
    public interface IDataCache<T> where T : class
    {
        /// <summary>
        /// 获取缓存玩家，不访问数据库。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Get(long id);
        /// <summary>
        /// 获取玩家，如果缓存不存在则自动从数据库中下载。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetOrLoad(long id);
        /// <summary>
        /// 获取玩家，如果缓存不存在则自动从数据库中下载，如果数据库中不存在则自动创建新玩家并写入数据库。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetOrCreateNew(long id, Func<T> creation);
        /// <summary>
        /// 移除返回一个玩家，并保存到数据库。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Remove(long id);
        int Count { get; }
        void Foreach(Action<T> action);
        bool Save(long id);
        bool SaveAll();
        void HealthCheck();
    }

    /// <summary>
    /// 数据缓存
    /// </summary>
    /// <typeparam name="TKey">数据Id</typeparam>
    /// <typeparam name="TValue">数据</typeparam>
    public interface IDataCache<TKey, TValue>  where TValue : class
    {
        /// <summary>
        /// 获取缓存，不访问数据库。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TValue Get(TKey id);
        /// <summary>
        /// 获取数据，如果缓存不存在则自动从数据库中下载。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TValue GetOrLoad(TKey id);
        /// <summary>
        /// 获取数据，如果缓存不存在则自动从数据库中下载，如果数据库中不存在则自动创建新数据并写入数据库。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TValue GetOrCreateNew(TKey id, Func<TValue> creation);
        /// <summary>
        /// 移除返回一项数据，并保存到数据库。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TValue Remove(TKey id);
        int Count { get; }
        void Foreach(Action<TValue> action);
        bool Save(TKey id);
        bool SaveAll();
        void HealthCheck();
    }

}
