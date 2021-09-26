// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Linq;

namespace Suity
{
    /// <summary>
    /// 两段键码
    /// </summary>
    public class KeyCode
    {
        static readonly char[] Splitter = new char[] { '|' };

        public readonly string MainKey;
        public readonly string ElementKey;

        public KeyCode(string fullCode)
        {
            if (fullCode == null)
            {
                fullCode = string.Empty;
            }

            string[] split = fullCode.Split(Splitter, 2);
            if (split.Length == 2)
            {
                MainKey = split[0] ?? string.Empty;
                ElementKey = split[1] ?? string.Empty;
            }
            else if (split.Length == 1)
            {
                MainKey = split[0] ?? string.Empty;
                ElementKey = string.Empty;
            }
            else
            {
                MainKey = string.Empty;
                ElementKey = string.Empty;
            }
        }

        public KeyCode(string mainKey, string elementKey)
        {
            if (mainKey == null)
            {
                mainKey = string.Empty;
            }
            if (mainKey.Contains('|'))
            {
                throw new ArgumentException("mainKey");
            }

            MainKey = mainKey ?? string.Empty;
            ElementKey = elementKey ?? string.Empty;
        }

        public bool IsEmpty { get { return string.IsNullOrEmpty(MainKey) && string.IsNullOrEmpty(ElementKey); } }


        public override string ToString()
        {
            return Combine(MainKey, ElementKey);
        }

        #region 判断部分
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(KeyCode)) return false;

            KeyCode other = (KeyCode)obj;

            return MainKey == other.MainKey &&
                ElementKey == other.ElementKey;
        }
        public static bool operator ==(KeyCode v1, KeyCode v2)
        {
            if (Equals(v1, null)) return Equals(v2, null); else return v1.Equals(v2);
        }
        public static bool operator !=(KeyCode v1, KeyCode v2)
        {
            if (Equals(v1, null)) return !Equals(v2, null); else return !v1.Equals(v2);
        }
        #endregion



        public static string Combine(string mainKey, string elementKey)
        {
            if (mainKey == null)
            {
                mainKey = string.Empty;
            }
            if (string.IsNullOrEmpty(elementKey))
            {
                return mainKey;
            }
            else
            {
                return mainKey + "|" + elementKey;
            }
        }
        public static bool HasElement(string keyString)
        {
            if (string.IsNullOrEmpty(keyString))
            {
                return false;
            }
            return keyString.Contains('|');
        }
    }
}
