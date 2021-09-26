// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.NodeQuery
{
    public static class NodeReaderExtensions
    {
        public static void WriteTo(this INodeReader reader, INodeWriter writer, int maxDepth = -1)
        {
            if (maxDepth == 0)
            {
                return;
            }
            if (maxDepth > 0)
            {
                maxDepth--;
            }

            writer.SetValue(reader.NodeValue);
            foreach (var attr in reader.Attributes)
            {
                writer.SetAttribute(attr.Key, attr.Value);
            }
            foreach (var childReader in reader.Nodes())
            {
                writer.AddElement(childReader.NodeName, childWriter => childReader.WriteTo(childWriter, maxDepth));
            }
        }

        public static bool GetBooleanValue(this INodeReader reader, bool defaultValue = default(bool))
        {
            if (bool.TryParse(reader.NodeValue, out bool value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static byte GetByteValue(this INodeReader reader, byte defaultValue = default(byte))
        {
            if (byte.TryParse(reader.NodeValue, out byte value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static sbyte GetSByteValue(this INodeReader reader, sbyte defaultValue = default(sbyte))
        {
            if (sbyte.TryParse(reader.NodeValue, out sbyte value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static short GetInt16Value(this INodeReader reader, short defaultValue = default(short))
        {
            if (short.TryParse(reader.NodeValue, out short value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static int GetInt32Value(this INodeReader reader, int defaultValue = default(int))
        {
            if (int.TryParse(reader.NodeValue, out int value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static long GetInt64Value(this INodeReader reader, long defaultValue = default(long))
        {
            if (long.TryParse(reader.NodeValue, out long value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static ushort GetUInt16Value(this INodeReader reader, ushort defaultValue = default(ushort))
        {
            if (ushort.TryParse(reader.NodeValue, out ushort value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static uint GetUInt32Value(this INodeReader reader, uint defaultValue = default(uint))
        {
            if (uint.TryParse(reader.NodeValue, out uint value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static ulong GetUInt64Value(this INodeReader reader, ulong defaultValue = default(ulong))
        {
            if (ulong.TryParse(reader.NodeValue, out ulong value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static float GetSingleValue(this INodeReader reader, float defaultValue = default(float))
        {
            if (float.TryParse(reader.NodeValue, out float value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static double GetDoubleValue(this INodeReader reader, double defaultValue = default(double))
        {
            if (double.TryParse(reader.NodeValue, out double value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static decimal GetDecimalValue(this INodeReader reader, decimal defaultValue = default(decimal))
        {
            if (decimal.TryParse(reader.NodeValue, out decimal value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }



        public static bool GetBooleanAttribute(this INodeReader reader, string name, bool defaultValue = default(bool))
        {
            if (bool.TryParse(reader.GetAttribute(name), out bool value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static byte GetByteAttribute(this INodeReader reader, string name, byte defaultValue = default(byte))
        {
            if (byte.TryParse(reader.GetAttribute(name), out byte value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static sbyte GetSByteAttribute(this INodeReader reader, string name, sbyte defaultValue = default(sbyte))
        {
            if (sbyte.TryParse(reader.GetAttribute(name), out sbyte value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static short GetInt16Attribute(this INodeReader reader, string name, short defaultValue = default(short))
        {
            if (short.TryParse(reader.GetAttribute(name), out short value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static int GetInt32Attribute(this INodeReader reader, string name, int defaultValue = default(int))
        {
            if (int.TryParse(reader.GetAttribute(name), out int value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static long GetInt64Attribute(this INodeReader reader, string name, long defaultValue = default(long))
        {
            if (long.TryParse(reader.GetAttribute(name), out long value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static ushort GetUInt16Attribute(this INodeReader reader, string name, ushort defaultValue = default(ushort))
        {
            if (ushort.TryParse(reader.GetAttribute(name), out ushort value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static uint GetUInt32Attribute(this INodeReader reader, string name, uint defaultValue = default(uint))
        {
            if (uint.TryParse(reader.GetAttribute(name), out uint value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static ulong GetUInt64Attribute(this INodeReader reader, string name, ulong defaultValue = default(ulong))
        {
            if (ulong.TryParse(reader.GetAttribute(name), out ulong value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static float GetSingleAttribute(this INodeReader reader, string name, float defaultValue = default(float))
        {
            if (float.TryParse(reader.GetAttribute(name), out float value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static double GetDoubleAttribute(this INodeReader reader, string name, double defaultValue = default(double))
        {
            if (double.TryParse(reader.GetAttribute(name), out double value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public static decimal GetDecimalAttribute(this INodeReader reader, string name, decimal defaultValue = default(decimal))
        {
            if (decimal.TryParse(reader.GetAttribute(name), out decimal value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

    }
}