// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Clustering;
using Suity.Networking;

namespace Suity.Engine.Debugging
{
    class DebugDevice : Device
    {
        public static readonly DebugDevice Instance = new DebugDevice();

        internal DebugApplication _app;
        readonly DateTime _startTime = DateTime.UtcNow;


        private DebugDevice()
        {
        }

        public override string Location => _app?.ServiceId;

        public override float Time => (float)(DateTime.UtcNow - _startTime).TotalSeconds;

        public override void AddLog(LogMessageType type, object message)
        {
            ConsoleRuntimeLog.Instance.AddLog(type, message);
        }

        public override void AddNetworkLog(LogMessageType type, NetworkDirection direction, string sessionId, string channelId, object message)
        {
            if (Debugger.NetworkLog)
            {
                ConsoleNetworkLog.Instance.AddNetworkLog(type, direction, sessionId, channelId, message);
            }
        }

        public override void AddOperationLog(int level, string category, string userId, string ip, object data, bool successful)
        {
        }

        public override void AddResourceLog(string key, string path)
        {
        }
        public override void AddEntityLog(long roomId, long entityId, string entityName, EntityActionTypes actionType, LogMessageType messageType, object component)
        {
        }

        public override string GetEnvironmentConfig(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            var configInfo = _app?._startInfo.Configs.Where(o => o.Name == key).FirstOrDefault();
            return configInfo?.Value;
        }

        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IRuntimeLog))
            {
                return ConsoleRuntimeLog.Instance;
            }
            if (serviceType == typeof(INetworkLog))
            {
                return Debugger.NetworkLog ? (INetworkLog)ConsoleNetworkLog.Instance : EmptyNetworkLog.Empty;
            }

            return _app?.GetService(serviceType);
        }

        public override void ObjectCreate(Object obj)
        {
        }

        public override void QueueAction(Action action)
        {
            if  (_app != null)
            {
                _app.QueueAction(action);
            }
            else
            {
                action?.Invoke();
            }
        }
    }
}
