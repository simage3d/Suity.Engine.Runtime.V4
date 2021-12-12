// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Engine.Debugging
{
    public interface IDebugHostService
    {
        NodeStartInfo GetStartInfo(string galaxyFile, string nodeName);

        IDebugInstanceService CreateDebugInstance(NodeStartInfo nodeStartInfo, IDebugNode debugNode);
    }

    public interface IDebugInstanceService : IServiceProvider
    {
        string Say(string str);

        void NodeAlive(string galaxyFile);
        void NodeLeaving(string galaxyFile);
        string ResolveModulePath(string moduleName);

        void AddServiceLocation(DebugServiceLocation location);
        void RemoveServiceLocation(DebugServiceLocation location);
        void RequestAutoDiscovery(string serviceCode);
        void CancelAutoDiscovery(string serviceCode);

        DebugServiceLocation[] GetGlobalServiceLocations();
    }

    public interface IDebugNode
    {
        string NodeId { get; }

        DebugServiceLocation[] GetServiceLocations();

        void AddServiceDiscovery(DebugServiceLocation location);

        void RemoveServiceDiscovery(DebugServiceLocation location);
    }
}
