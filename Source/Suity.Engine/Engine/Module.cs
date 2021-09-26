// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using Suity.Collections;
using Suity.Helpers;
using Suity.NodeQuery;
using Suity.Synchonizing;
using Suity.Synchonizing.Preset;
using Suity.Views;


namespace Suity.Engine
{
    public class ModuleBindingItem : Suity.Object, IViewObject, ITextDisplay, IPreviewDisplay, IInfoNode
    {
        public ModuleConfig Config { get; }
        public IModuleBinding Binding { get; }
        public ModuleBindingItem(ModuleConfig input, IModuleBinding binding)
        {
            Config = input ?? throw new ArgumentNullException(nameof(input));
            Binding = binding ?? throw new ArgumentNullException(nameof(binding));
        }

        #region ITextDisplay

        string ITextDisplay.Text => Config.Name;

        object ITextDisplay.Icon => "*CoreIcon|Binding";

        TextStatus ITextDisplay.TextStatus => TextStatus.Normal;

        #endregion

        #region IPreviewDisplay
        string IPreviewDisplay.PreviewText => Config.Owner?.GetType().FullName;

        object IPreviewDisplay.PreviewIcon => null;

        #endregion

        #region IVisionTreeObject
        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            (Binding as IViewObject)?.SetupView(setup);
        }

        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            (Binding as ISyncObject)?.Sync(sync, context);
        } 
        #endregion

        #region IInfoWriter
        void IInfoNode.WriteInfo(INodeWriter writer)
        {
            writer.WriteInfo("Binding", Config.Name, null, null, TextStatus.Normal, "*CoreIcon|Binding", w =>
            {
                (Binding as IInfoNode)?.WriteInfo(w);
            });
        }
        #endregion
    }

    public abstract class Module : Suity.Object, IViewNode, ITextDisplay, IPreviewDisplay, IInfoNode
    {

        public string Implement { get; }
        public string ModuleInfo { get; }

        readonly Dictionary<ModuleConfig, ModuleBindingItem> _bindings = new Dictionary<ModuleConfig, ModuleBindingItem>();
        readonly ReadonlySyncList<ModuleBindingItem> _list = new ReadonlySyncList<ModuleBindingItem>();
        //readonly object _sync = new object();

        protected Module(string implement, string moduleInfo)
        {
            if (string.IsNullOrEmpty(implement))
            {
                throw new ArgumentException("implement is empty", nameof(implement));
            }
            Implement = implement;
            ModuleInfo = moduleInfo;
        }

        protected override string GetName()
        {
            return ModuleInfo;
        }

        #region Binding
        public IModuleBinding GetBinding(ModuleConfig input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            lock (this)
            {
                ModuleBindingItem item = _bindings.GetValueOrDefault(input);
                if (item != null)
                {
                    return item.Binding;
                }

                IModuleBinding binding = this.Bind(input);
                if (binding != null)
                {
                    item = new ModuleBindingItem(input, binding);
                    _bindings.Add(input, item);
                    _list.Add(item);

                    return item.Binding;
                }
            }

            return null;
        }

        protected abstract IModuleBinding Bind(ModuleConfig input);

        //protected void RemoveBinding(ModuleConfig config)
        //{
        //    if (config == null)
        //    {
        //        return;
        //    }

        //    lock (this)
        //    {
        //        var item = _bindings.GetValueOrDefault(config);
        //        if (item != null)
        //        {
        //            _bindings.Remove(config);
        //            _list.Remove(item);
        //        }
        //    }
        //}

        public IEnumerable<ModuleBindingItem> Bindings => _bindings.Values.Select(o => o);

        #endregion

        #region AliveCheck
        public virtual void AliveCheck()
        {
        }

        #endregion

        #region ITextDisplay

        string ITextDisplay.Text => Implement;

        object ITextDisplay.Icon => "*CoreIcon|Module";

        TextStatus ITextDisplay.TextStatus => TextStatus.Normal;

        #endregion

        #region IPreviewDisplay

        string IPreviewDisplay.PreviewText => $"\"{ModuleInfo}\"";

        object IPreviewDisplay.PreviewIcon => null;

        #endregion

        #region IVisionTreeNode

        int IViewNode.ListViewId => ViewIds.TreeView;

        ISyncList ISyncNode.GetList() => _list;

        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
        }

        bool IDropInCheck.CanDropIn(object value) => false;

        object IDropInCheck.DropInConvert(object value)
        {
            return value;
        }
        #endregion

        #region IInfoWriter
        void IInfoNode.WriteInfo(INodeWriter writer)
        {
            writer.WriteInfo("Module", Implement, ModuleInfo, null, TextStatus.Normal, "*CoreIcon|Module", w =>
            {
                foreach (var binding in _list.List.OfType<IInfoNode>())
                {
                    binding.WriteInfo(w);
                }
            });
        }

        #endregion
    }
}
