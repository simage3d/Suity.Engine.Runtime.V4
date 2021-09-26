// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Engine
{
    public interface IResourceLog
    {
        void AddResourceLog(string key, string path);
    }

    public sealed class EmptyResourceLog : IResourceLog
    {
        public static readonly EmptyResourceLog Empty = new EmptyResourceLog();

        private EmptyResourceLog()
        {
        }

        public void AddResourceLog(string key, string path)
        {
        }
    }
}
