// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;

namespace Suity.Collections
{
    public static class ArrayHelper
    {
        public static T GetListItemSafe<T>(this List<T> list, int index)
        {
            if (list == null || list.Count == 0)
            {
                return default(T);
            }
            if (index < 0 || index >= list.Count)
            {
                return default(T);
            }
            else
            {
                return list[index];
            }
        }
        public static T GetListItemMinMax<T>(this List<T> list, int index)
        {
            if (list == null || list.Count == 0)
            {
                return default(T);
            }
            if (index < 0)
            {
                return list[0];
            }
            else if (index >= list.Count)
            {
                return list[list.Count - 1];
            }
            else
            {
                return list[index];
            }
        }
        public static void EnsureListSize<T>(this List<T> list, int size)
        {
            while (list.Count < size)
            {
                list.Add(default(T));
            }
        }
        public static int IndexOf<T>(this List<T> list, Predicate<T> condition)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (condition(list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static T GetArrayItemMinMax<T>(this T[] ary, int index)
        {
            if (ary == null || ary.Length == 0)
            {
                return default(T);
            }
            if (index < 0)
            {
                return ary[0];
            }
            else if (index >= ary.Length)
            {
                return ary[ary.Length - 1];
            }
            else
            {
                return ary[index];
            }
        }
        public static T GetArrayItemSafe<T>(this T[] ary, int index)
        {
            if (ary == null || ary.Length == 0)
            {
                return default(T);
            }
            if (index < 0 || index >= ary.Length)
            {
                return default(T);
            }
            else
            {
                return ary[index];
            }
        }
        public static int IndexOf<T>(this T[] ary, Predicate<T> condition)
        {
            for (int i = 0; i < ary.Length; i++)
            {
                if (condition(ary[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static T GetIListItemSafe<T>(this IList<T> list, int index)
        {
            if (list == null || list.Count == 0)
            {
                return default(T);
            }
            if (index < 0 || index >= list.Count)
            {
                return default(T);
            }
            else
            {
                return list[index];
            }
        }
        public static T GetIListItemMinMax<T>(this IList<T> list, int index)
        {
            if (list == null || list.Count == 0)
            {
                return default(T);
            }
            if (index < 0)
            {
                return list[0];
            }
            else if (index >= list.Count)
            {
                return list[list.Count - 1];
            }
            else
            {
                return list[index];
            }
        }
        public static void EnsureIListSize<T>(this IList<T> list, int size)
        {
            while (list.Count < size)
            {
                list.Add(default(T));
            }
        }
        public static void EnsureListSize(this IList list, int size)
        {
            while (list.Count < size)
            {
                list.Add(null);
            }
        }

        public static object GetListItemSafe(this IList list, int index)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }
            if (index < 0 || index >= list.Count)
            {
                return null;
            }
            else
            {
                return list[index];
            }
        }
        public static object GetListItemMinMax(this IList list, int index)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }
            if (index < 0)
            {
                return list[0];
            }
            else if (index >= list.Count)
            {
                return list[list.Count - 1];
            }
            else
            {
                return list[index];
            }
        }



        public static object GetArrayItemMinMax(this Array ary, int index)
        {
            if (ary == null || ary.Length == 0)
            {
                return null;
            }
            if (index < 0)
            {
                return ary.GetValue(0);
            }
            else if (index >= ary.Length)
            {
                return ary.GetValue(ary.Length - 1);
            }
            else
            {
                return ary.GetValue(index);
            }
        }
        public static object GetArrayItemSafe(this Array ary, int index)
        {
            if (ary == null || ary.Length == 0)
            {
                return null;
            }
            if (index < 0 || index >= ary.Length)
            {
                return null;
            }
            else
            {
                return ary.GetValue(index);
            }
        }

        public static T GetRandomListItem<T>(this Random rnd, List<T> list)
        {
            if (list.Count == 0)
            {
                return default(T);
            }

            return list[rnd.Next(list.Count)];
        }
        public static T GetRandomListItem<T>(this List<T> list, Random rnd)
        {
            if (list.Count == 0)
            {
                return default(T);
            }

            return list[rnd.Next(list.Count)];
        }

        public static T GetRandomArrayItem<T>(this Random rnd, T[] list)
        {
            if (list.Length == 0)
            {
                return default(T);
            }

            return list[rnd.Next(list.Length)];
        }
        public static T GetRandomArrayItem<T>(this T[] list, Random rnd)
        {
            if (list.Length == 0)
            {
                return default(T);
            }

            return list[rnd.Next(list.Length)];
        }

        public static bool ArrayEquals<T>(T[] aryA, T[] aryB)
        {
            if (aryA == null && aryB == null)
            {
                return true;
            }
            if (aryA == null || aryB == null)
            {
                return false;
            }
            if (aryA.Length != aryB.Length)
            {
                return false;
            }

            for (int i = 0; i < aryA.Length; i++)
            {
                if (!Equals(aryA[i], aryB[i]))
                {
                    return false;
                }
            }

            return true;
        }
        public static bool ArrayEquals<T>(T[] aryA, T[] aryB, int start, int len)
        {
            if (aryA == null && aryB == null)
            {
                return true;
            }
            if (aryA == null || aryB == null)
            {
                return false;
            }
            if (aryA.Length < len || aryB.Length < len)
            {
                return false;
            }

            for (int i = start; i < len; i++)
            {
                if (!Object.Equals(aryA[i], aryB[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static IEnumerable<T> TakeSafe<T>(this List<T> source, int count, Predicate<T> condition = null)
        {
            if (count <= 0)
            {
                yield break;
            }
            int num = 0;

            for (int i = 0; i < source.Count; i++)
            {
                T value = source[i];
                if (condition == null || condition(value))
                {
                    yield return value;
                }
                num++;
                if (num >= count)
                {
                    break;
                }
            }
        }
        public static IEnumerable<T> TakeLastSafe<T>(this List<T> source, int count, Predicate<T> condition = null)
        {
            if (count <= 0)
            {
                yield break;
            }
            int num = 0;

            for (int i = source.Count - 1; i >= 0; i--)
            {
                T value = source[i];
                if (condition == null || condition(value))
                {
                    yield return value;
                }
                num++;
                if (num >= count)
                {
                    break;
                }
            }
        }
        public static IEnumerable<T> ExceptHashSet<T>(this IEnumerable<T> source, HashSet<T> except)
        {
            foreach (var item in source)
            {
                if (!except.Contains(item))
                {
                    yield return item;
                }
            }
        }


        public static int GetPageCount(int totalItemCount, int pageItemCount)
        {
            if (totalItemCount == 0)
            {
                return 1;
            }

            return (totalItemCount - 1) / pageItemCount + 1;
        }
        public static IEnumerable<T> GetPagedItems<T>(this List<T> list, int pageIndex, int pageItemCount)
        {
            int startIndex = pageIndex * pageItemCount;

            if (startIndex < 0 || startIndex >= list.Count)
            {
                yield break;
            }

            int endCount = Math.Min(list.Count, startIndex + pageItemCount);

            for (int i = startIndex; i < endCount; i++)
            {
                yield return list[i];
            }
        }
        public static IEnumerable<T> GetPagedItems<T>(this T[] array, int pageIndex, int pageItemCount)
        {
            int startIndex = pageIndex * pageItemCount;

            if (startIndex < 0 || startIndex >= array.Length)
            {
                yield break;
            }

            int endCount = Math.Min(array.Length, startIndex + pageItemCount);

            for (int i = startIndex; i < endCount; i++)
            {
                yield return array[i];
            }
        }

    }
}
