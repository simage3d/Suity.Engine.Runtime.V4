// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Synchonizing;
using Suity.Views;
using Suity.Helpers;
using Suity.Engine;

namespace Suity.Networking
{
    public enum PortAllocationMode
    {
        /// <summary>
        /// 固定
        /// </summary>
        Fixed,
        /// <summary>
        /// 自动范围
        /// </summary>
        RangedAuto,
        /// <summary>
        /// 随机范围，用于避免同时启动冲突
        /// </summary>
        RangedRandom,
        /// <summary>
        /// 环境变量配置
        /// </summary>
        EnvironmentConfig,
    }

    public class PortConfig : IViewObject
    {
        public PortAllocationMode Mode { get; set; }
        public int Port { get; set; } = 2000;
        public int PortMin { get; set; } = 2000;
        public int PortMax { get; set; } = 4000;
        public string ConfigKey { get; set; }

        private int _cachedPort;


        public PortConfig()
        {
        }
        public PortConfig(int port)
        {
            Mode = PortAllocationMode.Fixed;
            Port = port;
        }

        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            if (NodeApplication.Current == null)
            {
                setup.InspectorField(Mode, new ViewProperty("Mode"));
                switch (Mode)
                {
                    case PortAllocationMode.Fixed:
                        setup.InspectorField(Port, new ViewProperty("Port"));
                        break;
                    case PortAllocationMode.RangedAuto:
                    case PortAllocationMode.RangedRandom:
                        setup.InspectorField(PortMin, new ViewProperty("PortMin"));
                        setup.InspectorField(PortMax, new ViewProperty("PortMax"));
                        break;
                    case PortAllocationMode.EnvironmentConfig:
                        setup.InspectorField(ConfigKey, new ViewProperty("ConfigKey"));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                setup.InspectorField(Port, new ViewProperty("LiveValue") { ReadOnly = true });
            }
        }
        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            Mode = sync.Sync("Mode", Mode, SyncFlag.AffectsOthers);
            Port = sync.Sync("Port", Port);
            PortMin = sync.Sync("PortMin", PortMin, SyncFlag.AffectsOthers);
            PortMax = sync.Sync("PortMax", PortMax, SyncFlag.AffectsOthers);
            ConfigKey = sync.Sync("ConfigKey", ConfigKey);

            if (sync.IsSingleSetterOf("Port"))
            {
                Port = Math.Max(Port, 0);
                Port = Math.Min(Port, 65535);
            }
            if (sync.IsSingleSetterOf("PortMin"))
            {
                PortMin = Math.Max(PortMin, 0);
                PortMin = Math.Min(PortMin, 65535);
                if (PortMax < PortMin)
                {
                    PortMax = PortMin;
                }
                Port = Math.Max(Port, PortMin);
            }
            if (sync.IsSingleSetterOf("PortMax"))
            {
                PortMax = Math.Max(PortMax, 0);
                PortMax = Math.Min(PortMax, 65535);
                if (PortMin > PortMax)
                {
                    PortMin = PortMax;
                }
                Port = Math.Min(Port, PortMax);
            }

            if (NodeApplication.Current != null)
            {
                sync.Sync("LiveValue", _cachedPort, SyncFlag.ReadOnly);
            }
        }

        public int GetTcpPort(int index = -1)
        {
            if (index < 0)
            {
                index = NodeApplication.Current?.MultipleLaunchIndex ?? 0;
            }

            int port = 0;
            int portMin = PortMin + index;

            switch (Mode)
            {
                case PortAllocationMode.Fixed:
                    port = Port;
                    break;
                case PortAllocationMode.RangedAuto:
                    if (portMin > PortMax)
                    {
                        return -1;
                    }
                    port = NetworkHelper.AllocateTcpPort(portMin, PortMax);
                    break;
                case PortAllocationMode.RangedRandom:
                    if (portMin > PortMax)
                    {
                        return -1;
                    }
                    port = NetworkHelper.AllocateTcpPortRandom(portMin, PortMax);
                    break;
                case PortAllocationMode.EnvironmentConfig:
                    string value = Environment.GetConfig(ConfigKey);
                    if (value == null)
                    {
                        Logs.LogError("Environment config is not set : " + ConfigKey);
                    }
                    else
                    {
                        if (!int.TryParse(value, out port))
                        {
                            Logs.LogError("Port value parse failed : " + value);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (port <= 0)
            {
                port = -1;
            }

            _cachedPort = port;
            return port;
        }
        public int GetUdpPort(int index = -1)
        {
            if (index < 0)
            {
                index = NodeApplication.Current?.MultipleLaunchIndex ?? 0;
            }

            int port = 0;
            int portMin = PortMin + index;

            switch (Mode)
            {
                case PortAllocationMode.Fixed:
                    port = Port;
                    break;
                case PortAllocationMode.RangedAuto:
                    if (portMin > PortMax)
                    {
                        return -1;
                    }
                    port = NetworkHelper.AllocateUdpPort(portMin, PortMax);
                    break;
                case PortAllocationMode.RangedRandom:
                    if (portMin > PortMax)
                    {
                        return -1;
                    }
                    port = NetworkHelper.AllocateUdpPortRandom(portMin, PortMax);
                    break;
                case PortAllocationMode.EnvironmentConfig:
                    string value = Environment.GetConfig(ConfigKey);
                    if (value == null)
                    {
                        Logs.LogError("Environment config is not set : " + ConfigKey);
                    }
                    else
                    {
                        if (!int.TryParse(value, out port))
                        {
                            Logs.LogError("Port value parse failed : " + value);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (port == 0)
            {
                port = -1;
            }

            _cachedPort = port;
            return port;
        }

        public override string ToString()
        {
            if (NodeApplication.Current == null)
            {
                switch (Mode)
                {
                    case PortAllocationMode.Fixed:
                        return Port.ToString();
                    case PortAllocationMode.RangedAuto:
                        return $"Auto:[{PortMin}-{PortMax}]";
                    case PortAllocationMode.RangedRandom:
                        return $"Random:[{PortMin}-{PortMax}]";
                    case PortAllocationMode.EnvironmentConfig:
                        return "Config:" + ConfigKey;
                    default:
                        return base.ToString();
                }
            }
            else
            {
                return _cachedPort.ToString();
            }
        }
    }
}
