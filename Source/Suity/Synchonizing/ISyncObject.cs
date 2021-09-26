// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Synchonizing
{
    /// <summary>
    /// 同步对象
    /// </summary>
    public interface ISyncObject
    {
        /// <summary>
        /// 执行同步
        /// </summary>
        /// <param name="sync"></param>
        /// <param name="context"></param>
        void Sync(IPropertySync sync, ISyncContext context);
    }
}
