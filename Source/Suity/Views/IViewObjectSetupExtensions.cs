// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using Suity.Collections;
using Suity.Synchonizing;

namespace Suity.Views
{
    public static class IViewObjectSetupExtensions
    {
        public static bool SupportInspector(this IViewObjectSetup setup)
        {
            return setup.IsViewIdSupported(ViewIds.Inspector);
        }
        public static void InspectorFieldOf<T>(this IViewObjectSetup setup, ViewProperty property)
        {
            property.ViewId = ViewIds.Inspector;
            setup.AddField(typeof(T), property);
        }
        public static void InspectorField<T>(this IViewObjectSetup setup, T value, ViewProperty property)
        {
            property.ViewId = ViewIds.Inspector;
            Type type = value != null ? value.GetType() : typeof(T);
            setup.AddField(type, property);
        }
        public static void AllInspectorField(this IViewObjectSetup setup, ISyncObject obj, bool expand = false, bool readOnly = false)
        {
            ViewObjectSetupAllFieldSync sync = new ViewObjectSetupAllFieldSync(setup, null, expand, readOnly, ViewIds.Inspector);
            obj.Sync(sync, EmptySyncContext.Empty);
        }
        public static void AllInspectorField(this IViewObjectSetup setup, ISyncObject obj, Predicate<string> predicate, bool expand = false, bool readOnly = false)
        {
            ViewObjectSetupAllFieldSync sync = new ViewObjectSetupAllFieldSync(setup, predicate, expand, readOnly, ViewIds.Inspector);
            obj.Sync(sync, EmptySyncContext.Empty);
        }

        public static bool SupportTreeView(this IViewObjectSetup setup)
        {
            return setup.IsViewIdSupported(ViewIds.TreeView);
        }
        public static void TreeViewFieldOf<T>(this IViewObjectSetup setup, ViewProperty property)
        {
            property.ViewId = ViewIds.TreeView;
            setup.AddField(typeof(T), property);
        }
        public static void TreeViewField<T>(this IViewObjectSetup setup, T value, ViewProperty property)
        {
            property.ViewId = ViewIds.TreeView;
            Type type = value != null ? value.GetType() : typeof(T);
            setup.AddField(type, property);
        }
        public static void AllTreeViewField(this IViewObjectSetup setup, ISyncObject obj, bool expand = false, bool readOnly = false)
        {
            ViewObjectSetupAllFieldSync sync = new ViewObjectSetupAllFieldSync(setup, null, expand, readOnly, ViewIds.TreeView);
            obj.Sync(sync, EmptySyncContext.Empty);
        }
        public static void AllTreeViewField(this IViewObjectSetup setup, ISyncObject obj, Predicate<string> predicate, bool expand = false, bool readOnly = false)
        {
            ViewObjectSetupAllFieldSync sync = new ViewObjectSetupAllFieldSync(setup, predicate, expand, readOnly, ViewIds.TreeView);
            obj.Sync(sync, EmptySyncContext.Empty);
        }

        public static bool SupportMainTreeView(this IViewObjectSetup setup)
        {
            return setup.IsViewIdSupported(ViewIds.MainTreeView);
        }
        public static void MainTreeViewFieldOf<T>(this IViewObjectSetup setup, ViewProperty property)
        {
            property.ViewId = ViewIds.MainTreeView;
            setup.AddField(typeof(T), property);
        }
        public static void MainTreeViewField<T>(this IViewObjectSetup setup, T value, ViewProperty property)
        {
            property.ViewId = ViewIds.MainTreeView;
            Type type = value != null ? value.GetType() : typeof(T);
            setup.AddField(type, property);
        }
        public static void AllMainTreeViewField(this IViewObjectSetup setup, ISyncObject obj, bool expand = false, bool readOnly = false)
        {
            ViewObjectSetupAllFieldSync sync = new ViewObjectSetupAllFieldSync(setup, null, expand, readOnly, ViewIds.MainTreeView);
            obj.Sync(sync, EmptySyncContext.Empty);
        }
        public static void AllMainTreeViewField(this IViewObjectSetup setup, ISyncObject obj, Predicate<string> predicate, bool expand = false, bool readOnly = false)
        {
            ViewObjectSetupAllFieldSync sync = new ViewObjectSetupAllFieldSync(setup, predicate, expand, readOnly, ViewIds.MainTreeView);
            obj.Sync(sync, EmptySyncContext.Empty);
        }

        public static bool SupportDetailTreeView(this IViewObjectSetup setup)
        {
            return setup.IsViewIdSupported(ViewIds.DetailTreeView);
        }
        public static void DetailTreeViewFieldOf<T>(this IViewObjectSetup setup, ViewProperty property)
        {
            property.ViewId = ViewIds.DetailTreeView;
            setup.AddField(typeof(T), property);
        }
        public static void DetailTreeViewField<T>(this IViewObjectSetup setup, T value, ViewProperty property)
        {
            property.ViewId = ViewIds.DetailTreeView;
            Type type = value != null ? value.GetType() : typeof(T);
            setup.AddField(type, property);
        }
        public static void AllDetailTreeViewField(this IViewObjectSetup setup, ISyncObject obj, bool expand = false, bool readOnly = false)
        {
            ViewObjectSetupAllFieldSync sync = new ViewObjectSetupAllFieldSync(setup, null, expand, readOnly, ViewIds.DetailTreeView);
            obj.Sync(sync, EmptySyncContext.Empty);
        }
        public static void AllDetailTreeViewField(this IViewObjectSetup setup, ISyncObject obj, Predicate<string> predicate, bool expand = false, bool readOnly = false)
        {
            ViewObjectSetupAllFieldSync sync = new ViewObjectSetupAllFieldSync(setup, predicate, expand, readOnly, ViewIds.DetailTreeView);
            obj.Sync(sync, EmptySyncContext.Empty);
        }

        public static void Label(this IViewObjectSetup setup, string text)
        {
            Label(setup, text, text);
        }
        public static void Label(this IViewObjectSetup setup, string name, string text)
        {
            setup.InspectorField(LabelValue.Empty, new ViewProperty(name, text));
        }
        public static void Label(this IViewObjectSetup setup, ViewProperty property)
        {
            setup.InspectorField(LabelValue.Empty, property);
        }
        
        #region ViewObjectSetupAllFieldSync
        class ViewObjectSetupAllFieldSync : IPropertySync
        {
            readonly IViewObjectSetup _setup;
            Predicate<string> _predicate;
            readonly bool _expand;
            readonly bool _readOnly;
            readonly int _viewId;

            public ViewObjectSetupAllFieldSync(IViewObjectSetup setup, Predicate<string> predicate, bool expand, bool readOnly, int viewId)
            {
                _setup = setup;
                _predicate = predicate;
                _expand = expand;
                _readOnly = readOnly;
                _viewId = viewId;
            }

            public SyncMode Mode => SyncMode.GetAll;

            public SyncIntent Intent => SyncIntent.View;

            public string Name => null;

            public IEnumerable<string> Names => EmptyArray<string>.Empty;

            public object Value => null;

            public T Sync<T>(string name, T obj, SyncFlag flag = SyncFlag.None, T defaultValue = default(T))
            {
                if (_predicate == null || _predicate(name))
                {
                    _setup.AddField(typeof(T), new ViewProperty(name) { Expand = _expand, ReadOnly = _readOnly, ViewId = _viewId });
                }
                return obj;
            }
        }

        #endregion
    }
}
