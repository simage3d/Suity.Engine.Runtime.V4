// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity
{
    /// <summary>
    /// Class类型信息
    /// </summary>
    [AssetDefinitionType(AssetDefinitionCodes.Struct)]
    public class ClassTypeInfo : TypeInfo
    {
        public readonly ObjectType.ReadDelegate OriginReader;
        public readonly ObjectType.WriteDelegate OriginWriter;

        public readonly ObjectType.ReadDelegate Reader;
        public readonly ObjectType.WriteDelegate Writer;
        public readonly ObjectType.CloneDelegate Cloner;
        public readonly ObjectType.EqualsDelegate EqualChecker;
        public readonly ObjectType.ExchangeDelegate Exchanger;
        public readonly ObjectType.GetPropertyDelegate PropertyGetter;
        public readonly ObjectType.SetPropertyDelegate PropertySetter;

        readonly PacketFormats _packetFormat;

        internal ClassTypeInfo(Type type, string key, string aliasName, PacketFormats packetFormat = PacketFormats.Default)
            : base(type, key, aliasName)
        {
        }

        internal ClassTypeInfo(
            Type type, string key, string aliasName, PacketFormats packetFormat,
            ObjectType.ReadDelegate reader,
            ObjectType.WriteDelegate writer,
            ObjectType.CloneDelegate cloner,
            ObjectType.EqualsDelegate equalChecker,
            ObjectType.ExchangeDelegate exchanger,
            ObjectType.GetPropertyDelegate propGetter = null,
            ObjectType.SetPropertyDelegate propSetter = null
            )
            : base(type, key, aliasName)
        {
            _packetFormat = packetFormat;

            OriginReader = Reader = reader ?? (r => throw new NotImplementedException());
            OriginWriter = Writer = writer ?? ((r, o) => throw new NotImplementedException());
            Cloner = cloner ?? ((r, o, a) => throw new NotImplementedException());
            EqualChecker = equalChecker ?? ((a, b) => throw new NotImplementedException());
            Exchanger = exchanger ?? ((o, e) => throw new NotImplementedException());
            PropertyGetter = propGetter;
            PropertySetter = propSetter;

            if (_packetFormat != PacketFormats.Default)
            {
                Reader = ConvertedReader;
                Writer = ConvertedWriter;
            }
        }

        public override bool IsClass => true;
        public override PacketFormats PacketFormat => _packetFormat;


        public object Read(IDataReader reader)
        {
            if (reader == null || reader.ReadIsEmpty())
            {
                return null;
            }
            return Reader(reader);
        }
        public void Write(IDataWriter writer, object obj)
        {
            if (obj == null)
            {
                writer.WriteEmpty(true);
                return;
            }
            writer.WriteEmpty(false);
            Writer(writer, obj);
        }
        public object Clone(object source, object target, bool autoNew)
        {
            return Cloner(source, target, autoNew);
        }
        public bool ObjectEquals(object obj1, object obj2)
        {
            return EqualChecker(obj1, obj2);
        }

        public object ConvertedReader(IDataReader reader)
        {
            IDataReader readerConverted = ObjectType.ConvertReader(reader, _packetFormat);
            return OriginReader(readerConverted);
        }
        public void ConvertedWriter(IDataWriter writer, object obj)
        {
            ObjectType.ConvertWriter(writer, _packetFormat, writerConverted => 
            {
                OriginWriter(writerConverted, obj);
            });
        }
    }
}
