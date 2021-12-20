using System;
using System.Collections.Generic;
using System.Net;
using NsqSharp.Bus;
using Suity.Engine;
using Suity.Networking;
using Suity.Helpers;

namespace Suity.Modules.Nsq
{
    class SuityNsqSession : NetworkSession
    {
        readonly IBus _bus;

        public SuityNsqSession(IBus bus)
        {
            _bus = bus;
        }

        public override NetworkServer Server => null;

        public override IPEndPoint RemoteEndPoint => null;

        public override IPEndPoint LocalEndPoint => null;

        public override bool Connected => true;

        public override string SessionId => null;

        public override KeepAliveModes KeepAlive => KeepAliveModes.LongTerm;

        public override void Close()
        {
        }


        public override T GetService<T>()
        {
            return Suity.Environment.GetService<T>();
        }

        public override void Send(object data, NetworkDeliveryMethods method, int channel)
        {
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Upload, ModuleBindingNames.MessageQueue, string.Empty, data);
            try
            {
                _bus.Send(data);
            }
            catch (Exception err)
            {
                Logs.LogError(err);
            }
        }

    }
}
