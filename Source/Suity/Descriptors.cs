// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    /// <summary>
    /// 类型信息描述
    /// </summary>
    public class TypeInfoDescriptor
    {
        public string Kind;

        public string Description;

        public string Icon;

        public string Category;

        public string Side;

        public bool IsValueType;

        public List<FieldDescriptor> Fields = new List<FieldDescriptor>();
    }

    /// <summary>
    /// 字段描述
    /// </summary>
    public class FieldDescriptor
    {
        public string Name;

        public string Description;

        public string Type;
    }

}
