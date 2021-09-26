// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Suity.Collections
{
    public class RangeGroup<T>
    {
        public T Value { get; set; }
        public int Low { get; }
        public int High { get; }

        [Obsolete("Use Low instead.")]
        public int LowValue => Low;

        [Obsolete("Use High instead.")]
        public int HighValue => High;


        public RangeGroup(T value, int low, int high)
        {
            Value = value;
            Low = low;
            High = high;
        }

        public int CompareTo(int value)
        {
            if (value < Low)
            {
                return 1;
            }
            if (value > High)
            {
                return -1;
            }
            return 0;
        }

        public override string ToString()
        {
            return $"[{Low}-{High}] {Value}";
        }
    }

    public class RangeCollection<T>
    {
        readonly List<RangeGroup<T>> _groups = new List<RangeGroup<T>>();

        public void Append(T value, int length)
        {
            if (_groups.Count > 0)
            {
                var last = _groups[_groups.Count - 1];
                int low = last.High + 1;
                int high = last.High + length;
                var group = new RangeGroup<T>(value, low, high);
                _groups.Add(group);
            }
            else
            {
                _groups.Add(new RangeGroup<T>(value, 0, length - 1));
            }
        }
        public void Prepend(T value, int length)
        {
            if (_groups.Count > 0)
            {
                var last = _groups[_groups.Count - 1];
                int high = last.Low - 1;
                int low = last.Low - length;
                var group = new RangeGroup<T>(value, low, high);
                _groups.Add(group);
            }
            else
            {
                _groups.Add(new RangeGroup<T>(value, -1, -length));
            }
        }

        
        
        public void AddByHigh(int highValue, T value)
        {
            InternalAddByHigh(highValue, value, false);
        }
        public void AddByLow(int lowValue, T value)
        {
            InternalAddByLow(lowValue, value, false);
        }
        
        [Obsolete]
        public void AddByHighValue(int highValue, T value)
        {
            InternalAddByHigh(highValue, value, false);
        }
        [Obsolete]
        public void AddByLowValue(int lowValue, T value)
        {
            InternalAddByLow(lowValue, value, false);
        }


        public RangeGroup<T> GetOrAddByHigh(int high, T value)
        {
            return InternalAddByHigh(high, value, true);
        }
        public RangeGroup<T> GetOrAddByLow(int low, T value)
        {
            return InternalAddByLow(low, value, true);
        }
        
        [Obsolete]
        public RangeGroup<T> GetOrAddByHighValue(int highValue, T value)
        {
            return InternalAddByHigh(highValue, value, true);
        }
        [Obsolete]
        public RangeGroup<T> GetOrAddByLowValue(int lowValue, T value)
        {
            return InternalAddByLow(lowValue, value, true);
        }

        private RangeGroup<T> InternalAddByHigh(int high, T value, bool replace)
        {
            int index = FindIndex(high);
            if (index >= 0)
            {
                RangeGroup<T> current = _groups[index];
                if (current.High == high)
                {
                    if (replace)
                    {
                        current.Value = value;
                        return current;
                    }
                    else
                    {
                        throw new InvalidOperationException("High value exist : " + high);
                    }
                }

                RangeGroup<T> higherGroup = new RangeGroup<T>(current.Value, high + 1, current.High);
                RangeGroup<T> lowerGroup = new RangeGroup<T>(value, current.Low, high);
                _groups[index] = higherGroup;
                _groups.Insert(index, lowerGroup);
                return lowerGroup;
            }
            else
            {
                if (_groups.Count == 0)
                {
                    RangeGroup<T> newGroup = new RangeGroup<T>(value, int.MinValue, high);
                    _groups.Add(newGroup);
                    return newGroup;
                }
                else
                {
                    RangeGroup<T> last = _groups[_groups.Count - 1];
                    RangeGroup<T> newGroup = new RangeGroup<T>(value, last.High + 1, high);
                    _groups.Add(newGroup);
                    return newGroup;
                }
            }
        }
        private RangeGroup<T> InternalAddByLow(int low, T value, bool replace)
        {
            int index = FindIndex(low);
            if (index >= 0)
            {
                RangeGroup<T> current = _groups[index];
                if (current.Low == low)
                {
                    if (replace)
                    {
                        current.Value = value;
                        return current;
                    }
                    else
                    {
                        throw new InvalidOperationException("Low value exist : " + low);
                    }
                }

                RangeGroup<T> higherGroup = new RangeGroup<T>(value, low, current.High);
                RangeGroup<T> lowerGroup = new RangeGroup<T>(current.Value, current.Low, low - 1);

                _groups[index] = higherGroup;
                _groups.Insert(index, lowerGroup);
                return lowerGroup;
            }
            else
            {
                if (_groups.Count == 0)
                {
                    RangeGroup<T> newGroup = new RangeGroup<T>(value, low, int.MaxValue);
                    _groups.Add(newGroup);
                    return newGroup;
                }
                else
                {
                    RangeGroup<T> first = _groups[0];
                    RangeGroup<T> newGroup = new RangeGroup<T>(value, low, first.Low - 1);
                    _groups.Insert(0, newGroup);
                    return newGroup;
                }
            }
        }

        public T FindValue(int position)
        {
            int index = FindIndex(position);
            if (index >= 0)
            {
                return _groups[index].Value;
            }
            else
            {
                return default(T);
            }
        }
        public RangeGroup<T> FindRangeGroup(int position)
        {
            int index = FindIndex(position);
            if (index >= 0)
            {
                return _groups[index];
            }
            else
            {
                return null;
            }
        }

        public int FindIndex(int number)
        {
            int min = 0;
            int max = _groups.Count - 1;

            while (min <= max)
            {
                int mid = (min + max) / 2;
                int comparison = _groups[mid].CompareTo(number);
                if (comparison == 0)
                {
                    return mid;
                }
                if (comparison < 0)
                {
                    min = mid + 1;
                }
                else
                {
                    max = mid - 1;
                }
            }
            return -1;
        }
        public int FindIndexMinMax(int number)
        {
            if (_groups.Count == 0)
            {
                return -1;
            }

            var first = _groups[0];
            if (number < first.Low)
            {
                return 0;
            }

            var last = _groups[_groups.Count - 1];
            if (number > last.High)
            {
                return _groups.Count - 1;
            }

            return FindIndex(number);
        }
        
        public IEnumerable<RangeGroup<T>> RangeGroups => _groups.Select(o => o);
        public IEnumerable<T> Values => _groups.Select(o => o.Value);
        public void Clear() => _groups.Clear();


        public int Count => _groups.Count;

        public int TotalLength
        {
            get
            {
                if (_groups.Count > 0)
                {
                    return _groups[_groups.Count - 1].High - _groups[0].Low + 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        
        public RangeGroup<T> this[int index] => _groups[index];
        public int IndexOf(RangeGroup<T> group) => _groups.IndexOf(group);
    }
}
