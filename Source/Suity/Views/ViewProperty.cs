// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.NodeQuery;
using System;

namespace Suity.Views
{
    [Serializable]
    public sealed class ViewProperty
    {
        /// <summary>
        /// 指定编辑器隐藏指定类型的字段
        /// </summary>
        public const string HiddenFieldTypeAttribute = "HiddenFieldType";
        /// <summary>
        /// 指定编辑器隐藏链接类型字段
        /// </summary>
        public const string DataLinkConnectorHiddenAttribute = "DataLinkConnectorHidden";

        public string Name { get; }
        public string Description { get; set; }
        public object Icon { get; set; }

        public int ViewId { get; set; }

        public TextStatus Status { get; set; }
        public bool Expand { get; set; }
        public bool ForceWriteBack { get; set; }
        public bool ReadOnly { get; set; }
        public bool Enabled { get; set; }

        public INodeReader Styles { get; set; }

        public string DisplayName => !string.IsNullOrEmpty(Description) ? Description : Name;

        public ViewProperty()
        {
            Name = string.Empty;
        }
        public ViewProperty(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
        }
        public ViewProperty(string name, string description)
            : this(name)
        {
            Description = description;
        }
        public ViewProperty(string name, string description, object icon)
            : this(name, description)
        {
            Icon = icon;
        }

        public static implicit operator ViewProperty(string name)
        {
            return new ViewProperty(name);
        }
    }
}
