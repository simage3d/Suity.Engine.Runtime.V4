using System;
using System.Collections;
using System.Collections.Generic;
using Suity.Helpers;
#if !BRIDGE
using ComputerBeacon.Json;
#endif

namespace Suity.Json
{
    public class JsonDataReader : IDataReader
    {
        readonly object _obj;

        public JsonDataReader(object obj)
        {
            _obj = obj;
        }
        public JsonDataReader(string json)
        {
            _obj = Parser.Parse(json);
        }

        #region IDataReader 成员

        public IDataReader Node(string name)
        {
#if BRIDGE
            if (_obj != null)
            {
                return new JsonDataReader(_obj[name]);
            }
            else
            {
                return EmptyDataReader.Empty;
            }
#else
            if (_obj is JsonObject jobj)
            {
                return new JsonDataReader(jobj[name]);
            }
            else
            {
                return EmptyDataReader.Empty;
            }
#endif
        }

        public IEnumerable<IDataReader> Nodes(string name)
        {
#if BRIDGE
            if (_obj != null)
            {
                IEnumerable ary = _obj[name] as IEnumerable;
                if (ary != null)
                {
                    foreach (var item in ary)
                    {
                        yield return new JsonDataReader(item);
                    }
                }
            }
#else
            if (_obj is JsonObject jobj)
            {
                JsonArray ary = jobj[name] as JsonArray;
                if (ary != null)
                {
                    foreach (var item in ary)
                    {
                        yield return new JsonDataReader(item);
                    }
                }
            }
#endif
        }

        public bool ReadIsEmpty()
        {
            return _obj == null;
        }

        public string ReadTypeName()
        {
#if BRIDGE
            if (_obj != null)
            {
                return _obj["@type"] as string;
            }
            else
            {
                return null;
            }
#else
            if (_obj is JsonObject jobj)
            {
                return jobj["@type"] as string;
            }
            else
            {
                return null;
            }
#endif
        }

        public bool ReadBoolean()
        {
            return (_obj != null) ? Convert.ToBoolean(_obj) : default(bool);
        }

        public byte ReadByte()
        {
            return (_obj != null) ? Convert.ToByte(_obj) : default(byte);
        }

        public double ReadDouble()
        {
            return (_obj != null) ? Convert.ToDouble(_obj) : default(double);
        }

        public short ReadInt16()
        {
            return (_obj != null) ? Convert.ToInt16(_obj) : default(short);
        }

        public int ReadInt32()
        {
            return (_obj != null) ? Convert.ToInt32(_obj) : default(int);
        }

        public long ReadInt64()
        {
            return (_obj != null) ? Convert.ToInt64(_obj) : default(long);
        }

        public sbyte ReadSByte()
        {
            return (_obj != null) ? Convert.ToSByte(_obj) : default(sbyte);
        }

        public float ReadSingle()
        {
            return (_obj != null) ? Convert.ToSingle(_obj) : default(float);
        }

        public string ReadString()
        {
            return (_obj != null) ? Convert.ToString(_obj) : null;
        }

        public ushort ReadUInt16()
        {
            return (_obj != null) ? Convert.ToUInt16(_obj) : default(ushort);
        }

        public uint ReadUInt32()
        {
            return (_obj != null) ? Convert.ToUInt32(_obj) : default(uint);
        }

        public ulong ReadUInt64()
        {
            return (_obj != null) ? Convert.ToUInt64(_obj) : default(ulong);
        }

        public DateTime ReadDateTime()
        {
            string str = ReadString();
            if (DateTime.TryParse(str, out DateTime result))
            {
                return result;
            }
            else
            {
                return default(DateTime);
            }
        }

        public byte[] ReadBytes()
        {
            string str = ReadString();
            return Convert.FromBase64String(str);
        }

        #endregion


        public static IDataReader Create(string json)
        {
#if BRIDGE
            var obj = JSON.parse(json);
            if (obj != null)
            {
                return new JsonDataReader(obj);
            }
            else
            {
                return EmptyDataReader.Empty;
            }
#else
            JsonObject obj = Parser.Parse(json) as JsonObject;
            if (obj != null)
            {
                return new JsonDataReader(obj);
            }
            else
            {
                return EmptyDataReader.Empty;
            }
#endif
        }
    }
}
