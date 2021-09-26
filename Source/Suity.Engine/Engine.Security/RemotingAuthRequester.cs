// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using Suity.Networking;
using Suity.Collections;

namespace Suity.Engine.Security
{
    [NodeService(typeof(INetworkAuthentication))]
    public class RemotingAuthRequester : BaseRemotingRequester<INetworkAuthentication>, INetworkAuthentication
    {
        public override string Icon => "*CoreIcon|Auth";

        #region INetworkAuthentication
        public NetworkUser AuthenticateSession(string sessionId, string ip)
        {
            var user = Proxy?.AuthenticateSession(sessionId, ip);
            if (user != null)
            {
                return new AuthNetworkUser(user.UserId)
                {
                    Token = user.Token,
                    _claims = new HashSet<string>(user.Claims ?? EmptyArray<string>.Empty),
                };
            }
            else
            {
                return null;
            }
        }

        public NetworkUser AuthenticateUserId(string userId, string password, string ip)
        {
            var user = Proxy?.AuthenticateUserId(userId, password, ip);
            if (user != null)
            {
                return new AuthNetworkUser(user.UserId)
                {
                    Token = user.Token,
                    _claims = new HashSet<string>(user.Claims ?? EmptyArray<string>.Empty),
                };
            }
            else
            {
                return null;
            }
        }

        public NetworkUser AuthenticateEmail(string email, string password, string ip)
        {
            var user = Proxy?.AuthenticateEmail(email, password, ip);
            if (user != null)
            {
                return new AuthNetworkUser(user.UserId)
                {
                    Token = user.Token,
                    _claims = new HashSet<string>(user.Claims ?? EmptyArray<string>.Empty),
                };
            }
            else
            {
                return null;
            }
        }

        public void AddClaim(string claim, string description)
        {
            Proxy?.AddClaim(claim, description);
        }
        #endregion

        [Serializable]
        class AuthNetworkUser : NetworkUser
        {
            internal HashSet<string> _claims;

            public override ICollection<string> Claims => _claims;

            public AuthNetworkUser(string userId)
                : base(userId)
            {
            }
        }
    }
}
