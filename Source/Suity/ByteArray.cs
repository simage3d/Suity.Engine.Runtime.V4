// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    /// <summary>
    /// Byte数组
    /// </summary>
    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public sealed class ByteArray : BinaryDataWriter
    {
        readonly BinaryDataReader _reader;

        public ByteArray()
            : base()
        {
            _reader = new BinaryDataReader(Buffer);
        }
        public ByteArray(int initialCapacity)
            : base(initialCapacity)
        {
            _reader = new BinaryDataReader(Buffer);
        }
        public ByteArray(byte[] buffer)
            : base(buffer)
        {
            _reader = new BinaryDataReader(Buffer);
        }

        /// <summary>
        /// 读取器
        /// </summary>
        public BinaryDataReader Reader => _reader;

        /// <summary>
        /// 写入器偏移与读取器偏移之间的程度
        /// </summary>
        public int RestFromReader => Offset - _reader.Offset;

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            _reader.Reset();
        }
        protected override void OnResized()
        {
            _reader._buffer = Buffer;
        }
    }
}
