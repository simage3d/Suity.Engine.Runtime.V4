// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity
{
    /// <summary>
    /// 函数信息
    /// </summary>
    public class FunctionInfo
    {
        public readonly string Name;

        public readonly ObjectType.FunctionDelegate Handler;

        public FunctionInfo(string name, ObjectType.FunctionDelegate handler)
        {
            Name = name;
            Handler = handler;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
