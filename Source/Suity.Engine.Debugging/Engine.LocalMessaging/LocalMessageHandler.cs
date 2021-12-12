// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Engine;
using Suity.Networking;
using Suity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suity.NodeQuery;
using Suity.Json;

namespace Suity.Engine.LocalMessaging
{
    public class LocalMessageHandler : MarshalByRefObject
    {
        readonly LocalMessageService _service;
        readonly NetworkCommand _command;
        readonly string _typeName;

        public LocalMessageHandler(LocalMessageService service, NetworkCommand command)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _typeName = ObjectType.GetClassTypeInfo(command.RequestType)?.Key;
        }


        public string TypeName => _typeName;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void HandleMessage(string message)
        {
            Task.Run(() =>
            {
                //object obj = JsonConvert.DeserializeObject(message, _command.RequestType);
                //object obj = ComputerBeacon.Json.Serializer.Deserialize(_command.RequestType, message);

                //var reader = XmlNodeReader.FromXml(message);
                //var obj = Synchonizing.Core.Serializer.Deserialize(reader, _command.RequestType);

                JsonDataReader reader = new JsonDataReader(message);
                var obj = ObjectType.Read(_command.RequestType, reader);

                Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Download, ModuleBindingNames.MessageQueue, string.Empty, obj);

                MsgInfo info = new MsgInfo(_typeName, obj);
                var result = _command.ExecuteCommand(_service, info);
                if (result != null && !(result is EmptyResult))
                {
                    _service.Send(result);
                }
            });
        }

        class MsgInfo : INetworkInfo
        {
            readonly string _typeName;
            readonly object _message;

            public MsgInfo(string typeName, object message)
            {
                _typeName = typeName;
                _message = message;
            }

            public NetworkDeliveryMethods Method => NetworkDeliveryMethods.Default;

            public int Channel => 0;

            public string Key => _typeName;

            public object Body => _message;

            public object GetArgs(string name)
            {
                return null;
            }
        }
    }
}
