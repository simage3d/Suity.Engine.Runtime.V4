
// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Networking;
using System;

namespace Suity
{
    public static class Logs
    {
        public static void LogDebug(object message) 
            => Suity.Environment._device.AddLog(LogMessageType.Debug, message);

        public static void LogInfo(object message) 
            => Suity.Environment._device.AddLog(LogMessageType.Info, message);

        public static void LogWarning(object message)
            => Suity.Environment._device.AddLog(LogMessageType.Warning, message);

        public static void LogError(object message)
            => Suity.Environment._device.AddLog(LogMessageType.Error, message);

        public static void AddLog(LogMessageType type, object message) 
            => Suity.Environment._device.AddLog(type, message);

        public static void AddNetworkLog(LogMessageType type, NetworkDirection direction, string sessionId, string channelId, object message)
            => Suity.Environment._device.AddNetworkLog(type, direction, sessionId, channelId, message);

        public static void AddResourceLog(string key, string path) 
            => Suity.Environment._device.AddResourceLog(key, path);

        public static void AddEntityLog(long roomId, long entityId, string entityName, EntityActionTypes actionType, LogMessageType messageType, object value)
            => Suity.Environment._device.AddEntityLog(roomId, entityId, entityName, actionType, messageType, value);

        public static void AddOperationLog(int level, string category, string userId, string ip, object data, bool successful)
            => Suity.Environment._device.AddOperationLog(level, category, userId, ip, data, successful);
    }

}
