// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Net;
using Suity.Collections;
using Suity.Engine;
using Suity.Networking;
using Suity.Helpers;
using Suity.NodeQuery;
using Suity.Json;

namespace Suity.Engine.LocalMessaging
{
    public class LocalMessageService : NetworkSession
    {
        readonly LocalMessageBus _bus;
        readonly UniqueMultiDictionary<string, LocalMessageHandler> _handlers = new UniqueMultiDictionary<string, LocalMessageHandler>();

        public LocalMessageBus Bus => _bus;

        public LocalMessageService(LocalMessageBus bus)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public void RegisterCommand(NetworkCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            LocalMessageHandler handler = new LocalMessageHandler(this, command);
            _handlers.Add(handler.TypeName, handler);
            _bus.Subscribe(handler);
        }

        public void Send(object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }

            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Upload, ModuleBindingNames.MessageQueue, string.Empty, data);

            string typeName = ObjectType.GetClassTypeInfo(data.GetType())?.Key;
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentException("T must registered in ObjectType.");
            }

            //string str = JsonConvert.SerializeObject(data);
            //string str = ComputerBeacon.Json.Serializer.Serialize(data);
            //XmlNodeWriter writer = new XmlNodeWriter("msg");
            //Synchonizing.Core.Serializer.Serialize(data, writer);

            JsonDataWriter writer = new JsonDataWriter();
            ObjectType.Write(data.GetType(), writer, data);

            _bus.Send(typeName, writer.ToString());
        }


        #region NetworkSession
        public override NetworkServer Server => null;

        public override IPEndPoint RemoteEndPoint => null;

        public override IPEndPoint LocalEndPoint => null;

        public override bool Connected => true;

        public override string SessionId => string.Empty;

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
            Send(data);
        }

        #endregion
    }
}
