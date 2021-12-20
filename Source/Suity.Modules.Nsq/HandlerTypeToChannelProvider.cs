using NsqSharp.Bus.Configuration.Providers;
using Suity.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules.Nsq
{
    public class HandlerTypeToChannelProvider : IHandlerTypeToChannelProvider
    {
        private readonly string _domain;
        // every handler maps to a channel off a topic.
        // channels are independent listeners to the stream of messages sent to a topic.

        // a handler is an implementation of IHandleMessages<T>.

        private readonly Dictionary<Type, string> _handlerToChannel = new Dictionary<Type, string>();

        public HandlerTypeToChannelProvider(string domain, IEnumerable<NetworkCommand> commands)
        {
            _domain = domain ?? throw new ArgumentNullException(nameof(domain));

            foreach (NetworkCommand command in commands.OfType<NetworkCommand>())
            {
                if (command.RequestType == null)
                {
                    Logs.LogWarning($"Command request type == null : {command.GetType().Name}");
                    continue;
                }

                string handlerName = domain + "-" + command.Name.Replace('|', '-').Replace('/', '-') + "-handler";

                Type handlerType;

                if (command.ResultType != null)
                {
                    handlerType = typeof(SuityNsqHandler<,>).MakeGenericType(command.RequestType, command.ResultType);
                }
                else
                {
                    handlerType = typeof(SuityNsqHandler<>).MakeGenericType(command.RequestType);
                }
                
                _handlerToChannel.Add(handlerType, handlerName);
            }
        }

        public string GetChannel(Type handlerType)
        {
            return _handlerToChannel[handlerType];
        }

        public IEnumerable<Type> GetHandlerTypes()
        {
            return _handlerToChannel.Keys;
        }
    }
}
