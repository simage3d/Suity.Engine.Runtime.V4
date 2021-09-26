// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections.Generic;
using System.Linq;
using Suity.Helpers;

namespace Suity.Collections
{
    /// <summary>
    /// 简易多值字典
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class UniqueMultiDictionary<TKey, TValue>
    {
        int _count;
        readonly Dictionary<TKey, HashSet<TValue>> _dic;

        public UniqueMultiDictionary()
        {
            _dic = new Dictionary<TKey, HashSet<TValue>>();
        }
        public UniqueMultiDictionary(IEqualityComparer<TKey> comparer)
        {
            _dic = new Dictionary<TKey, HashSet<TValue>>(comparer);
        }

        public bool Add(TKey key, TValue value)
        {
            if (EnsureSet(key).Add(value))
            {
                _count++;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Remove(TKey key, TValue value)
        {
            if (_dic.TryGetValue(key, out HashSet<TValue> set))
            {
                bool removed = set.Remove(value);
                if (removed)
                {
                    _count--;
                    if (set.Count == 0)
                    {
                        _dic.Remove(key);
                    }
                }
                return removed;
            }
            else
            {
                return false;
            }
        }
        public void RemoveAll(TKey key)
        {
            if (_dic.TryGetValue(key, out HashSet<TValue> set))
            {
                int removeCount = set.Count;
                _dic.Remove(key);
                _count -= removeCount;
            }
        }
        public int RemoveAllValues(TValue value)
        {
            TKey[] keys = Keys.ToArray();
            return keys.Count(key => Remove(key, value));
        }
        public void Clear()
        {
            _dic.Clear();
            _count = 0;
        }
        /// <summary>
        /// 替换键的值到新的键上
        /// </summary>
        /// <param name="oldKey">就键值</param>
        /// <param name="newKey">新简直</param>
        /// <returns>若成功替换，返回true。若旧值不存在或新值已存在则返回false。</returns>
        public bool RenameKey(TKey oldKey, TKey newKey)
        {
            if (_dic.TryGetValue(oldKey, out HashSet<TValue> set) && !_dic.ContainsKey(newKey))
            {
                _dic.Remove(oldKey);
                _dic.Add(newKey, set);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void RenameCombineKey(TKey oldKey, TKey newKey)
        {
            if (_dic.TryGetValue(oldKey, out HashSet<TValue> oldSet))
            {
                _dic.Remove(oldKey);

                if (_dic.TryGetValue(newKey, out HashSet<TValue> newSet))
                {
                    newSet.AddRange(oldSet);
                }
                else
                {
                    _dic.Add(newKey, oldSet);
                }
            }
        }

        public int Count { get { return _dic.Count; } }
        public int ValueCount { get { return _count; } }
        public int GetValueCount(TKey key)
        {
            if (_dic.TryGetValue(key, out HashSet<TValue> set))
            {
                return set.Count;
            }
            else
            {
                return 0;
            }
        }
        public int KeyCount { get { return _dic.Count; } }
        public bool ContainsKey(TKey key)
        {
            return _dic.ContainsKey(key);
        }
        public bool Contains(TKey key, TValue value)
        {
            if (_dic.TryGetValue(key, out HashSet<TValue> set))
            {
                return set.Contains(value);
            }
            else
            {
                return false;
            }
        }
        public TValue GetFirstOrDefault(TKey key)
        {
            if (_dic.TryGetValue(key, out HashSet<TValue> set))
            {
                return set.FirstOrDefault();
            }
            else
            {
                return default(TValue);
            }
        }
        public IEnumerable<TKey> Keys
        {
            get { return _dic.Keys; }
        }
        public IEnumerable<TKey> GetKeysByValue(TValue value)
        {
            foreach (var pair in _dic)
            {
                if (pair.Value.Contains(value))
                {
                    yield return pair.Key;
                }
            }
        }

        public IEnumerable<TValue> this[TKey key]
        {
            get
            {
                if (_dic.TryGetValue(key, out HashSet<TValue> set))
                {
                    foreach (TValue value in set)
                    {
                        yield return value;
                    }
                }
            }
        }
        public IEnumerable<TValue> Values
        {
            get
            {
                foreach (HashSet<TValue> set in _dic.Values)
                {
                    foreach (TValue value in set)
                    {
                        yield return value;
                    }
                }
            }
        }
        public IEnumerable<TValue> SoloValues
        {
            get
            {
                foreach (HashSet<TValue> set in _dic.Values)
                {
                    if (set.Count == 1)
                    {
                        foreach (TValue value in set)
                        {
                            yield return value;
                        }
                    }
                }
            }
        }
        public IEnumerable<TValue> FirstValues
        {
            get
            {
                foreach (HashSet<TValue> set in _dic.Values)
                {
                    if (set.Count > 0)
                    {
                        yield return set.First();
                    }
                }
            }
        }
        public IEnumerable<KeyValuePair<TKey, TValue>> Pairs
        {
            get
            {
                foreach (var pair in _dic)
                {
                    foreach (var value in pair.Value)
                    {
                        yield return new KeyValuePair<TKey, TValue>(pair.Key, value);
                    }
                }
            }
        }

        private HashSet<TValue> EnsureSet(TKey key)
        {
            if (!_dic.TryGetValue(key, out HashSet<TValue> set))
            {
                set = new HashSet<TValue>();
                _dic.Add(key, set);
            }
            return set;
        }



        public UniqueMultiDictionary<TKey, TValue> Clone()
        {
            UniqueMultiDictionary<TKey, TValue> clone = new UniqueMultiDictionary<TKey, TValue>();

            foreach (var pair in _dic)
            {
                HashSet<TValue> hashSet = new HashSet<TValue>(pair.Value);
                clone._dic.Add(pair.Key, hashSet);
            }

            return clone;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", GetType().Name, Count);
        }
    }
}
