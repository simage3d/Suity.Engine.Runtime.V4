// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity
{
    /// <summary>
    /// 数据写入器
    /// </summary>
    public interface IDataWriter
    {
        IDataWriter Node(string name);
        IDataArrayWriter Nodes(string name, int count);

        void WriteEmpty(bool empty);
        void WriteTypeName(string typeName);

        void WriteBoolean(bool value);
        void WriteByte(byte value);
        void WriteSByte(sbyte value);
        void WriteInt16(short value);
        void WriteUInt16(ushort value);
        void WriteInt32(int value);
        void WriteUInt32(uint value);
        void WriteInt64(long value);
        void WriteUInt64(ulong value);
        void WriteSingle(float value);
        void WriteDouble(double value);
        void WriteString(string value);
        void WriteDateTime(DateTime value);
        void WriteBytes(byte[] b, int offset, int length);
    }

    public interface IDataArrayWriter
    {
        IDataWriter Item();
        void Finish();
    }
}
