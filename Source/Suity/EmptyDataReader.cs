// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using Suity.Collections;

namespace Suity
{
    public sealed class EmptyDataReader : IDataReader
    {
        public static readonly EmptyDataReader Empty = new EmptyDataReader();

        private EmptyDataReader() { }

        #region IDataReader 成员

        public IDataReader Node(string name)
        {
            return this;
        }

        public IEnumerable<IDataReader> Nodes(string name)
        {
            yield break;
        }

        public bool ReadIsEmpty()
        {
            return true;
        }

        public string ReadTypeName()
        {
            return null;
        }

        public bool ReadBoolean()
        {
            return default(bool);
        }

        public byte ReadByte()
        {
            return default(byte);
        }

        public sbyte ReadSByte()
        {
            return default(sbyte);
        }

        public short ReadInt16()
        {
            return default(short);
        }

        public ushort ReadUInt16()
        {
            return default(ushort);
        }

        public int ReadInt32()
        {
            return default(int);
        }

        public uint ReadUInt32()
        {
            return default(uint);
        }

        public long ReadInt64()
        {
            return default(long);
        }

        public ulong ReadUInt64()
        {
            return default(ulong);
        }

        public float ReadSingle()
        {
            return default(float);
        }

        public double ReadDouble()
        {
            return default(double);
        }

        public string ReadString()
        {
            return default(string);
        }

        public DateTime ReadDateTime()
        {
            return default(DateTime);
        }

        public byte[] ReadBytes()
        {
            return EmptyArray<byte>.Empty;
        }

        #endregion
    }
}
