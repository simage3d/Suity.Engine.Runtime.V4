using System;
using System.Collections.Generic;
using Kernys.Bson;
using Suity.Collections;

namespace Suity.Json
{
    public class BsonDataReader : IDataReader
    {
        readonly object _obj;

        public BsonDataReader(object obj)
        {
            _obj = obj;
        }
        private BsonDataReader(BSONValue value)
        {
            _obj = value;
        }
        private BsonDataReader(BSONObject value)
        {
            _obj = value;
        }
        public BsonDataReader(byte[] buf)
        {
            _obj = SimpleBSON.Load(buf);
        }
        public BsonDataReader(byte[] buf, int offset, int length)
        {
            _obj = SimpleBSON.Load(buf, offset, length);
        }


        #region IDataReader 成员

        public IDataReader Node(string name)
        {
            BSONObject jobj = _obj as BSONObject;
            if (jobj != null)
            {
                return new BsonDataReader(jobj[name]);
            }
            else
            {
                return EmptyDataReader.Empty;
            }
        }

        public IEnumerable<IDataReader> Nodes(string name)
        {
            BSONObject jobj = _obj as BSONObject;
            if (jobj != null)
            {
                BSONArray ary = jobj[name] as BSONArray;
                if (ary != null)
                {
                    foreach (var item in ary)
                    {
                        yield return new BsonDataReader(item);
                    }
                }
            }
        }

        public bool ReadIsEmpty()
        {
            BSONValue v = _obj as BSONValue;
            return v?.valueType == BSONValue.ValueType.None;
        }

        public string ReadTypeName()
        {
            BSONObject jobj = _obj as BSONObject;
            if (jobj != null)
            {
                return jobj["@type"]?.stringValueSafe;
            }
            else
            {
                return null;
            }
        }

        public bool ReadBoolean()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return b.boolValueSafe;
                case bool value:
                    return value;
                default:
                    return default(bool);
            }
        }

        public byte ReadByte()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return (byte)b.int32ValueSafe;
#if BRIDGE
                case Object p when p.GetType().IsPrimitive():
#else
                case Object p when p.GetType().IsPrimitive:
#endif
                    return Convert.ToByte(_obj);
                default:
                    return default(byte);
            }
        }

        public sbyte ReadSByte()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return (sbyte)b.int32ValueSafe;
#if BRIDGE
                case Object p when p.GetType().IsPrimitive():
#else
                case Object p when p.GetType().IsPrimitive:
#endif
                    return Convert.ToSByte(_obj);
                default:
                    return default(sbyte);
            }
        }

        public short ReadInt16()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return (short)b.int32ValueSafe;
#if BRIDGE
                case Object p when p.GetType().IsPrimitive():
#else
                case Object p when p.GetType().IsPrimitive:
#endif

                    return Convert.ToInt16(_obj);
                default:
                    return default(short);
            }
        }

        public ushort ReadUInt16()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return (ushort)b.int32ValueSafe;
#if BRIDGE
                case Object p when p.GetType().IsPrimitive():
#else
                case Object p when p.GetType().IsPrimitive:
#endif
                    return Convert.ToUInt16(_obj);
                default:
                    return default(ushort);
            }
        }

        public int ReadInt32()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return b.int32ValueSafe;
#if BRIDGE
                case Object p when p.GetType().IsPrimitive():
#else
                case Object p when p.GetType().IsPrimitive:
#endif
                    return Convert.ToInt32(_obj);
                default:
                    return default(int);
            }
        }

        public uint ReadUInt32()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return (uint)b.int64ValueSafe;
#if BRIDGE
                case Object p when p.GetType().IsPrimitive():
#else
                case Object p when p.GetType().IsPrimitive:
#endif
                    return Convert.ToUInt32(_obj);
                default:
                    return default(int);
            }
        }

        public long ReadInt64()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return b.int64ValueSafe;
#if BRIDGE
                case Object p when p.GetType().IsPrimitive():
#else
                case Object p when p.GetType().IsPrimitive:
#endif
                    return Convert.ToInt64(_obj);
                default:
                    return default(int);
            }
        }

        public ulong ReadUInt64()
        {
            switch (_obj)
            {
                case BSONValue b:
                    ulong result;
                    UInt64.TryParse(b.stringValueSafe, out result);
                    return result;
#if BRIDGE
                case Object p when p.GetType().IsPrimitive():
#else
                case Object p when p.GetType().IsPrimitive:
#endif
                    return Convert.ToUInt64(_obj);
                default:
                    return default(int);
            }
        }

        public float ReadSingle()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return (float)b.doubleValueSafe;
#if BRIDGE
                case Object p when p.GetType().IsPrimitive():
#else
                case Object p when p.GetType().IsPrimitive:
#endif
                    return Convert.ToSingle(_obj);
                default:
                    return default(int);
            }
        }

        public double ReadDouble()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return b.doubleValueSafe;
#if BRIDGE
                case Object p when p.GetType().IsPrimitive():
#else
                case Object p when p.GetType().IsPrimitive:
#endif
                    return Convert.ToDouble(_obj);
                default:
                    return default(int);
            }
        }

        public string ReadString()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return b.stringValueSafe;
#if BRIDGE
                case Object p when p.GetType().IsPrimitive():
#else
                case Object p when p.GetType().IsPrimitive:
#endif
                    return p.ToString();
                default:
                    return null;
            }
        }

        public DateTime ReadDateTime()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return b.dateTimeValueSafe;
                case DateTime p:
                    return p;
                default:
                    return default(DateTime);
            }
        }

        public byte[] ReadBytes()
        {
            switch (_obj)
            {
                case BSONValue b:
                    return b.binaryValueSafe;
                case Byte[] p:
                    return p;
                default:
                    return EmptyArray<byte>.Empty;
            }
        }

#endregion


        public static BsonDataReader LoadBsonDocument(byte[] buf, int offset, int length)
        {
            BSONObject obj = SimpleBSON.Load(buf, offset, length);
            if (obj == null)
            {
                throw new InvalidOperationException();
            }
            return new BsonDataReader(obj);
        }
    }
}
