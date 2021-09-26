// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Linq;
using System.Text;

namespace Suity.Helpers
{
    public static class ByteHelpers
    {
        public static int ToUTF8Bytes(this string s, byte[] bytes, int index)
        {
            return Encoding.UTF8.GetBytes(s, 0, s.Length, bytes, index);
        }
        public static int GetUTF8Length(this string s)
        {
            return Encoding.UTF8.GetByteCount(s);
        }
        public static string GetUTF8String(byte[] bytes, int index, int count)
        {
            return Encoding.UTF8.GetString(bytes, index, count);
        }

        public static byte[] FromString(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        public static byte[] FromBase64(string base64Text)
        {
            return Convert.FromBase64String(base64Text);
        }

        public static string ToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string ToBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static string ToHex(byte[] bytes)
        {
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.AppendFormat("{0:X2}", b);
            }
            return builder.ToString();
        }

        public static byte[] Combine(params byte[][] arrays)
        {
            var result = new byte[arrays.Sum(a => a.Length)];

            var offset = 0;

            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }

            return result;
        }
    }
}
