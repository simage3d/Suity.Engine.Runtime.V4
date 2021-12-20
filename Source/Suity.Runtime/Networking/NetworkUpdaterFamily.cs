// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.NodeQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    [AssetDefinitionType(AssetDefinitionCodes.TypeLibrary)]
    public class NetworkUpdaterFamily : RuntimeObject, IExchangableObject
    {
        readonly string _key;
        readonly string _name;
        readonly string _version;
        readonly Dictionary<string, NetworkUpdater> _updaters = new Dictionary<string, NetworkUpdater>();

        public NetworkUpdaterFamily()
        {
            _key = _name = this.GetType().FullName;
            _version = "0";
        }
        public NetworkUpdaterFamily(string key)
        {
            _key = key;
            _name = this.GetType().FullName;
            _version = "0";
        }
        public NetworkUpdaterFamily(string key, string name)
        {
            _key = key;
            _name = name;
            _version = "0";
        }
        public NetworkUpdaterFamily(string key, string name, string version)
        {
            _key = key;
            _name = name;
            _version = version;
        }

        public string Version => _version;

        protected override string GetName()
        {
            return _name;
        }

        protected void RegisterUpdater(NetworkUpdater updater)
        {
            if (updater == null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            _updaters.Add(updater.Name, updater);
        }

        public void ExecuteUpdater(NetworkClient client, object request, INetworkInfo resultInfo)
        {
            if (_updaters.TryGetValue(resultInfo.Key, out NetworkUpdater command))
            {
                command.ExecuteUpdater(client, request, resultInfo);
            }
        }

        public IEnumerable<NetworkUpdater> Updaters => _updaters.Values.Select(o => o);

        public void ExchangeProperty(IExchange exchange)
        {
            foreach (var pair in _updaters)
            {
                exchange.Exchange(pair.Key, pair.Value);
            }
        }

        //void IInfoNode.WriteInfo(INodeWriter writer)
        //{
        //    writer.WriteInfo("NetworkUpdaterFamily", _key, null, null, TextStatus.Normal, "*CoreIcon|Family", w =>
        //    {
        //        foreach (var item in _updaters.OfType<IInfoNode>())
        //        {
        //            item.WriteInfo(w);
        //        }
        //    });
        //}

        public override string  ToString()
        {
            return _name ?? base.ToString();
        }


    }
}
