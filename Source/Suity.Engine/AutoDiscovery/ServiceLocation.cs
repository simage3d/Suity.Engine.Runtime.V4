// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.AutoDiscovery
{
    public class ServiceLocation
    {
        public ServiceCode ServiceCode { get; }

        public string IPAddress { get; }
        public int Port { get; }
        public string Url { get; }


        private ServiceLocation(ServiceCode serviceCode)
        {
            if (serviceCode == null)
            {
                throw new ArgumentNullException(nameof(serviceCode));
            }

            ServiceCode = serviceCode;
        }

        public ServiceLocation(ServiceCode serviceCode, string ipAddress, int port, string url)
            : this(serviceCode)
        {
            IPAddress = ipAddress;
            Port = port;
            Url = url;
        }
        public ServiceLocation(ServiceCode serviceCode, string url)
            : this(serviceCode)
        {
            Url = url;
        }
        public ServiceLocation(ServiceCode serviceCode, string ipAddress, int port)
            : this(serviceCode)
        {
            IPAddress = ipAddress;
            Port = port;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                return $"{ServiceCode}-{Url}";
            }
            else
            {
                return $"{ServiceCode}-{IPAddress}:{Port}";
            }
        }

        public bool Equals(ServiceLocation other)
        {
            if (other == null)
            {
                return false;
            }

            return this.ServiceCode == other.ServiceCode &&
                this.IPAddress == other.IPAddress &&
                this.Port == other.Port &&
                this.Url == other.Url;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is ServiceLocation))
                return false;
            else
                return this.Equals((ServiceLocation)obj);
        }
        public override int GetHashCode()
        {
            return ServiceCode.GetHashCode() ^ (IPAddress?.GetHashCode() ?? 0) ^ Port ^ (Url?.GetHashCode() ?? 0);
        }
        public static bool operator ==(ServiceLocation left, ServiceLocation right)
        {
            if (!ReferenceEquals(left, null))
            {
                return left.Equals(right);
            }
            else
            {
                return ReferenceEquals(right, null);
            }
        }
        public static bool operator !=(ServiceLocation left, ServiceLocation right)
        {
            if (!ReferenceEquals(left, null))
            {
                return !left.Equals(right);
            }
            else
            {
                return !ReferenceEquals(right, null);
            }
        }
    }
}
