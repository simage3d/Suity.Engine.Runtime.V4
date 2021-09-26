// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Linq;

namespace Suity
{
    public class FieldCode
    {
        public readonly string MainName;
        public readonly string FieldName;

        public FieldCode(string code)
        {
            if (code == null)
            {
                code = string.Empty;
            }

            int index = code.LastIndexOf('.');

            if (index >= 0)
            {
                MainName = code.Substring(0, index) ?? string.Empty;
                FieldName = code.Substring(index + 1, code.Length - index - 1) ?? string.Empty;
            }
            else
            {
                MainName = code;
                FieldName = string.Empty;
            }
        }

        public FieldCode(string mainName, string fieldName)
        {
            MainName = mainName ?? string.Empty;
            FieldName = fieldName ?? string.Empty;
        }

        public override string ToString()
        {
            return Combine(MainName, FieldName);
        }

        #region 判断部分
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(FieldCode)) return false;

            FieldCode other = (FieldCode)obj;

            return MainName == other.MainName &&
                FieldName == other.FieldName;
        }
        public static bool operator ==(FieldCode v1, FieldCode v2)
        {
            if (Equals(v1, null)) return Equals(v2, null); else return v1.Equals(v2);
        }
        public static bool operator !=(FieldCode v1, FieldCode v2)
        {
            if (Equals(v1, null)) return !Equals(v2, null); else return !v1.Equals(v2);
        }
        #endregion

        public static string Combine(string mainName, string fieldName)
        {
            if (mainName == null) mainName = string.Empty;

            if (string.IsNullOrEmpty(fieldName))
            {
                return mainName;
            }
            else
            {
                return mainName + "." + fieldName;
            }
        }
        public static bool HasField(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            return str.Contains('.');
        }
    }
}
