// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Suity.Collections
{
    public static class CollectionExtensions
    {
        #region IList
        public static TValue GetValueOrDefault<TValue>(this IList<TValue> list, int index)
        {
            if (index >= 0 && index < list.Count)
            {
                return list[index];
            }
            else
            {
                return default(TValue);
            }
        }

        public static void InsertSorted<T>(this IList<T> list, T item, Comparison<T> compare)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (compare(item, list[i]) < 0)
                {
                    list.Insert(i, item);
                    return;
                }
            }
            list.Add(item);
        } 

        public static bool ElementEquals<T>(this IList<T> list, IList<T> other)
        {
            if (list == null || other == null)
            {
                return false;
            }
            if (list.Count != other.Count)
            {
                return false;
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (!object.Equals(list[i], other[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static int IndexOf<T>(this IList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region IDictionary
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TValue> source, Func<TValue, TKey> keyResolve)
        {
            foreach (var value in source)
            {
                TKey key = keyResolve(value);
                dictionary[key] = value;
            }
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (key == null)
            {
                return default(TValue);
            }

            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                return default(TValue);
            }
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defalutValue)
        {
            if (key == null)
            {
                return default(TValue);
            }

            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                return defalutValue;
            }
        }


        public static TValue GetValueOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> creation)
        {
            if (key == null)
            {
                return default(TValue);
            }

            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                value = creation();
                dictionary.Add(key, value);
                return value;
            }
        }

        public static TValue GetValueOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (key == null)
            {
                return default(TValue);
            }

            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                dictionary.Add(key, defaultValue);
                return defaultValue;
            }
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, factory());
                return true;
            }
            else
            {
                return false;
            }
        }

        public static TValue RemoveAndGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (key == null)
            {
                return default(TValue);
            }

            if (dictionary.TryGetValue(key, out TValue value))
            {
                dictionary.Remove(key);
                return value;
            }
            else
            {
                return default(TValue);
            }
        }
        public static bool TryRemoveAndGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                dictionary.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveAllByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Predicate<TValue> predicate)
        {
            List<TKey> removes = null;
            foreach (var pair in dictionary)
            {
                if (predicate(pair.Value))
                {
                    (removes ?? (removes = new List<TKey>())).Add(pair.Key);
                }
            }
            if (removes != null)
            {
                foreach (TKey key in removes)
                {
                    dictionary.Remove(key);
                }
            }
        }
        public static IEnumerable<TValue> RemoveAllByValueAndGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Predicate<TValue> predicate)
        {
            List<KeyValuePair<TKey, TValue>> removes = null;
            foreach (var pair in dictionary)
            {
                if (predicate(pair.Value))
                {
                    (removes ?? (removes = new List<KeyValuePair<TKey, TValue>>())).Add(pair);
                }
            }
            if (removes != null)
            {
                foreach (var pair in removes)
                {
                    dictionary.Remove(pair.Key);
                }
                return removes.Select(o => o.Value);
            }
            else
            {
                return EmptyArray<TValue>.Empty;
            }
        }

        public static void RemoveAllByKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Predicate<TKey> predicate)
        {
            List<TKey> removes = null;
            foreach (var pair in dictionary)
            {
                if (predicate(pair.Key))
                {
                    (removes ?? (removes = new List<TKey>())).Add(pair.Key);
                }
            }
            if (removes != null)
            {
                foreach (TKey key in removes)
                {
                    dictionary.Remove(key);
                }
            }
        }
        public static void RemoveAllByKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                dictionary.Remove(key);
            }
        }
        public static IEnumerable<TValue> RemoveAllByKeyAndGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Predicate<TKey> predicate)
        {
            List<KeyValuePair<TKey, TValue>> removes = null;
            foreach (var pair in dictionary)
            {
                if (predicate(pair.Key))
                {
                    (removes ?? (removes = new List<KeyValuePair<TKey, TValue>>())).Add(pair);
                }
            }
            if (removes != null)
            {
                foreach (var pair in removes)
                {
                    dictionary.Remove(pair.Key);
                }
                return removes.Select(o => o.Value);
            }
            else
            {
                return EmptyArray<TValue>.Empty;
            }
        }
        public static IEnumerable<TValue> RemoveAllByKeyAndGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
        {
            List<TValue> removes = null;
            foreach (var key in keys)
            {
                if (dictionary.TryRemoveAndGet(key, out TValue value))
                {
                    (removes ?? (removes = new List<TValue>())).Add(value);
                }
            }

            return removes ?? (IEnumerable<TValue>)EmptyArray<TValue>.Empty;
        }
        public static IDictionary<TKey, object> ConvertToObjectDictionary<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary.ToDictionary<KeyValuePair<TKey, TValue>, TKey, object>(pair => pair.Key, pair => pair.Value);
        }
        #endregion

        #region ICollection
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> other)
        {
            if (other != null)
            {
                foreach (T item in other)
                {
                    collection.Add(item);
                }
            }
        }
        #endregion

        #region Stack

        public static T PopOrCreate<T>(this Stack<T> stack, Func<T> creation)
        {
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
            else
            {
                return creation();
            }
        }

        #endregion

        #region IEnumerable
        public static IEnumerable<T> ConcatMultiple<T>(this IEnumerable<T> collection, params IEnumerable<T>[] others)
        {
            foreach (T item in collection)
            {
                yield return item;
            }
            foreach (IEnumerable<T> other in others)
            {
                foreach (T item in other)
                {
                    yield return item;
                }
            }
        }
        public static IEnumerable<T> As<T>(this IEnumerable source) where T : class
        {
            foreach (var s in source)
            {
                yield return s as T;
            }
        }
        public static IEnumerable<T> Pass<T>(this IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                yield return item;
            }
        }
        public static IEnumerable<T> SkipNull<T>(this IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }
        public static IEnumerable<T> ConcatOne<T>(this IEnumerable<T> source, T other)
        {
            foreach (var item in source)
            {
                yield return item;
            }
            yield return other;
        }
        public static IEnumerable<T> WithAction<T>(this IEnumerable<T> source, Action<T> action) where T : class
        {
            foreach (var s in source)
            {
                action(s);
                yield return s;
            }
        }
        public static bool AllEqual<TSource>(this IEnumerable<TSource> source, bool emptyValue = false)
        {
            if (!source.Any())
            {
                return emptyValue;
            }
            if (source.CountOne())
            {
                return true;
            }

            TSource first = source.First();
            return source.Skip(1).All(o => Equals(o, first));
        }
        public static void Foreach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static bool CountOne(this IEnumerable source)
        {
            if (source == null) return false;
            int num = 0;
            foreach (var item in source)
            {
                num++;
                if (num > 1)
                {
                    return false;
                }
            }
            return num == 1;
        }
        public static bool CountMoreThanOne(this IEnumerable source)
        {
            if (source == null) return false;
            int num = 0;
            foreach (var item in source)
            {
                num++;
                if (num > 1)
                {
                    return true;
                }
            }
            return false;
        }


        public static Type GetCommonType(this IEnumerable<Type> source)
        {
            if (!source.Any())
            {
                return null;
            }

            Type commonType = source.OfType<Type>().FirstOrDefault();
            if (commonType is null)
            {
                return null;
            }

            foreach (Type o in source.Skip(1))
            {
                Type curType = o;
                while (commonType != curType && !commonType.IsAssignableFrom(curType))
                {
                    commonType = commonType.BaseType;
                }
            }
            return commonType;
        }
        public static Type GetCommonType(this IEnumerable<object> source)
        {
            if (!source.Any())
            {
                return null;
            }

            Type commonType = source.OfType<object>().FirstOrDefault()?.GetType();
            if (commonType is null)
            {
                return null;
            }

            foreach (object o in source.Skip(1))
            {
                Type curType = o.GetType();
                while (commonType != curType && !commonType.IsAssignableFrom(curType))
                {
                    commonType = commonType.BaseType;
                }
            }
            return commonType;
        }


        public static T OnlyOneOfDefault<T>(this IEnumerable<T> collection)
        {
            bool getValue = false;
            T value = default(T);

            foreach (var item in collection)
            {
                if (getValue)
                {
                    value = default(T);
                    break;
                }
                else
                {
                    getValue = true;
                    value = item;
                }
            }
            return value;
        }

        #endregion

        #region RangeCollection

        public static T GetItemSafe<T>(this RangeCollection<T> collection, int index)
        {
            if (index >= 0 && index < collection.Count)
            {
                var group = collection[index];
                if (group != null)
                {
                    return group.Value;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                return default(T);
            }
        }

        public static T GetItemMinMax<T>(this RangeCollection<T> collection, int index)
        {
            if (collection.Count == 0)
            {
                return default(T);
            }
            if (index < 0)
            {
                index = 0;
            }
            else if (index >= collection.Count)
            {
                index = collection.Count - 1;
            }

            var group = collection[index];
            if (group != null)
            {
                return group.Value;
            }
            else
            {
                return default(T);
            }

        }

        public static int FindIndexMinMax<T>(this RangeCollection<T> collection, int number)
        {
            if (collection.Count == 0)
            {
                return -1;
            }
            if (number <= collection[0].High)
            {
                return 0;
            }
            if (number >= collection[collection.Count - 1].Low)
            {
                return collection.Count - 1;
            }
            return collection.FindIndex(number);
        }

        #endregion
    }
}
