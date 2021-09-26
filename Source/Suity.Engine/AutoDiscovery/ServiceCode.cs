// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.AutoDiscovery
{
    public class ServiceCode
    {
        public string Protocol { get; }
        public string ServiceName { get; }
        public string Version { get; }

        public ServiceCode(string protocol, string serviceName)
            : this(protocol, serviceName, "0")
        {
        }
        public ServiceCode(string protocol, string serviceName, string version)
        {
            if (string.IsNullOrEmpty(protocol))
            {
                throw new ArgumentNullException(nameof(protocol));
            }
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            Protocol = protocol;
            ServiceName = serviceName;
            Version = version;
        }

        public override string ToString()
        {
            return $"{Protocol}:{ServiceName}:{Version}";
        }

        public bool Equals(ServiceCode other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Protocol == other.Protocol &&
                this.ServiceName == other.ServiceName &&
                this.Version == other.Version;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is ServiceCode))
                return false;
            else
                return this.Equals((ServiceCode)obj);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public static bool operator ==(ServiceCode left, ServiceCode right)
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
        public static bool operator !=(ServiceCode left, ServiceCode right)
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
        public static implicit operator string(ServiceCode c)
        {
            return c.ToString();
        }

        public static ServiceCode Parse(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            string[] s = code.Split(':');
            if (s.Length != 3)
            {
                throw new FormatException("Code does not contain 3 components.");
            }
            if (string.IsNullOrEmpty(s[0]))
            {
                throw new FormatException("Protocol component is empty.");
            }
            if (string.IsNullOrEmpty(s[1]))
            {
                throw new FormatException("ServiceName component is empty.");
            }
            if (string.IsNullOrEmpty(s[2]))
            {
                throw new FormatException("Version component is empty.");
            }

            return new ServiceCode(s[0], s[1], s[2]);
        }
        public static bool TryParse(string code, out ServiceCode serviceCode)
        {
            serviceCode = null;
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }

            string[] s = code.Split(':');
            if (s.Length != 3)
            {
                return false;
            }
            if (string.IsNullOrEmpty(s[0]) || string.IsNullOrEmpty(s[1]) || string.IsNullOrEmpty(s[2]))
            {
                return false;
            }

            serviceCode = new ServiceCode(s[0], s[1], s[2]);
            return true;
        }
    }
}
