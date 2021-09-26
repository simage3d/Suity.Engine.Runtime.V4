// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using Suity.NodeQuery;

namespace Suity.Engine
{
    [Serializable]
    public class GalaxyStartInfo
    {
        public readonly List<NodeStartInfo> Nodes = new List<NodeStartInfo>();
    }
    [Serializable]
    public class NodeStartInfo
    {
        public string NodeId;
        public string Description;
        public int MultipleLaunchIndex;

        public string GalaxyName;
        public string GalaxyId;
        public string GalaxyVersion;
        public string ApplicationName;
        public string ApplicationId;
        public string DataId;
        public string DataVersion;

        public string WorkSpaceName;
        public string AppDirectory;
        public string PackageDirectory;
        public string ModuleDirectory;

        public readonly List<string> DataInputs = new List<string>();
        public readonly List<NodeComponentStartInfo> Components = new List<NodeComponentStartInfo>();
        public readonly List<EnvironmentConfigInfo> Configs = new List<EnvironmentConfigInfo>();

        public NodeStartInfo()
        {
        }

        public override string ToString()
        {
            return NodeId ?? ApplicationName ?? base.ToString();
        }
    }

    [Serializable]
    public class NodeComponentStartInfo
    {
        public string FullTypeName;
        public string ComponentName;
        public INodeReader ComponentData;

        public override string ToString()
        {
            return FullTypeName ?? base.ToString();
        }
    }

    [Serializable]
    public class EnvironmentConfigInfo
    {
        public string Name;
        public string Value;
        public bool Override;
    }
}
