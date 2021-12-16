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
        readonly DateTime _startTime;

        public SuityNsqSession(IBus bus)
        {
            _bus = bus;
            _startTime = DateTime.UtcNow;
        }

        public override NetworkServer Server => null;

        public override IPEndPoint RemoteEndPoint => null;

        public override IPEndPoint LocalEndPoint => null;

        public override bool Connected => true;

        public override string SessionId => null;

        public override DateTime StartTime => _startTime;

        public override NetworkUser User { get; set; }

        public override void Close()
        {
        }

        public override DateTime GetLastIncomingTime()
        {
            return _startTime;
        }

        public override DateTime GetLastIncomingTime(int channel)
        {
            return _startTime;
        }


        public override T GetService<T>()
        {
            return Suity.Environment.GetService<T>();
        }

        public override void Send(object data, NetworkDeliveryMethods method, int channel)
        {
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Upload, ModuleBindingNames.MessageQueue, string.Empty, data);
            _bus.Send(data);
        }

    }
}
