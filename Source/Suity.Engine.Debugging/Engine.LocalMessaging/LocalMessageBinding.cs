// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Engine;
using Suity.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Engine.LocalMessaging
{
    public class LocalMessageBinding : MessageQueue, IModuleBinding
    {
        readonly ModuleConfig _config;
        readonly Dictionary<Type, NetworkCommand> _commands = new Dictionary<Type, NetworkCommand>();
        readonly LocalMessageService _messageService;
        

        public LocalMessageBinding(ModuleConfig config, LocalMessageBus bus)
        {
            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            _config = config ?? throw new ArgumentNullException(nameof(config));
            _messageService = new LocalMessageService(bus);

            IEnumerable<NetworkCommand> commands = config.GetItem(NetworkConfigs.CommandHandlers);
            if (commands != null)
            {
                foreach (NetworkCommand command in commands.OfType<NetworkCommand>())
                {
                    _commands[command.RequestType] = command;
                    _messageService.RegisterCommand(command);
                }
            }
        }
        protected override void Destroy()
        {
            Dispose();
        }
        public void Dispose()
        {
        }

        public T GetServiceObject<T>() where T : class
        {
            return this as T;
        }

        public override void Send<T>(T message)
        {
            _messageService.Send(message);
        }
    }
}
