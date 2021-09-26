// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Helpers
{
    /// <summary>
    /// 远程UTC时间转换
    /// </summary>
    public class RemoteUtcDateTime
    {
        DateTime _remoteBegin;
        DateTime _localBegin;

        public RemoteUtcDateTime()
        {
            _remoteBegin = _localBegin = DateTime.UtcNow;
        }

        /// <summary>
        /// 远程初始化时间
        /// </summary>
        public DateTime RemoteBegin => _remoteBegin;
        /// <summary>
        /// 客户端初始化时间
        /// </summary>
        public DateTime LocalBegin => _localBegin;
        /// <summary>
        /// 远程当前时间
        /// </summary>
        public DateTime RemoteNow => _remoteBegin + (DateTime.UtcNow - _localBegin);


        /// <summary>
        /// 更新远程当前时间
        /// </summary>
        /// <param name="remoteNow"></param>
        public void UpdateRemoteNow(DateTime remoteNow)
        {
            _remoteBegin = remoteNow;
            _localBegin = DateTime.UtcNow;
        }

        /// <summary>
        /// 更新远程当前时间
        /// </summary>
        /// <param name="remoteNow">远程当前时间</param>
        /// <param name="timeSpanPingPong">客户端计算出的请求与结果之间的时间差，远程时间会增加此值的一半作为参考延迟值</param>
        public void UpdateRemoteNow(DateTime remoteNow, TimeSpan timeSpanPingPong)
        {
            UpdateRemoteNow(remoteNow, timeSpanPingPong.TotalSeconds);
        }
        /// <summary>
        /// 更新远程当前时间
        /// </summary>
        /// <param name="remoteNow"></param>
        /// <param name="timeSpanPingPongSec">客户端计算出的请求与结果之间的时间差，远程时间会增加此值的一半作为参考延迟值</param>
        public void UpdateRemoteNow(DateTime remoteNow, double timeSpanPingPongSec)
        {
            _localBegin = DateTime.UtcNow;

            var latency = TimeSpan.FromSeconds(timeSpanPingPongSec * 0.5f);

            _remoteBegin = remoteNow + latency;
        }



        /// <summary>
        /// 通过本地时间获取远程时间
        /// </summary>
        /// <param name="localTime">本地时间</param>
        /// <returns>返回远程时间</returns>
        public DateTime GetRemoteTime(DateTime localTime)
        {
            return RemoteBegin + (localTime - _localBegin);
        }
        /// <summary>
        /// 通过远程时间获取本地时间
        /// </summary>
        /// <param name="remoteTime">远程时间</param>
        /// <returns>返回本地时间</returns>
        public DateTime GetLocalTime(DateTime remoteTime)
        {
            return _localBegin + (remoteTime - _remoteBegin);
        }

        /// <summary>
        /// 获取远程未来时间戳
        /// </summary>
        /// <param name="restSeconds">剩余数</param>
        /// <returns></returns>
        public DateTime GetRemoteFuture(TimeSpan timeSpan)
        {
            return RemoteNow + timeSpan;
        }
        /// <summary>
        /// 获取剩余时间段
        /// </summary>
        /// <param name="remoteStartTime">远程启动时间戳</param>
        /// <param name="timeSpan">总时间段</param>
        /// <returns></returns>
        public TimeSpan GetRestTimeSpan(DateTime remoteStartTime, TimeSpan timeSpan)
        {
            DateTime finishTime = remoteStartTime + timeSpan;
            return finishTime - RemoteNow;
        }
        /// <summary>
        /// 获取剩余时间段
        /// </summary>
        /// <param name="remoteFinishTime">远程完成时间戳</param>
        /// <returns></returns>
        public TimeSpan GetRestTimeSpan(DateTime remoteFinishTime)
        {
            return remoteFinishTime - RemoteNow;
        }
    }
}
