// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public abstract class ResourceObject : Suity.Object
    {
        public string Key { get; protected set; }

        public void MarkAccess()
        {
            string key = Key;
            if (!string.IsNullOrEmpty(key))
            {
                Logs.AddResourceLog(key, null);
            }
        }
        public void MarkAccess(string message)
        {
            string key = Key;
            if (!string.IsNullOrEmpty(key))
            {
                Logs.AddResourceLog(key, message);
            }
        }
    }
}
