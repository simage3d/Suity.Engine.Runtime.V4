// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Suity
{
    /// <summary>
    /// 数据读取器
    /// </summary>
    public interface IDataReader
    {
        IDataReader Node(string name);
        IEnumerable<IDataReader> Nodes(string name);

        bool ReadIsEmpty();
        string ReadTypeName();


        bool ReadBoolean();
        byte ReadByte();
        sbyte ReadSByte();
        short ReadInt16();
        ushort ReadUInt16();
        int ReadInt32();
        uint ReadUInt32();
        long ReadInt64();
        ulong ReadUInt64();
        float ReadSingle();
        double ReadDouble();
        string ReadString();
        DateTime ReadDateTime();
        byte[] ReadBytes();
    }
}
