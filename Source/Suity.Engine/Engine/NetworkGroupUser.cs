// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Helpers;

namespace Suity.Networking
{
    public sealed class NetworkGroupUser : NetworkUser
    {
        public static readonly NetworkGroupUser Default = new NetworkGroupUser();

        private NetworkGroupUser()
            : base("#NetworkGroupUser")
        {
        }
    }
}
