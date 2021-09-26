// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity
{
    public sealed class EmptyDataWriter : IDataWriter
    {
        public static readonly EmptyDataWriter Empty = new EmptyDataWriter();

        private EmptyDataWriter() { }

        #region IDataWriter 成员

        public IDataWriter Node(string name)
        {
            return EmptyDataWriter.Empty;
        }

        public IDataArrayWriter Nodes(string name, int count)
        {
            return EmptyDataArrayWriter.Empty;
        }

        public void WriteEmpty(bool empty)
        {
        }

        public void WriteTypeName(string typeName)
        {
        }

        public void WriteBoolean(bool value)
        {
        }

        public void WriteByte(byte value)
        {
        }

        public void WriteSByte(sbyte value)
        {
        }

        public void WriteInt16(short value)
        {
        }

        public void WriteUInt16(ushort value)
        {
        }

        public void WriteInt32(int value)
        {
        }

        public void WriteUInt32(uint value)
        {
        }

        public void WriteInt64(long value)
        {
        }

        public void WriteUInt64(ulong value)
        {
        }

        public void WriteSingle(float value)
        {
        }

        public void WriteDouble(double value)
        {
        }

        public void WriteString(string value)
        {
        }

        public void WriteDateTime(DateTime value)
        {
        }

        public void WriteBytes(byte[] b, int offset, int length)
        {
        }

        #endregion
    }

    public sealed class EmptyDataArrayWriter : IDataArrayWriter
    {
        public static readonly EmptyDataArrayWriter Empty = new EmptyDataArrayWriter();

        private EmptyDataArrayWriter() { }

        #region IDataArrayWriter 成员

        public IDataWriter Item()
        {
            return EmptyDataWriter.Empty;
        }

        public void Finish()
        {
        }

        #endregion
    }
}
