// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Views
{
    /// <summary>
    /// 设值检查
    /// </summary>
    public interface IDropInCheck
    {
        /// <summary>
        /// 获取指定的值对于此次操作是否合法
        /// </summary>
        /// <param name="value">要拖拽的对象</param>
        /// <returns>返回是否可拖拽</returns>
        bool CanDropIn(object value);

        /// <summary>
        /// 将传入的值转换成当前适合的值。当<see cref="CanDropIn"/>通过后，开始传值前执行。
        /// </summary>
        /// <param name="value">拖拽传入的值</param>
        /// <returns>转换后的值，若无需转换，直接返回value。</returns>
        object DropInConvert(object value);
    }
}
