// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Suity.Helpers;

namespace Suity
{
    [AssetDefinitionType(AssetDefinitionCodes.Enum)]
    public class EnumTypeInfo : TypeInfo
    {

        //TODO: EnumTypeInfo里面可以增加ReadByString/ReadByNumber设置，优化网络传输

        public EnumTypeInfo(Type type, string key, string aliasName)
            : base(type, key, aliasName)
        {
        }
        public EnumTypeInfo(Type type, string key, string aliasName, PacketFormats packetFormat)
            : base(type, key, aliasName)
        {
        }

        public override bool IsEnum => true;

    }
}
