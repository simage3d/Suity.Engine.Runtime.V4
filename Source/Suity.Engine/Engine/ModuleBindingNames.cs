// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Engine
{
    public static class ModuleBindingNames
    {
        public const string TcpServerSocket = "TcpServerSocket";
        public const string TcpClientSocket = "TcpClientSocket";

        public const string UdpServerSocket = "UdpServerSocket";
        public const string UdpClientSocket = "UdpClientSocket";

        public const string HttpService = "HttpService";
        public const string HttpRequest = "HttpRequest";

        public const string MessageQueue = "MessageQueue";

        public const string AutoDiscovery = "AutoDiscovery";
    }
}
