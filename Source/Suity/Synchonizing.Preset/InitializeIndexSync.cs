// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Suity.Synchonizing.Preset
{
    public class InitializeIndexSync : MarshalByRefObject, IIndexSync
    {
        #region IIndexSync 成员

        public SyncMode Mode
        {
            get { return SyncMode.Initialize; }
        }

        public SyncIntent Intent { get { return SyncIntent.None; } }

        public int Count
        {
            get { return 0; }
        }

        public int Index
        {
            get { return 0; }
        }

        public object Value
        {
            get { return null; }
        }

        public int SyncCount(int count)
        {
            return count;
        }

        public T Sync<T>(int index, T obj, SyncFlag flag = SyncFlag.None)
        {
            return obj;
        }

        public string SyncAttribute(string name, string value)
        {
            return value;
        }

        #endregion
    }
}
