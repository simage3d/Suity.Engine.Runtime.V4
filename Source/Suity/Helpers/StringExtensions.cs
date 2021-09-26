// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Helpers
{
    public static class StringExtensions
    {
        public static string Fmt(this string format, object arg0)
        {
            return string.Format(format, arg0);
        }
        public static string Fmt(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
        public static string RemoveFromFirst(this string str, int count)
        {
            return str.Substring(count, str.Length - count);
        }
        public static string RemoveFromLast(this string str, int count)
        {
            return str.Substring(0, str.Length - count);
        }
        public static string RemoveFromFirst(this string str, string remove)
        {
            if (str.StartsWith(remove))
            {
                return str.Substring(remove.Length, str.Length - remove.Length);
            }
            else
            {
                return str;
            }
        }
        public static string RemoveFromLast(this string str, string remove)
        {
            if (str.EndsWith(remove))
            {
                return str.Substring(0, str.Length - remove.Length);
            }
            else
            {
                return str;
            }
        }
        public static string Limit(this string str, int count)
        {
            if (str == null)
            {
                return null;
            }
            if (str.Length > count)
            {
                return str.Substring(0, count);
            }
            return str;
        }

        public static string ExtractFirstSplit(this string str, char splitter)
        {
            int index = str.IndexOf(splitter);
            if (index >= 0)
            {
                return str.Substring(0, index);
            }
            else
            {
                return null;
            }
        }
        public static string ExtractLastSplit(this string str, char splitter)
        {
            int index = str.LastIndexOf(splitter);
            if (index >= 0)
            {
                return str.Substring(index + 1, str.Length - index - 1);
            }
            else
            {
                return null;
            }
        }
    }
}
