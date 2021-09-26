// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using Suity.Collections;
    using Suity.Engine;

    public static class NetworkHelper
    {
        static readonly Random _rnd = new Random();

        public static int AllocateTcpPort(int portMin, int portMax)
        {
            HashSet<int> ports = new HashSet<int>();

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            ports.AddRange(ipProperties.GetActiveTcpListeners().Select(o => o.Port));

            for (int iPort = portMin; iPort <= portMax; iPort++)
            {
                if (!ports.Contains(iPort))
                {
                    return iPort;
                }
            }
            return -1;
        }
        public static int AllocateTcpPortRandom(int portMin, int portMax, int retryTimes = 20)
        {
            HashSet<int> ports = new HashSet<int>();

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            ports.AddRange(ipProperties.GetActiveTcpListeners().Select(o => o.Port));

            for (int i = 0; i < retryTimes; i++)
            {
                int iPort = _rnd.Range(portMin, portMax);
                if (!ports.Contains(iPort))
                {
                    return iPort;
                }
            }
            //最终按顺序来
            return AllocateTcpPort(portMin, portMax);
        }

        public static int AllocateUdpPort(int portMin, int portMax)
        {
            HashSet<int> ports = new HashSet<int>();

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            ports.AddRange(ipProperties.GetActiveUdpListeners().Select(o => o.Port));

            for (int iPort = portMin; iPort <= portMax; iPort++)
            {
                if (!ports.Contains(iPort))
                {
                    return iPort;
                }
            }
            return -1;
        }
        public static int AllocateUdpPortRandom(int portMin, int portMax, int retryTimes = 20)
        {
            HashSet<int> ports = new HashSet<int>();

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            ports.AddRange(ipProperties.GetActiveUdpListeners().Select(o => o.Port));

            for (int i = 0; i < retryTimes; i++)
            {
                int iPort = _rnd.Range(portMin, portMax);
                if (!ports.Contains(iPort))
                {
                    return iPort;
                }
            }
            //最终按顺序来
            return AllocateUdpPort(portMin, portMax);
        }

        public static bool IsTcpPortInUse(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            foreach (IPEndPoint endPoint in ipProperties.GetActiveTcpListeners())
            {
                if (endPoint.Port == port)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsUdpPortInUse(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            foreach (IPEndPoint endPoint in ipProperties.GetActiveUdpListeners())
            {
                if (endPoint.Port == port)
                {
                    return true;
                }
            }
            return false;
        }
    }
}