using System;
using Suity.Helpers;
#if !BRIDGE
using ComputerBeacon.Json;
#endif

namespace Suity.Json
{
    public class JsonDataWriter : IDataWriter
    {
        public class NullToken
        {
            public static readonly NullToken Null = new NullToken();
        }

        internal Action<object> _setter;
        object _value;

        public object Value { get { return _value; } }

        public JsonDataWriter()
        {
            _setter = v => { };
        }
        internal JsonDataWriter(Action<object> setter)
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
#if BRIDGE
            if (_value == null)
            {
                _value = new Object();
                _setter(_value);
            }
            Object doc = _value as Object;
            if (doc == null)
            {
                return EmptyDataWriter.Empty;
            }
            JsonDataWriter childWriter = new JsonDataWriter(v => doc[name] = v);
            return childWriter;
#else
            if (_value == null)
            {
                _value = new JsonObject();
                _setter(_value);
            }
            JsonObject doc = _value as JsonObject;
            if (doc == null)
            {
                return EmptyDataWriter.Empty;
            }
            JsonDataWriter childWriter = new JsonDataWriter(v => doc.Add(name, v));
            return childWriter;
#endif
        }

        public IDataArrayWriter Nodes(string name, int count)
        {
#if BRIDGE
            if (_value == null)
            {
                _value = new Object();
                _setter(_value);
            }
            Object doc = _value as Object;
            if (doc == null)
            {
                return EmptyDataArrayWriter.Empty;
            }
            Object[] ary = new Object[0];
            doc[name] = ary;
            return new JsonDataArrayWriter(ary);
#else
            if (_value == null)
            {
                _value = new JsonObject();
                _setter(_value);
            }
            JsonObject doc = _value as JsonObject;
            if (doc == null)
            {
                return EmptyDataArrayWriter.Empty;
            }
            JsonArray ary = new JsonArray();
            doc.Add(name, ary);
            return new JsonDataArrayWriter(ary);
#endif
        }

        public void WriteEmpty(bool empty)
        {
            if (_value != null)
            {
                return;
            }
            if (empty)
            {
                _value = NullToken.Null;
                _setter(null);
            }
        }

        public void WriteTypeName(string typeName)
        {
#if BRIDGE
            if (!(_value is Object))
            {
                _value = new Object();
                _setter(_value);
            }

            _value["@type"] = typeName;
#else
            if (!(_value is JsonObject))
            {
                _value = new JsonObject();
                _setter(_value);
            }

            ((JsonObject)_value).Add("@type", typeName);
#endif
        }

        public void WriteBoolean(bool value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteByte(byte value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteSByte(sbyte value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteInt16(short value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteUInt16(ushort value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteInt32(int value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteUInt32(uint value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteInt64(long value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteUInt64(ulong value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteSingle(float value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteDouble(double value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteString(string value)
        {
            _value = value;
            _setter(_value);
        }

        public void WriteDateTime(DateTime value)
        {
            WriteString(value.ToString());
        }

        public void WriteBytes(byte[] b, int offset, int length)
        {
            string value = Convert.ToBase64String(b, offset, length);

            _value = value;
            _setter(_value);
        }

        #endregion


        public override string ToString()
        {
#if BRIDGE
            return _value != null ? JSON.stringify(_value) : null;
#else
            return _value?.ToString();
#endif
        }

#if BRIDGE
        public string ToString(bool niceFormat)
        {
            return _value != null ? JSON.stringify(_value) : null;
        }
#else
        public string ToString(bool niceFormat)
        {
            if (_value is JsonObject obj)
            {
                return obj.ToString(niceFormat);
            }
            else if (_value is JsonArray ary)
            {
                return ary.ToString(niceFormat);
            }
            else if (_value != null)
            {
                return _value.ToString();
            }
            else
            {
                return null;
            }
        }
#endif
    }

    public class JsonDataArrayWriter : IDataArrayWriter
    {
#if BRIDGE
        Object[] _ary;
        public Object[] Value { get { return _ary; } }

        public JsonDataArrayWriter()
        {
            _ary = new Object[0];
        }
        internal JsonDataArrayWriter(Object[] ary)
        {
            if (ary == null)
            {
                throw new ArgumentNullException();
            }
            _ary = ary;
        }
#else
        readonly JsonArray _ary;
        public JsonArray Value => _ary;

        public JsonDataArrayWriter()
        {
            _ary = new JsonArray();
        }
        public JsonDataArrayWriter(JsonArray ary)
        {
            _ary = ary ?? throw new ArgumentNullException();
        }
#endif

        #region IDataArrayWriter 成员

        public IDataWriter Item()
        {
#if BRIDGE
            JsonDataWriter childWriter = new JsonDataWriter(v => _ary.Push(v));
#else
            JsonDataWriter childWriter = new JsonDataWriter(v => _ary.Add(v));
#endif
            return childWriter;
        }

        public void Finish()
        {
        }

        #endregion

    }
}
