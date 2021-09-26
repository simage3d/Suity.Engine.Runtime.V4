using System;
using Kernys.Bson;
using Suity.Collections;

namespace Suity.Json
{
    public class BsonDataWriter : IDataWriter
    {
        public class NullToken
        {
            public static readonly NullToken Null = new NullToken();
        }

        internal Action<BSONValue> _setter;
        BSONValue _value;

        public object Value { get { return _value; } }

        public BsonDataWriter()
        {
            _setter = v => { };
        }
        public BsonDataWriter(Action<BSONValue> setter)
        {
            _setter = setter ?? throw new ArgumentNullException();
        }

        public void Reset()
        {
            _value = null;
        }

        #region IDataWriter 成员

        public IDataWriter Node(string name)
        {
            if (_value == null)
            {
                _value = new BSONObject();
                _setter(_value);
            }
            BSONObject doc = _value as BSONObject;
            if (doc == null)
            {
                return EmptyDataWriter.Empty;
            }
            BsonDataWriter childWriter = new BsonDataWriter(v => doc.Add(name, v));
            return childWriter;
        }

        public IDataArrayWriter Nodes(string name, int count)
        {
            if (_value == null)
            {
                _value = new BSONObject();
                _setter(_value);
            }
            BSONObject doc = _value as BSONObject;
            if (doc == null)
            {
                return EmptyDataArrayWriter.Empty;
            }
            BSONArray ary = new BSONArray();
            doc.Add(name, ary);
            return new BsonDataArrayWriter(ary);
        }

        public void WriteEmpty(bool empty)
        {
            if (_value != null)
            {
                return;
            }
            if (empty)
            {
                _value = new BSONValue();
                _setter(_value);
            }
        }

        public void WriteTypeName(string typeName)
        {
            if (_value != null)
            {
                return;
            }

            var obj = new BSONObject();

            _value = obj;
            _setter(_value);
            obj.Add("@type", typeName);
        }

        public void WriteBoolean(bool value)
        {
            _value = new BSONValue(value);
            _setter(_value);
        }

        public void WriteByte(byte value)
        {
            _value = new BSONValue((int)value);
            _setter(_value);
        }

        public void WriteSByte(sbyte value)
        {
            _value = new BSONValue((int)value);
            _setter(_value);
        }

        public void WriteInt16(short value)
        {
            _value = new BSONValue((int)value);
            _setter(_value);
        }

        public void WriteUInt16(ushort value)
        {
            _value = new BSONValue((int)value);
            _setter(_value);
        }

        public void WriteInt32(int value)
        {
            _value = new BSONValue(value);
            _setter(_value);
        }

        public void WriteUInt32(uint value)
        {
            _value = new BSONValue((Int64)value);
            _setter(_value);
        }

        public void WriteInt64(long value)
        {
            _value = new BSONValue(value);
            _setter(_value);
        }

        public void WriteUInt64(ulong value)
        {
            _value = new BSONValue(value.ToString());
            _setter(_value);
        }

        public void WriteSingle(float value)
        {
            _value = new BSONValue((double)value);
            _setter(_value);
        }

        public void WriteDouble(double value)
        {
            _value = new BSONValue(value);
            _setter(_value);
        }

        public void WriteString(string value)
        {
            _value = new BSONValue(value);
            _setter(_value);
        }

        public void WriteDateTime(DateTime value)
        {
            _value = new BSONValue(value);
            _setter(_value);
        }

        public void WriteBytes(byte[] b, int offset, int length)
        {
            byte[] data = new byte[length];
            Array.Copy(b, offset, data, 0, length);
            _value = new BSONValue(data);
            _setter(_value);
        }

        #endregion

        public override string ToString()
        {
            return _value?.ToString();
        }

        public byte[] ToBytes()
        {
            var obj = _value as BSONObject;
            if (obj != null)
            {
                return SimpleBSON.Dump(obj);
            }
            else
            {
                return EmptyArray<byte>.Empty;
            }
        }

    }

    public class BsonDataArrayWriter : IDataArrayWriter
    {
        readonly BSONArray _ary;
        public BSONArray Value { get { return _ary; } }

        public BsonDataArrayWriter()
        {
            _ary = new BSONArray();
        }
        internal BsonDataArrayWriter(BSONArray ary)
        {
            _ary = ary ?? throw new ArgumentNullException();
        }

        #region IDataArrayWriter 成员

        public IDataWriter Item()
        {
            BsonDataWriter childWriter = new BsonDataWriter(v => _ary.Add(v));
            return childWriter;
        }

        public void Finish()
        {
        }

        #endregion
    }
}
