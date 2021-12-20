using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public class PrimaryTypeInfo : ClassTypeInfo
    {
        private PrimaryTypeInfo(
            Type type,
            string name,
            string aliasName, 
            ObjectType.ReadDelegate reader, 
            ObjectType.WriteDelegate writer, 
            ObjectType.CloneDelegate cloner, 
            ObjectType.EqualsDelegate equalChecker,
            ObjectType.ExchangeDelegate exchanger)
            : base(type, name, aliasName, PacketFormats.Default, reader, writer, cloner, equalChecker, exchanger)
        {
        }


        public override bool IsPrimitive => true;

        internal static PrimaryTypeInfo Create(TypeCode typeCode)
        {
            object cloner(object source, object target, bool autoNew) => source;
            bool equal(object a, object b) => Equals(a, b);
            void exchange(object obj, IExchange ex) { }

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return new PrimaryTypeInfo(typeof(Boolean), "Boolean", "bool", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.SByte:
                    return new PrimaryTypeInfo(typeof(SByte), "SByte", "sbyte", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.Byte:
                    return new PrimaryTypeInfo(typeof(Byte), "Byte", "byte", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.Int16:
                    return new PrimaryTypeInfo(typeof(Int16), "Int16", "short", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.UInt16:
                    return new PrimaryTypeInfo(typeof(UInt16), "UInt16", "ushort", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.Int32:
                    return new PrimaryTypeInfo(typeof(Int32), "Int32", "int", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.UInt32:
                    return new PrimaryTypeInfo(typeof(UInt32), "UInt32", "uint", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.Int64:
                    return new PrimaryTypeInfo(typeof(Int64), "Int64", "long", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.UInt64:
                    return new PrimaryTypeInfo(typeof(UInt64), "UInt64", "ulong", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.Single:
                    return new PrimaryTypeInfo(typeof(Single), "Single", "float", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.Double:
                    return new PrimaryTypeInfo(typeof(Double), "Double", "double", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.DateTime:
                    return new PrimaryTypeInfo(typeof(DateTime), "DateTime", null, GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                case TypeCode.String:
                    return new PrimaryTypeInfo(typeof(String), "String", "string", GetReader(typeCode), GetWriter(typeCode), cloner, equal, exchange);
                default:
                    return null;
            }
        }

        private static ObjectType.ReadDelegate GetReader(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return r => r.ReadBoolean();
                case TypeCode.SByte:
                    return r => r.ReadSByte();
                case TypeCode.Byte:
                    return r => r.ReadByte();
                case TypeCode.Int16:
                    return r => r.ReadInt16();
                case TypeCode.UInt16:
                    return r => r.ReadUInt16();
                case TypeCode.Int32:
                    return r => r.ReadInt32();
                case TypeCode.UInt32:
                    return r => r.ReadUInt32();
                case TypeCode.Int64:
                    return r => r.ReadInt64();
                case TypeCode.UInt64:
                    return r => r.ReadUInt64();
                case TypeCode.Single:
                    return r => r.ReadSingle();
                case TypeCode.Double:
                    return r => r.ReadDouble();
                case TypeCode.DateTime:
                    return r => r.ReadDateTime();
                case TypeCode.String:
                    return r => r.ReadString();
                default:
                    return null;
            }
        }

        private static ObjectType.WriteDelegate GetWriter(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return (w, o) => w.WriteBoolean((Boolean)o);
                case TypeCode.SByte:
                    return (w, o) => w.WriteSByte((SByte)o);
                case TypeCode.Byte:
                    return (w, o) => w.WriteByte((Byte)o);
                case TypeCode.Int16:
                    return (w, o) => w.WriteInt16((Int16)o);
                case TypeCode.UInt16:
                    return (w, o) => w.WriteUInt16((UInt16)o);
                case TypeCode.Int32:
                    return (w, o) => w.WriteInt32((Int32)o);
                case TypeCode.UInt32:
                    return (w, o) => w.WriteUInt32((UInt32)o);
                case TypeCode.Int64:
                    return (w, o) => w.WriteInt64((Int64)o);
                case TypeCode.UInt64:
                    return (w, o) => w.WriteUInt64((UInt64)o);
                case TypeCode.Single:
                    return (w, o) => w.WriteSingle((Single)o);
                case TypeCode.Double:
                    return (w, o) => w.WriteDouble((Double)o);
                case TypeCode.DateTime:
                    return (w, o) => w.WriteDateTime((DateTime)o);
                case TypeCode.String:
                    return (w, o) => w.WriteString((String)o);
                default:
                    return null;
            }
        }
    }
}
