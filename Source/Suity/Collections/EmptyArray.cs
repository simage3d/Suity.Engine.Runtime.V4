// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Collections
{
#pragma warning disable RCS1102 // Make class static.
    public sealed class EmptyArray<T>
#pragma warning restore RCS1102 // Make class static.
    {
        public static readonly T[] Empty = new T[0];
    }
}
