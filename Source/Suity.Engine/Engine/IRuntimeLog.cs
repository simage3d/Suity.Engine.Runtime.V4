// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Engine
{
    public interface IRuntimeLog
    {
        void AddLog(LogMessageType type, object message);
    }

    public sealed class EmptyRuntimeLog : IRuntimeLog
    {
        public static readonly EmptyRuntimeLog Empty = new EmptyRuntimeLog();

        private EmptyRuntimeLog()
        {
        }

        public void AddLog(LogMessageType type, object message)
        {
        }
    }
}
