// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Helpers;
using Suity.NodeQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    [AssetDefinitionType(AssetDefinitionCodes.TypeLibrary)]
    public class NetworkCommandFamily : Suity.Object, IInfoNode
    {
        readonly string _key;
        readonly string _name;
        readonly HashSet<NetworkCommand> _commands = new HashSet<NetworkCommand>();
        readonly HashSet<string> _versions = new HashSet<string>();

        public NetworkCommandFamily()
        {
            _key = _name = this.GetType().FullName;
        }
        public NetworkCommandFamily(string key)
        {
            _key = key;
            _name = this.GetType().FullName;
        }
        public NetworkCommandFamily(string key, string name)
        {
            _key = key;
            _name = name;
        }

        protected override string GetName()
        {
            return _name;
        }

        protected void RegisterCommand(NetworkCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException();
            }
            _commands.Add(command);
        }

        protected void RegisterVersion(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                version = "0";
            }

            _versions.Add(version);
        }

        public IEnumerable<NetworkCommand> Commands => _commands.Select(o => o);

        public IEnumerable<string> Versions => _versions.Count > 0 ? _versions.Select(o => o) : new string[] { "0" };

        //#region ITextDisplay
        //string ITextDisplay.Text => _name;

        //object ITextDisplay.Icon => "*CoreIcon|Package";

        //TextStatus ITextDisplay.TextStatus => TextStatus.Normal;
        //#endregion

        //#region IVisionTreeObject
        //void IVisionTreeObject.SetupVisionTree(IViewObjectSetup setup)
        //{
        //    setup.AllField(this);
        //}

        //void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        //{
        //    foreach (NetworkCommand command in _commands)
        //    {
        //        sync.Sync(command.Method + command.Name, command, SyncFlag.ReadOnly);
        //    }
        //}
        //#endregion

        void IInfoNode.WriteInfo(INodeWriter writer)
        {
            writer.WriteInfo("NetworkCommandFamily", _key, null, null, TextStatus.Normal, "*CoreIcon|Family", w => 
            {
                foreach (var item in _commands.OfType<IInfoNode>())
                {
                    item.WriteInfo(w);
                }
            });
        }

        public override string ToString()
        {
            return _name ?? base.ToString();
        }

    }
}
