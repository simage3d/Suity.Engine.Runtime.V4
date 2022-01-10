// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using LZ4;
using Suity.Crypto;
using Suity.Helpers.Conversion;
using Suity.Json;

namespace Suity
{
    public abstract class PacketFormatter
    {
        [MultiThreadSecurity(MultiThreadSecurityMethods.ReadonlySecure)]
        public abstract bool Decode(byte[] data, int offset, int length, ref string typeName, ref object obj);
        [MultiThreadSecurity(MultiThreadSecurityMethods.PerThreadSecure)]
        public abstract void Encode(BinaryDataWriter writer, object obj);

        public static PacketFormatter CreatePacketFormatter(PacketFormats packetFormat)
        {
            switch (packetFormat)
            {
                case PacketFormats.Bson:
                    return new BsonPacketFormatter();
                case PacketFormats.Json:
                    return new JsonPacketFormatter();
                case PacketFormats.Binary:
                case PacketFormats.Default:
                default:
                    return new BinaryPacketFormatter();
            }
        }
        public static PacketFormatter CreatePacketFormatter(PacketFormats packetFormat, bool compressed, AesKey aesKey = null)
        {
            PacketFormatter formatter = CreatePacketFormatter(packetFormat);

            if (aesKey != null)
            {
                if (compressed)
                {
                    return new AesPacketFormatter(aesKey, new LZ4PacketFormatter(formatter));
                }
                else
                {
                    return new AesPacketFormatter(aesKey, formatter);
                }
            }
            else
            {
                if (compressed)
                {
                    return new LZ4PacketFormatter(formatter);
                }
                else
                {
                    return formatter;
                }
            }
        }
        public static PacketFormatter CreateCompressedPacketFormatter(PacketFormatter inner)
        {
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            return new LZ4PacketFormatter(inner);
        }
    }

    class AesPacketFormatter : PacketFormatter
    {
        readonly AesKey _key;
        readonly PacketFormatter _inner;

        Rijndael _aes;
        ICryptoTransform _encryptor;
        ICryptoTransform _decryptor;

        public AesPacketFormatter(AesKey key, PacketFormatter formatter)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
            _inner = formatter ?? throw new ArgumentNullException(nameof(formatter));

            _aes = Rijndael.Create();
            _encryptor = _aes.CreateEncryptor(_key.GetKey(), _key.GetIv());
            _decryptor = _aes.CreateDecryptor(_key.GetKey(), _key.GetIv());
        }
        ~AesPacketFormatter()
        {
            _encryptor?.Dispose();
            _decryptor?.Dispose();
            _aes?.Dispose();

            _encryptor = null;
            _decryptor = null;
            _aes = null;
        }

        public override bool Decode(byte[] data, int offset, int length, ref string typeName, ref object obj)
        {
            byte[] edata = _decryptor.TransformFinalBlock(data, offset, length);

            return _inner.Decode(edata, 0, edata.Length, ref typeName, ref obj);
        }

        public override void Encode(BinaryDataWriter writer, object obj)
        {
            int begin = writer.Offset;

            _inner.Encode(writer, obj);

            byte[] pdata = _encryptor.TransformFinalBlock(writer.Buffer, begin, writer.Offset - begin);

            writer.Offset = begin;
            writer.WriteRawBytes(pdata, 0, pdata.Length);
        }

    }

    class LZ4PacketFormatter : PacketFormatter
    {
        PacketFormatter _inner;

        public LZ4PacketFormatter(PacketFormatter formatter)
        {
            _inner = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }


        public override bool Decode(byte[] data, int offset, int length, ref string typeName, ref object obj)
        {
            byte[] pdata = LZ4Codec.Unpickle(data, offset, length);

            //int size = EndianBitConverter.Little.ToInt32(data, offset);
            return _inner.Decode(pdata, 0, pdata.Length, ref typeName, ref obj);
        }

        public override void Encode(BinaryDataWriter writer, object obj)
        {
            int begin = writer.Offset;

            _inner.Encode(writer, obj);

            byte[] cdata = null;
            int cSize = 0;
            LZ4Codec.Pickle(writer.Buffer, begin, writer.Offset - begin, ref cdata, ref cSize);

            writer.Offset = begin;
            writer.WriteRawBytes(cdata, 0, cSize);
        }
    }

    class BinaryPacketFormatter : PacketFormatter
    {
        public override bool Decode(byte[] data, int offset, int length, ref string typeName, ref object obj)
        {
            BinaryDataReader reader = new BinaryDataReader(data, offset);

            if (ObjectType.TryReadObject(reader, out typeName, out obj, out bool isArray))
            {
                return !isArray;
            }
            else
            {
                return false;
            }
        }

        public override void Encode(BinaryDataWriter writer, object obj)
        {
            ObjectType.WriteObject(writer, obj);
        }
    }

    class BsonPacketFormatter : PacketFormatter
    {
        [ThreadStatic]
        static BsonDataWriter _bsonWriter;

        public override bool Decode(byte[] data, int offset, int length, ref string typeName, ref object obj)
        {
            BsonDataReader reader = new BsonDataReader(data, offset, length);

            if (ObjectType.TryReadObject(reader, out typeName, out obj, out bool isArray))
            {
                return !isArray;
            }
            else
            {
                return false;
            }
        }

        public override void Encode(BinaryDataWriter writer, object obj)
        {
            byte[] bytes;

            var bsonWriter = _bsonWriter ?? (_bsonWriter = new BsonDataWriter());

            bsonWriter.Reset();
            ObjectType.WriteObject(bsonWriter, obj);
            bytes = bsonWriter.ToBytes();

            writer.WriteRawBytes(bytes, 0, bytes.Length);
        }
    }

    class JsonPacketFormatter : PacketFormatter
    {
        [ThreadStatic]
        static JsonDataWriter _jsonWriter;

        public override bool Decode(byte[] data, int offset, int length, ref string typeName, ref object obj)
        {
            string str = Encoding.UTF8.GetString(data, offset, length);
            var reader = JsonDataReader.Create(str);

            if (ObjectType.TryReadObject(reader, out typeName, out obj, out bool isArray))
            {
                return !isArray;
            }
            else
            {
                return false;
            }
        }

        public override void Encode(BinaryDataWriter writer, object obj)
        {
            byte[] bytes;

            var jsonWriter = _jsonWriter ?? (_jsonWriter = new JsonDataWriter());

            jsonWriter.Reset();
            ObjectType.WriteObject(jsonWriter, obj);
            string str = jsonWriter.ToString(false);
            bytes = Encoding.UTF8.GetBytes(str);

            writer.WriteRawBytes(bytes, 0, bytes.Length);
        }
    }
}
