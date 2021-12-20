// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public interface IEntityLog
    {
        void AddEntityLog(long roomId, long entityId, string entityName, EntityActionTypes actionType, LogMessageType messageType, object value);
    }

    public sealed class EmptyEntityLog : IEntityLog
    {
        public static readonly EmptyEntityLog Empty = new EmptyEntityLog();

        private EmptyEntityLog()
        {
        }

        public void AddEntityLog(long roomId, long entityId, string entityName, EntityActionTypes actionType, LogMessageType messageType, object value)
        {
        }
    }
}
