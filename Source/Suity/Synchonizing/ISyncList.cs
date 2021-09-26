// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Synchonizing
{
    /// <summary>
    /// 同步列表
    /// </summary>
    public interface ISyncList
    {
        int Count { get; }
        void Sync(IIndexSync sync, ISyncContext context);
    }
}
