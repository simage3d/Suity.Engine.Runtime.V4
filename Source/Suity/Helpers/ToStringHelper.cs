﻿// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Text;

namespace Suity.Helpers
{
    public static class ToStringHelper
    {
        /// <summary>
        /// Creates a separated list of the string versions of a set of objects.
        /// </summary>
        /// <typeparam name="T">The type of the incoming objects.</typeparam>
        /// <param name="collection">A set of objects.</param>
        /// <param name="separator">The string to use as separator between two string values.</param>
        /// <returns></returns>
        public static string ToString<T>(this IEnumerable<T> collection, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in collection)
            {
                sb.Append(item != null ? item.ToString() : "null");
                sb.Append(separator);
            }
            return sb.ToString(0, Math.Max(0, sb.Length - separator.Length));  // Remove at the end is faster
        }
        /// <summary>
        /// Creates a separated list of the string versions of a set of objects.
        /// </summary>
        /// <typeparam name="T">The type of the incoming objects.</typeparam>
        /// <param name="collection">A set of objects.</param>
        /// <param name="toString">A function that transforms objects to strings.</param>
        /// <param name="separator">The string to use as separator between two string values.</param>
        /// <returns></returns>
        public static string ToString<T>(this IEnumerable<T> collection, Func<T, string> toString, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in collection)
            {
                sb.Append(toString(item));
                sb.Append(separator);
            }
            return sb.ToString(0, Math.Max(0, sb.Length - separator.Length));  // Remove at the end is faster
        }
    }
}
