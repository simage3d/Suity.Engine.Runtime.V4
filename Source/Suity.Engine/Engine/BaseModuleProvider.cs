// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
#if !BRIDGE
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Suity.Collections;
using Suity.Helpers;
using Suity.NodeQuery;
using Suity.Synchonizing;
using Suity.Views;


namespace Suity.Engine
{
    public abstract class BaseModuleProvider : Suity.Object, IModuleProvider, IViewList, ITextDisplay, IInfoNode
    {
        readonly Dictionary<string, Module> _loadedModules = new Dictionary<string, Module>();
        readonly List<Module> _moduleList = new List<Module>();


        public BaseModuleProvider()
        {
        }
        protected override void Destroy()
        {
            Stop();
        }

        public virtual void AliveCheck()
        {
            foreach (var module in _moduleList)
            {
                module.AliveCheck();
            }
        }

        public void Stop()
        {
            foreach (var module in _moduleList)
            {
                foreach (ModuleBindingItem item in module.Bindings)
                {
                    item.Binding.Dispose();
                }
            }
        }

        #region IModuleProvider

        public T Bind<T>(string moduleName, ModuleConfig config) where T : class
        {
            Logs.LogInfo($"Resolving module {moduleName}...");

            Module module = GetModule(moduleName);
            if (module == null)
            {
                Logs.LogError($"Can not resolve module : {moduleName}");
                return null;
            }

            IModuleBinding binding = null;
            try
            {
                binding = module.GetBinding(config);
            }
            catch (Exception err)
            {
                Logs.LogError(new ModuleException($"Module {moduleName} bind failed, can not resolve type {typeof(T).Name}", err));
                return null;
            }

            if (binding == null)
            {
                Logs.LogError($"{module.GetType().FullName} can not resolve type : {typeof(T).Name}");
                return null;
            }

            OnBound(binding, config);
            return binding.GetServiceObject<T>();
        }
        public Module GetModule(string moduleName)
        {
            Module module = _loadedModules.GetValueOrDefault(moduleName);
            if (module != null)
            {
                return module;
            }

            if (module == null)
            {
                module = ResolveBuildInModule(moduleName);
            }
            if (module == null)
            {
                module = ResolveExternalModule(moduleName);
            }
            if (module == null)
            {
                module = ResolveInternalDefaultModule(moduleName);
                if (module != null)
                {
                    _loadedModules[moduleName] = module;
                    _moduleList.Add(module);
                }
            }

            if (module != null)
            {
                Logs.LogInfo($"Module resolved for {moduleName} : {module.GetType().Name}");
            }
            else
            {
                Logs.LogError($"Module resolved failed : {moduleName}");
            }

            return module;
        }


        protected virtual Module ResolveBuildInModule(string moduleName)
        {
            return null;
        }
        private Module ResolveExternalModule(string moduleName)
        {
            Logs.LogInfo($"Resolved external module for {moduleName}...");

            string fullPath = ResolveExternalModuleFile(moduleName);
            
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }

            if (!File.Exists(fullPath))
            {
                Logs.LogError($"Module [{moduleName}] file not found : {fullPath}.");
                return null;
            }
            try
            {
                Assembly assembly = Assembly.LoadFile(fullPath);
                foreach (var type in typeof(Module).GetDerivedTypes(assembly))
                {
                    Module newModule = null;
                    try
                    {
                        newModule = (Module)Activator.CreateInstance(type);
                    }
                    catch (Exception err)
                    {
                        Logs.LogError(new ModuleException($"Create module {type.Name} failed, default construct not found.", err));
                        continue;
                    }

                    if (newModule == null || string.IsNullOrEmpty(newModule.Implement))
                    {
                        continue;
                    }
                    if (_loadedModules.ContainsKey(newModule.Implement))
                    {
                        continue;
                    }
                    _loadedModules.Add(newModule.Implement, newModule);
                    _moduleList.Add(newModule);
                }

                Module module = _loadedModules.GetValueOrDefault(moduleName);
                if (module != null)
                {
                    return module;
                }
            }
            catch (Exception e)
            {
                Logs.LogError(new ModuleException("Module load failed : " + moduleName, e));
            }

            return null;
        }
        protected virtual Module ResolveInternalDefaultModule(string moduleName)
        {
            return null;
        }


        protected virtual void OnBound(IModuleBinding binding, ModuleConfig config)
        {
        }

        protected abstract string ResolveExternalModuleFile(string moduleName);

        #endregion

        #region IViewList
        int IViewList.ListViewId => ViewIds.TreeView;

        int ISyncList.Count => _loadedModules.Count;

        void ISyncList.Sync(IIndexSync sync, ISyncContext context)
        {
            if (sync.IsGetter())
            {
                sync.SyncGenericIList(_moduleList, typeof(Engine.Module));
            }
        }

        bool IDropInCheck.CanDropIn(object value)
        {
            return false;
        }

        object IDropInCheck.DropInConvert(object value)
        {
            return value;
        }

        #endregion

        #region IInfoGetter
        public void WriteInfo(INodeWriter writer)
        {
            writer.WriteInfo("Modules", "Modules", null, null, TextStatus.Normal, "*CoreIcon|Module", w =>
            {
                foreach (var module in _moduleList.OfType<IInfoNode>())
                {
                    module.WriteInfo(w);
                }
            });
        }

        #endregion

        #region ITextDisplay
        string ITextDisplay.Text => "Modules";

        object ITextDisplay.Icon => "*CoreIcon|Module";

        TextStatus ITextDisplay.TextStatus => TextStatus.Normal;

        #endregion
    }
}
#endif