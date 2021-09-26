// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Synchonizing;
using Suity.Views;
using Suity.Engine;

namespace Suity.Networking
{
    public enum IPAllocationMode
    {
        LocalHost,
        PublicIP,
        InternalIP,
        EnvironmentConfig,
        Custom,
    }

    public class IPConfig : IViewObject
    {
        public const string LocalHost = "127.0.0.1";

        public IPAllocationMode Mode { get; set; } = IPAllocationMode.LocalHost;
        public string IP { get; set; } = LocalHost;
        public string ConfigKey { get; set; }


        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            if (NodeApplication.Current == null)
            {
                setup.InspectorField(Mode, new ViewProperty("Mode"));
                switch (Mode)
                {
                    case IPAllocationMode.EnvironmentConfig:
                        setup.InspectorField(ConfigKey, new ViewProperty("ConfigKey"));
                        break;
                    case IPAllocationMode.Custom:
                        setup.InspectorField(IP, new ViewProperty("IP"));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                setup.InspectorField(IP, new ViewProperty("LiveValue") { ReadOnly = true });
            }
        }

        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            Mode = sync.Sync("Mode", Mode, SyncFlag.AffectsOthers, IPAllocationMode.LocalHost);
            IP = sync.Sync("IP", IP, SyncFlag.NotNull, LocalHost);
            ConfigKey = sync.Sync("ConfigKey", ConfigKey);

            if (NodeApplication.Current != null)
            {
                sync.Sync("LiveValue", GetIPAddress(), SyncFlag.ReadOnly);
            }
        }

        public string GetIPAddress()
        {
            if (NodeApplication.Current == null)
            {
                return null;
            }

            switch (Mode)
            {
                case IPAllocationMode.LocalHost:
                    return LocalHost;
                case IPAllocationMode.PublicIP:
                    return NodeApplication.Current?.PublicIPAddress ?? LocalHost;
                case IPAllocationMode.InternalIP:
                    return NodeApplication.Current?.InternalIPAddress ?? LocalHost;
                case IPAllocationMode.EnvironmentConfig:
                    string value = Environment.GetConfig(ConfigKey);
                    if (value == null)
                    {
                        Logs.LogError("Environment config is not set : " + ConfigKey);
                    }
                    return value;
                case IPAllocationMode.Custom:
                default:
                    return IP;
            }
        }

        public override string ToString()
        {
            if (NodeApplication.Current == null)
            {
                switch (Mode)
                {
                    case IPAllocationMode.LocalHost:
                        return "LocalHost";
                    case IPAllocationMode.PublicIP:
                        return "PublicIP";
                    case IPAllocationMode.InternalIP:
                        return "InternalIP";
                    case IPAllocationMode.EnvironmentConfig:
                        return "Config:" + ConfigKey;
                    case IPAllocationMode.Custom:
                    default:
                        return IP;
                }
            }
            else
            {
                return GetIPAddress();
            }
        }
    }
}
