// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity
{
    /// <summary>
    /// 数据冲突解决方式
    /// </summary>
    public enum DataConflictMode
    {
        Default = 0,
        Override = 1,
        Ignore = 2,
        Throw = 3,
    }
}
