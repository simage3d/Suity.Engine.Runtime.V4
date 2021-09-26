// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public class RemoteDateTime
    {
        DateTime _remoteBegin;
        DateTime _localBegin;

        public RemoteDateTime()
        {
            _remoteBegin = _localBegin = DateTime.UtcNow;
        }
        public RemoteDateTime(DateTime remoteNow)
        {
            _remoteBegin = remoteNow;
            _localBegin = DateTime.UtcNow;
        }
        public RemoteDateTime(DateTime remoteBegin, DateTime localBegin)
        {
            _remoteBegin = remoteBegin;
            _localBegin = localBegin;
        }

        /// <summary>
        /// 服务端初始化时间
        /// </summary>
        public DateTime RemoteBegin => _remoteBegin;
        /// <summary>
        /// 客户端初始化时间
        /// </summary>
        public DateTime LocalBegin => _localBegin;

        /// <summary>
        /// 服务端当前时间
        /// </summary>
        public DateTime RemoteNow => _remoteBegin + (DateTime.UtcNow - _localBegin);


        /// <summary>
        /// 更新服务端当前时间
        /// </summary>
        /// <param name="remoteNow"></param>
        public void UpdateRemoteNow(DateTime remoteNow)
        {
            _remoteBegin = remoteNow;
            _localBegin = DateTime.UtcNow;
        }

        /// <summary>
        /// 通过本地时间获取服务端时间
        /// </summary>
        /// <param name="localTime">本地时间</param>
        /// <returns>返回服务端时间</returns>
        public DateTime GetRemoteTime(DateTime localTime)
        {
            return RemoteBegin + (localTime - _localBegin);
        }
        /// <summary>
        /// 通过服务端时间获取本地时间
        /// </summary>
        /// <param name="remoteTime">服务端时间</param>
        /// <returns>返回本地时间</returns>
        public DateTime GetLocalTime(DateTime remoteTime)
        {
            return _localBegin + (remoteTime - _remoteBegin);
        }

        /// <summary>
        /// 获取服务端未来时间戳
        /// </summary>
        /// <param name="restSeconds">剩余数</param>
        /// <returns></returns>
        public DateTime GetRemoteFuture(int restSeconds)
        {
            return RemoteNow + TimeSpan.FromSeconds(restSeconds);
        }
        /// <summary>
        /// 获取剩余时间段
        /// </summary>
        /// <param name="remoteStartTime">服务端启动时间戳</param>
        /// <param name="seconds">总时间段</param>
        /// <returns></returns>
        public TimeSpan GetRestTimeSpan(DateTime remoteStartTime, int seconds)
        {
            DateTime finishTime = remoteStartTime + TimeSpan.FromSeconds(seconds);
            return finishTime - RemoteNow;
        }
        /// <summary>
        /// 获取剩余时间段
        /// </summary>
        /// <param name="remoteStartTime">服务端启动时间戳</param>
        /// <param name="span">总时间段</param>
        /// <returns></returns>
        public TimeSpan GetRestTimeSpan(DateTime remoteStartTime, TimeSpan span)
        {
            DateTime finishTime = remoteStartTime + span;
            return finishTime - RemoteNow;
        }
        /// <summary>
        /// 获取剩余时间段
        /// </summary>
        /// <param name="remoteFinishTime">服务端完成时间戳</param>
        /// <returns></returns>
        public TimeSpan GetRestTimeSpan(DateTime remoteFinishTime)
        {
            return remoteFinishTime - RemoteNow;
        }
    }
}
