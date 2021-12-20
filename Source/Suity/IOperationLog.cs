// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity
{
    public interface IOperationLog
    {
        void AddOperationLog(int level, string category, string userId, string ip, object data, bool successful);
    }

    public sealed class EmptyOperationLog : IOperationLog
    {
        public static readonly EmptyOperationLog Empty = new EmptyOperationLog();

        private EmptyOperationLog()
        {
        }

        public void AddOperationLog(int level, string category, string userId, string ip, object data, bool successful)
        {
        }
    }
}
