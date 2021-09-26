// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Collections;
using Suity.Helpers;

namespace Suity.Synchonizing.Preset
{
    public sealed class InitializePropertySync : MarshalByRefObject, IPropertySync
    {
        public static readonly InitializePropertySync Instance = new InitializePropertySync();
        private InitializePropertySync()
        {
        }

        #region IPropertySync 成员

        public SyncMode Mode
        {
            get { return SyncMode.Initialize; }
        }

        public SyncIntent Intent { get { return SyncIntent.None; } }


        public string Name
        {
            get { return null; }
        }

        public IEnumerable<string> Names
        {
            get { return EmptyArray<string>.Empty; }
        }

        public object Value
        {
            get { return null; }
        }

        public T Sync<T>(string name, T obj, SyncFlag flag = SyncFlag.None, T defaultValue = default(T))
        {
            return obj;
        }

        #endregion
    }
}
