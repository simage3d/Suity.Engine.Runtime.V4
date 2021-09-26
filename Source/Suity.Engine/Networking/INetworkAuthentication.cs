// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    public interface INetworkAuthentication
    {
        NetworkUser AuthenticateUserId(string userId, string password, string ip);
        NetworkUser AuthenticateEmail(string email, string password, string ip);
        NetworkUser AuthenticateSession(string sessionId, string ip);
        void AddClaim(string claim, string description);
    }
}
