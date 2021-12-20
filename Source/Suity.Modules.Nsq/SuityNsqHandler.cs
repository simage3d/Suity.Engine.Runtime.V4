using NsqSharp.Bus;
using Suity.Engine;
using Suity.Networking;
using Suity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules.Nsq
{
    class SuityNsqHandler<TRequest> : IHandleMessages<TRequest>
    {
        readonly SuityNsqBinding _binding;
        readonly NetworkSession _session;
        readonly NetworkCommand _command;

        public SuityNsqHandler(SuityNsqBinding binding, SuityNsqSession session)
        {
            _binding = binding;
            _session = session;
            _command = _binding.GetCommand<TRequest>();
        }

        public void Handle(TRequest message)
        {
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Download, ModuleBindingNames.MessageQueue, string.Empty, message);
            _command.ExecuteCommand(_session, new MsgInfo(message));
        }

        class MsgInfo : INetworkInfo
        {
            readonly TRequest _message;


            public MsgInfo(TRequest message)
            {
                _message = message;
            }

            public NetworkDeliveryMethods Method => NetworkDeliveryMethods.Default;

            public int Channel => 0;

            public string Key => ObjectType.GetClassTypeInfo(typeof(TRequest))?.Name;

            public object Body => _message;

            public object GetArgs(string name)
            {
                return null;
            }
        }
    }

    class SuityNsqHandler<TRequest, TResult> : IHandleMessages<TRequest>
    {
        readonly SuityNsqBinding _binding;
        readonly NetworkSession _session;
        readonly NetworkCommand _command;

        public SuityNsqHandler(SuityNsqBinding binding, SuityNsqSession session)
        {
            _binding = binding;
            _session = session;
            _command = _binding.GetCommand<TRequest>();
        }

        public void Handle(TRequest message)
        {
            Logs.AddNetworkLog(LogMessageType.Info, NetworkDirection.Download, ModuleBindingNames.MessageQueue, string.Empty, message);
            var result = _command.ExecuteCommand(_session, new MsgInfo(message));
            if (result != null && result is TResult && !(result is EmptyResult))
            {
                _session.Send((TResult)result, NetworkDeliveryMethods.Default, 0);
            }
        }

        class MsgInfo : INetworkInfo
        {
            readonly TRequest _message;


            public MsgInfo(TRequest message)
            {
                _message = message;
            }

            public NetworkDeliveryMethods Method => NetworkDeliveryMethods.Default;

            public int Channel => 0;

            public string Key => ObjectType.GetClassTypeInfo(typeof(TRequest))?.Name;

            public object Body => _message;

            public object GetArgs(string name)
            {
                return null;
            }
        }
    }
}
