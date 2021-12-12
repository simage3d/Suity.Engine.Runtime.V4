// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Suity.AutoDiscovery;
using Suity.Engine.LocalMessaging;
using Suity.Helpers;
using Suity.Reflecting;
using Suity.Synchonizing;
using Suity.Views;


namespace Suity.Engine.Debugging
{
    class DebugModuleProvider : BaseAutoDiscoveryModuleProvider
    {
        public static readonly bool LocalMessageQueue = true;

        readonly IDebugInstanceService _debugInstance;
        readonly Dictionary<string, Module> _loadedModules = new Dictionary<string, Module>();
        readonly List<Module> _moduleList = new List<Module>();

        readonly string _moduleDir;

        public DebugModuleProvider(IDebugInstanceService debugInstance, IServiceDiscoveryHost discoveryHost, string moduleDirectory)
            : base(discoveryHost)
        {
            _debugInstance = debugInstance ?? throw new ArgumentNullException(nameof(debugInstance));
            _moduleDir = moduleDirectory ?? throw new ArgumentNullException(nameof(moduleDirectory));
        }


        protected override Module ResolveBuildInModule(string moduleName)
        {
            switch (moduleName)
            {
                case ModuleBindingNames.MessageQueue:
                    if (LocalMessageQueue)
                    {
                        return LocalMessageModule.Instance;

                        //return EmptyMessageModule.Empty;
                        //return _debugInstance.BindBuildInModule(moduleName);
                    }
                    break;
                default:
                    break;
            }

            return base.ResolveBuildInModule(moduleName);
        }
        protected override string ResolveExternalModuleFile(string moduleName)
        {
            string assemblyPath = _debugInstance.ResolveModulePath(moduleName);
            if (assemblyPath == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(_moduleDir))
            {
                Logs.LogError("Module directory is not set.");
                return null;
            }

            string fullPath = Path.Combine(_moduleDir, assemblyPath);
            //添加到搜索路径
            Debugger.AddFileNameForProbePath(fullPath);

            return fullPath;
        }

    }
}
