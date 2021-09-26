// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity
{
    /// <summary>
    /// 用于解析类型到资源类型的映射
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AssetDefinitionTypeAttribute : Attribute
    {
        public string AssetTypeName { get; }

        public AssetDefinitionTypeAttribute(string assetTypeName)
        {
            AssetTypeName = assetTypeName;
        }
    }
}
