// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.AutoDiscovery;

namespace Suity.Engine.Debugging
{
    public static class LocationHelpers
    {
        public static DebugServiceLocation ToDebugLocation(this ServiceLocation location)
        {
            return new DebugServiceLocation
            {
                ServiceCode = location.ServiceCode,
                IPAddress = location.IPAddress,
                Port = location.Port,
                Url = location.Url,
            };
        }


        public static ServiceLocation ToLocation(this DebugServiceLocation apiLocation)
        {
            ServiceCode code;
            if (ServiceCode.TryParse(apiLocation.ServiceCode, out code))
            {
                return new ServiceLocation(code, apiLocation.IPAddress, apiLocation.Port, apiLocation.Url);
            }
            else
            {
                return null;
            }
        }
    }
}
