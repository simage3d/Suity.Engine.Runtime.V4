// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Synchonizing
{
    public interface IIndexSync
    {
        SyncMode Mode { get; }
        SyncIntent Intent { get; }
        int Count { get; }
        int Index { get; }
        object Value { get; }

        T Sync<T>(int index, T obj, SyncFlag flag = SyncFlag.None);
        string SyncAttribute(string name, string value);
    }
}
