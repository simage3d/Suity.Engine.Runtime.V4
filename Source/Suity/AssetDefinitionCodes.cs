// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    /// <summary>
    /// 资源定义代码
    /// </summary>
    public static class AssetDefinitionCodes
    {
        public const string AssetLinkPrefix = "*AssetLink";


        //TODO: DTypeLibrary 改为 DTypeFamily
        public const string TypeLibrary = "DTypeLibrary";
        public const string Type = "DType";

        public const string Enum = "DEnum";
        public const string Struct = "DStruct";
        public const string Side = "DSide";
        public const string Event = "DEvent";
        public const string Function = "DFunction";
        public const string AbstractFunction = "DAbstractFunction";
        public const string Delegate = "DDelegate";
        public const string ActionNode = "DActionNode";
        public const string NativeValueType = "DNativeValueType";

        public const string LogicModule = "LogicModule";

        public const string Data = "KeyLink";
        public const string DataFamily = "DataFamily";

        public const string Value = "Value";
        public const string ValueFamily = "ValueFamily";


        public const string Asset = "DAssetLink";

        public const string Controller = "DController";


        public static string MaskAssetTypeName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return KeyCode.Combine(AssetLinkPrefix, name);
        }
    }
}
