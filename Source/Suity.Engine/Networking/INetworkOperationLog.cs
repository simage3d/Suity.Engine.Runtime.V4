// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    public interface INetworkOperationLog
    {
        void AddOperationLog(int level, string category, string userId, string ip, string data, bool successful);
    }
}
