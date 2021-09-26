// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    [Serializable]
    public class NetworkUser
    {
        readonly string _userId;

        /// <summary>
        /// Gets the id of the current user.
        /// </summary>
        public string UserId => _userId;

        /// <summary>
        /// Gets or sets the auth token of the current user.
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// Gets the claims of the current user.
        /// </summary>
        public virtual ICollection<string> Claims => EmptyArray<string>.Empty;

        public NetworkUser(string userId)
        {
            _userId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }
}
