// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Suity.Networking
{
    [AssetDefinitionType(AssetDefinitionCodes.Struct)]
    public abstract class NetworkUpdater : RuntimeObject, IExchangableObject
    {
        public abstract Type ResultType { get; }

        protected int _counter;

        public int Counter => _counter;


        public abstract void ExecuteUpdater(NetworkClient client, object request, INetworkInfo resultInfo);


        public virtual void ExchangeProperty(IExchange exchange)
        {
        }

    }

    public abstract class NetworkUpdater<T> : NetworkUpdater
    {
        public override Type ResultType => typeof(T);
        protected override string GetName()
        {
            return typeof(T).FullName;
        }

        public override void ExecuteUpdater(NetworkClient client, object request, INetworkInfo resultInfo)
        {
#if BRIDGE
            _counter++;
#else
            Interlocked.Increment(ref _counter);
#endif
            //Logs.AddNetworkLog(LogMessageType.Debug, NetworkDirection.None, "NetworkUpdater", this.GetType().Name, resultInfo.Body);
            T result = (T)resultInfo.Body;
            Execute(client, request, result, resultInfo);
        }

        protected abstract void Execute(NetworkClient client, object request, T result, INetworkInfo info);
    }
}
