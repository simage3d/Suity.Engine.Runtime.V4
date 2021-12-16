using NsqSharp.Bus.Configuration.Providers;
using Suity.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules.Nsq
{
    public class MessageTypeToTopicProvider : IMessageTypeToTopicProvider
    {
        private readonly string _domain;

        // every message type maps to a topic. sending this message type sends to this topic.
        private readonly Dictionary<Type, string> _messageToTopic = new Dictionary<Type, string>();

        public MessageTypeToTopicProvider(string domain, IEnumerable<NetworkCommandFamily> families)
        {
            _domain = domain ?? throw new ArgumentNullException(nameof(domain));

            foreach (NetworkCommandFamily family in families.OfType<NetworkCommandFamily>())
            {
                foreach (NetworkCommand command in family.Commands.OfType<NetworkCommand>())
                {
                    if (command.RequestType != null && !string.IsNullOrEmpty(command.FullName))
                    {
                        string topicName = _domain + "-" + command.FullName.Replace('|', '-').Replace('/', '-');

                        _messageToTopic.Add(command.RequestType, topicName);
                    }                    
                }
            }
        }

        public string GetTopic(Type messageType)
        {
            return _messageToTopic[messageType];
        }
    }
}
