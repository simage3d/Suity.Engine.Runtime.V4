// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Suity.Synchonizing
{
    public static class SyncExtensions
    {
        public static T SyncEnumAttribute<T>(this IPropertySync sync, string name, T value) where T : struct
        {
            string str = value.ToString();
            str = sync.Sync(name, str, SyncFlag.AttributeMode);

            try
            {
                return (T)Enum.Parse(typeof(T), str);
            }
            catch (Exception)
            {
                return value;
            }
        }
        public static bool SyncBooleanAttribute(this IPropertySync sync, string name, bool value)
        {
            string str = sync.Sync(name, value.ToString(), SyncFlag.AttributeMode);

            if (Boolean.TryParse(str, out bool result))
            {
                return result;
            }
            else
            {
                return value;
            }
        }
        public static int SyncInt32Attribute(this IPropertySync sync, string name, int value)
        {
            string str = sync.Sync(name, value.ToString(), SyncFlag.AttributeMode);

            if (Int32.TryParse(str, out int result))
            {
                return result;
            }
            else
            {
                return value;
            }
        }


        public static void SyncGenericIList<T>(this IIndexSync sync, IList<T> list, Type elementType = null, Predicate<T> check = null, Func<object> createNew = null, Action<T> added = null, Action<T> removed = null)
        {
            if (sync == null)
            {
                throw new ArgumentNullException(nameof(sync));
            }
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            T oldValue;
            T value;

            switch (sync.Mode)
            {
                case SyncMode.RequestElementType:
                    sync.Sync(0, elementType ?? typeof(T));
                    break;
                case SyncMode.Get:
                    if (sync.Index >= 0 && sync.Index < list.Count)
                    {
                        sync.Sync(sync.Index, list[sync.Index]);
                    }
                    break;
                case SyncMode.Set:
                    value = sync.Sync(sync.Index, list.GetIListItemSafe(sync.Index));
                    if (check == null || check(value))
                    {
                        oldValue = list.GetIListItemSafe(sync.Index);
                        if (oldValue != null && removed != null)
                        {
                            removed(oldValue);
                        }
                        list.EnsureIListSize(sync.Index + 1);
                        list[sync.Index] = value;
                        added?.Invoke(value);
                    }
                    break;
                case SyncMode.GetAll:
                    for (int i = 0; i < list.Count; i++)
                    {
                        sync.Sync(i, list[i]);
                    }
                    break;
                case SyncMode.SetAll:
                    if (removed != null)
                    {
                        foreach (var remove in list)
                        {
                            removed(remove);
                        }
                    }
                    list.Clear();
                    for (int i = 0; i < sync.Count; i++)
                    {
                        value = sync.Sync<T>(i, default(T));
                        if (check == null || check(value))
                        {
                            list.Add(value);
                            added?.Invoke(value);
                        }
                    }
                    break;
                case SyncMode.Insert:
                    value = sync.Sync(sync.Index, default(T));
                    if (check == null || check(value))
                    {
                        list.Insert(sync.Index, value);
                        added?.Invoke(value);
                    }
                    break;
                case SyncMode.RemoveAt:
                    oldValue = list[sync.Index];
                    if (oldValue != null && removed != null)
                    {
                        removed(oldValue);
                    }
                    list.RemoveAt(sync.Index);
                    break;
                case SyncMode.CreateNew:
                    if (createNew != null)
                    {
                        sync.Sync<object>(0, createNew());
                    }
                    else
                    {
                        if (elementType != null)
                        {
                            sync.Sync<T>(0, (T)Activator.CreateInstance(elementType));
                        }
                        else if (typeof(T).IsPrimitive)
                        {
                            sync.Sync<T>(0, default(T));
                        }
                        else if (typeof(T) == typeof(string))
                        {
                            sync.Sync<T>(0, (T)(object)string.Empty);
                        }
                        else
                        {
                            sync.Sync<T>(0, (T)Activator.CreateInstance(typeof(T)));
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public static void SyncGenericIListReadOnly<T>(this IIndexSync sync, IList<T> list)
        {
            if (sync == null)
            {
                throw new ArgumentNullException(nameof(sync));
            }
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            switch (sync.Mode)
            {
                case SyncMode.RequestElementType:
                    break;
                case SyncMode.Get:
                    if (sync.Index >= 0 && sync.Index < list.Count)
                    {
                        sync.Sync(sync.Index, list[sync.Index]);
                    }
                    break;
                case SyncMode.GetAll:
                    for (int i = 0; i < list.Count; i++)
                    {
                        sync.Sync(i, list[i]);
                    }
                    break;
                default:
                    break;
            }
        }
        public static void SyncIList(this IIndexSync sync, IList list, Type elementType, Predicate<object> check = null, Func<object> createNew = null, Action<object> added = null, Action<object> removed = null)
        {
            if (sync == null)
            {
                throw new ArgumentNullException(nameof(sync));
            }
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (elementType == null)
            {
                throw new ArgumentNullException(nameof(elementType));
            }

            object oldValue;
            object value;

            switch (sync.Mode)
            {
                case SyncMode.RequestElementType:
                    sync.Sync(0, elementType);
                    break;
                case SyncMode.Get:
                    if (sync.Index >= 0 && sync.Index < list.Count)
                    {
                        sync.Sync(sync.Index, list[sync.Index]);
                    }
                    break;
                case SyncMode.Set:
                    value = sync.Sync<object>(sync.Index, list.GetListItemSafe(sync.Index));
                    if (check == null || check(value))
                    {
                        oldValue = list.GetListItemSafe(sync.Index);
                        if (oldValue != null && removed != null)
                        {
                            removed(oldValue);
                        }
                        list.EnsureListSize(sync.Index + 1);
                        list[sync.Index] = value;
                        added?.Invoke(value);
                    }
                    break;
                case SyncMode.GetAll:
                    for (int i = 0; i < list.Count; i++)
                    {
                        sync.Sync(i, list[i]);
                    }
                    break;
                case SyncMode.SetAll:
                    if (removed != null)
                    {
                        foreach (var remove in list)
                        {
                            removed(remove);
                        }
                    }
                    list.Clear();
                    for (int i = 0; i < sync.Count; i++)
                    {
                        value = sync.Sync<object>(i, null);
                        if (check == null || check(value))
                        {
                            list.Add(value);
                            added?.Invoke(value);
                        }
                    }
                    break;
                case SyncMode.Insert:
                    value = sync.Sync<object>(sync.Index, null);
                    if (check == null || check(value))
                    {
                        list.Insert(sync.Index, value);
                        added?.Invoke(value);
                    }
                    break;
                case SyncMode.RemoveAt:
                    oldValue = list[sync.Index];
                    if (oldValue != null && removed != null)
                    {
                        removed(oldValue);
                    }
                    list.RemoveAt(sync.Index);
                    break;
                case SyncMode.CreateNew:
                    if (createNew != null)
                    {
                        sync.Sync<object>(0, createNew());
                    }
                    else
                    {
                        sync.Sync<object>(0, Activator.CreateInstance(elementType));
                    }
                    break;
                default:
                    break;
            }
        }
        public static void SyncListOperation(this IIndexSync sync, IIndexSyncOperation list)
        {
            if (sync == null)
            {
                throw new ArgumentNullException(nameof(sync));
            }
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            object value;

            switch (sync.Mode)
            {
                case SyncMode.RequestElementType:
                    sync.Sync(0, list.GetElementType(sync));
                    break;
                case SyncMode.Get:
                    if (sync.Index >= 0 && sync.Index < list.Count)
                    {
                        sync.Sync(sync.Index, list.GetItem(sync, sync.Index));
                    }
                    break;
                case SyncMode.Set:
                    value = sync.Sync<object>(sync.Index, list.GetItem(sync, sync.Index));
                    list.SetItem(sync, sync.Index, value);
                    break;
                case SyncMode.GetAll:
                    for (int i = 0; i < list.Count; i++)
                    {
                        sync.Sync(i, list.GetItem(sync, i));
                    }
                    break;
                case SyncMode.SetAll:
                    list.Clear(sync);
                    for (int i = 0; i < sync.Count; i++)
                    {
                        value = sync.Sync<object>(i, null);
                        list.Insert(sync, i, value);
                    }
                    break;
                case SyncMode.Insert:
                    value = sync.Sync<object>(sync.Index, null);
                    list.Insert(sync, sync.Index, value);
                    break;
                case SyncMode.RemoveAt:
                    list.RemoveAt(sync, sync.Index);
                    break;
                case SyncMode.CreateNew:
                    sync.Sync<object>(0, list.CreateNew(sync));
                    break;
                default:
                    break;
            }
        }

        public static bool IsSingleSetterOf(this IPropertySync sync, string propertyName)
        {
            return sync.Mode == SyncMode.Set && sync.Name == propertyName;
        }
        public static bool IsSingleGetterOf(this IPropertySync sync, string propertyName)
        {
            return sync.Mode == SyncMode.Get && sync.Name == propertyName;
        }
        public static bool IsSetterOf(this IPropertySync sync, string propertyName)
        {
            return (sync.Mode == SyncMode.Set && sync.Name == propertyName) ||
                sync.Mode == SyncMode.SetAll;
        }
        public static bool IsGetterOf(this IPropertySync sync, string propertyName)
        {
            return (sync.Mode == SyncMode.Get && sync.Name == propertyName) ||
                sync.Mode == SyncMode.GetAll;
        }
        public static bool IsSingleSetterOf(this IPropertySync sync, params string[] propertyNames)
        {
            return sync.Mode == SyncMode.Set && propertyNames.Any(name => sync.Name == name);
        }
        public static bool IsSingleGetterOf(this IPropertySync sync, params string[] propertyNames)
        {
            return sync.Mode == SyncMode.Get && propertyNames.Any(name => sync.Name == name);
        }
        public static bool IsSetterOf(this IPropertySync sync, params string[] propertyNames)
        {
            return (sync.Mode == SyncMode.Set && propertyNames.Any(name => sync.Name == name)) ||
                sync.Mode == SyncMode.SetAll;
        }
        public static bool IsGetterOf(this IPropertySync sync, params string[] propertyNames)
        {
            return (sync.Mode == SyncMode.Get && propertyNames.Any(name => sync.Name == name)) ||
                sync.Mode == SyncMode.GetAll;
        }

        public static bool IsSetter(this IPropertySync sync)
        {
            switch (sync.Mode)
            {
                case SyncMode.Set:
                case SyncMode.SetAll:
                case SyncMode.Insert:
                case SyncMode.RemoveAt:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsGetter(this IPropertySync sync)
        {
            switch (sync.Mode)
            {
                case SyncMode.Get:
                case SyncMode.GetAll:
                case SyncMode.CreateNew:
                case SyncMode.RequestElementType:
                    return true;
                default:
                    return false;
            }
        }


        public static bool IsSetter(this IIndexSync sync)
        {
            switch (sync.Mode)
            {
                case SyncMode.Set:
                case SyncMode.SetAll:
                case SyncMode.Insert:
                case SyncMode.RemoveAt:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsGetter(this IIndexSync sync)
        {
            switch (sync.Mode)
            {
                case SyncMode.Get:
                case SyncMode.GetAll:
                case SyncMode.CreateNew:
                case SyncMode.RequestElementType:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsSetterOf(this IIndexSync sync, int index)
        {
            return (sync.Mode == SyncMode.Set && sync.Index == index) ||
                sync.Mode == SyncMode.SetAll;
        }
        public static bool IsGetterOf(this IIndexSync sync, int index)
        {
            return (sync.Mode == SyncMode.Get && sync.Index == index) ||
                sync.Mode == SyncMode.GetAll;
        }


        public static T GetService<T>(this ISyncContext context) where T : class
        {
            return context.GetService(typeof(T)) as T;
        }
    }
}
