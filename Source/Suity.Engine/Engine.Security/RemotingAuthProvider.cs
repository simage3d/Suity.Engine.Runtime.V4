// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using Suity.Networking;
using Suity.Helpers;
using Suity.Synchonizing;
using Suity.Collections;

namespace Suity.Engine.Security
{
    public class RemotingAuthProvider : BaseRemotingProvider<INetworkAuthentication, RemotingAuthProvider.Proxy>
    {
        public override string Icon => "*CoreIcon|Auth";

        public class Proxy : MarshalByRefObject, INetworkAuthentication
        {
            readonly INetworkAuthentication _inner;

            public Proxy()
            {
                _inner = Suity.Environment.GetService<INetworkAuthentication>();
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }

            public NetworkUser AuthenticateSession(string sessionId, string ip)
            {
                var user = _inner?.AuthenticateSession(sessionId, ip);
                if (user != null)
                {
                    return new AuthNetworkUser(user.UserId)
                    {
                        Token = user.Token,
                        _claims = user.Claims?.ToArray() ?? EmptyArray<string>.Empty,
                    };
                }
                else
                {
                    return null;
                }
            }

            public NetworkUser AuthenticateUserId(string userId, string password, string ip)
            {
                var user = _inner?.AuthenticateUserId(userId, password, ip);
                if (user != null)
                {
                    return new AuthNetworkUser(user.UserId)
                    {
                        Token = user.Token,
                        _claims = user.Claims?.ToArray() ?? EmptyArray<string>.Empty,
                    };
                }
                else
                {
                    return null;
                }
            }

            public NetworkUser AuthenticateEmail(string email, string password, string ip)
            {
                var user = _inner?.AuthenticateEmail(email, password, ip);
                if (user != null)
                {
                    return new AuthNetworkUser(user.UserId)
                    {
                        Token = user.Token,
                        _claims = user.Claims?.ToArray() ?? EmptyArray<string>.Empty,
                    };
                }
                else
                {
                    return null;
                }
            }

            public void AddClaim(string claim, string description)
            {
                _inner?.AddClaim(claim, description);
            }
        }

        [Serializable]
        class AuthNetworkUser : NetworkUser
        {
            internal string[] _claims;

            public override ICollection<string> Claims => _claims;

            public AuthNetworkUser(string userId)
                : base(userId)
            {
            }
        }
    }
}
