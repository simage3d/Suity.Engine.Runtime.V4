using Suity.Engine;
using Suity.Networking;
using Suity.Networking.Client;
using Suity.Helpers;
using Suity.NodeQuery;
using Suity.Synchonizing;
using Suity.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules
{
    class ClientSocketBinding : TcpClient, IModuleBinding, IViewObject, IInfoNode
    {
        readonly ModuleConfig _config;
        readonly List<NetworkUpdaterFamily> _updaterFamilies = new List<NetworkUpdaterFamily>();

        public ClientSocketBinding(ModuleConfig input)
        {
            _config = input ?? throw new ArgumentNullException(nameof(input));

            var packetFormat = input.GetItem(NetworkConfigs.PacketFormat);
            bool compressed = input.GetItem(NetworkConfigs.Compressed, false);
            Formatter = PacketFormatter.CreatePacketFormatter(packetFormat, compressed);

            foreach (NetworkUpdaterFamily family in _config.GetItem(NetworkConfigs.UpdaterFamilies).OfType<NetworkUpdaterFamily>())
            {
                RegisterUpdaterFamily(family);
                _updaterFamilies.Add(family);
            }
        }

        #region IModuleBinding
        T IModuleBinding.GetServiceObject<T>()
        {
            return this as T;
        }
        #endregion

        #region IViewObject
        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            setup.AllTreeViewField(this);
        }

        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            sync.Sync("Updaters", _updaterFamilies, SyncFlag.ReadOnly);
        }
        #endregion

        #region IInfoNode
        void IInfoNode.WriteInfo(INodeWriter writer)
        {
            foreach (var item in _updaterFamilies.OfType<IInfoNode>())
            {
                item.WriteInfo(writer);
            }
        }
        #endregion
    }
}
