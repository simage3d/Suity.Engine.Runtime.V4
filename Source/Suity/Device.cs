// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    /// <summary>
    /// 启动设备
    /// </summary>
    public abstract class Device : IServiceProvider
    {
        /// <summary>
        /// 设备所处的位置
        /// </summary>
        public abstract string Location { get; }

        /// <summary>
        /// 设备当前运行时间
        /// </summary>
        public abstract float Time { get; }


        public abstract void ObjectCreate(Object obj);

        public abstract void QueueAction(Action action);

        public abstract void AddLog(LogMessageType type, object message);

        public abstract void AddNetworkLog(LogMessageType type, NetworkDirection direction, string sessionId, string channelId, object message);

        public abstract void AddOperationLog(int level, string category, string userId, string ip, object data, bool successful);

        public abstract void AddResourceLog(string key, string path);

        public abstract void AddEntityLog(long roomId, long entityId, string entityName, EntityActionTypes actionType, LogMessageType messageType, object value);

        public abstract string GetEnvironmentConfig(string key);

        public abstract object GetService(Type serviceType);
    }

    internal sealed class DefaultDevice : Device
    {
        public static readonly DefaultDevice Default = new DefaultDevice();

        readonly DateTime _startTime = DateTime.UtcNow;

        private DefaultDevice()
        {
        }

        public override string Location => string.Empty;

        public override float Time => (float)(DateTime.UtcNow - _startTime).TotalSeconds;

        public override void ObjectCreate(Object obj)
        {
        }

        public override void QueueAction(Action action)
        {
            action?.Invoke();
        }

        public override void AddLog(LogMessageType type, object message)
        {
        }

        public override void AddNetworkLog(LogMessageType type, NetworkDirection direction, string sessionId, string channelId, object message)
        {
        }

        public override void AddOperationLog(int level, string category, string userId, string ip, object data, bool successful)
        {
        }

        public override void AddResourceLog(string key, string path)
        {
        }

        public override void AddEntityLog(long roomId, long entityId, string entityName, EntityActionTypes actionType, LogMessageType messageType, object value)
        {
        }

        public override string GetEnvironmentConfig(string key)
        {
            return null;
        }

        public override object GetService(Type serviceType)
        {
            return null;
        }
    }
}
