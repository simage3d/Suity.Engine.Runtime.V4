using Suity.NodeQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Views
{
    public static class ViewPropertyExtensions
    {
        public static ViewProperty WithReadOnly(this ViewProperty viewProperty, int viewId)
        {
            viewProperty.ViewId = viewId;
            return viewProperty;
        }
        public static ViewProperty WithInspectorView(this ViewProperty viewProperty)
        {
            viewProperty.ViewId = ViewIds.Inspector;
            return viewProperty;
        }
        public static ViewProperty WithTreeView(this ViewProperty viewProperty)
        {
            viewProperty.ViewId = ViewIds.TreeView;
            return viewProperty;
        }
        public static ViewProperty WithMainTreeView(this ViewProperty viewProperty)
        {
            viewProperty.ViewId = ViewIds.MainTreeView;
            return viewProperty;
        }
        public static ViewProperty WithDetailTreeView(this ViewProperty viewProperty)
        {
            viewProperty.ViewId = ViewIds.DetailTreeView;
            return viewProperty;
        }

        public static ViewProperty WithReadOnly(this ViewProperty viewProperty)
        {
            viewProperty.ReadOnly = true;
            return viewProperty;
        }
        public static ViewProperty WithConfirm(this ViewProperty viewProperty, string message)
        {
            viewProperty.EnsureStyles().SetAttribute("Confirm", message);
            return viewProperty;
        }
        public static ViewProperty WithEmptyListGray(this ViewProperty viewProperty)
        {
            viewProperty.EnsureStyles().SetAttribute("EmptyListGray", true);
            return viewProperty;
        }
        public static ViewProperty WithHeaderStyle(this ViewProperty viewProperty, string style) 
        {
            viewProperty.EnsureStyles().SetAttribute("HeaderStyle", style);
            return viewProperty;
        }


        public static RawNode EnsureStyles(this ViewProperty viewProperty)
        {
            RawNode styles = viewProperty.Styles as RawNode;
            if (styles == null)
            {
                styles = new RawNode("Styles");
                viewProperty.Styles = styles;
            }

            return styles;
        }
        public static ViewProperty WithObsolete(this ViewProperty viewProperty)
        {
            viewProperty.EnsureStyles().SetAttribute("Obsolete", true);
            return viewProperty;
        }


        public static string GetConfirm(this INodeReader reader)
        {
            return reader.GetAttribute("Confirm");
        }
        public static bool GetEmptyListGray(this INodeReader reader)
        {
            return reader.GetBooleanAttribute("EmptyListGray");
        }
        public static string GetHeaderStyle(this INodeReader reader)
        {
            return reader.GetAttribute("HeaderStyle");
        }
    }
}
