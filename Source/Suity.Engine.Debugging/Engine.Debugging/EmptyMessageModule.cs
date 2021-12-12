// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Engine;
using Suity.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Engine.Debugging
{
    class EmptyMessageModule : Module
    {
        public static readonly EmptyMessageModule Empty = new EmptyMessageModule();

        private EmptyMessageModule() : base(ModuleBindingNames.MessageQueue, "Empty MessageQueue")
        {
        }

        protected override IModuleBinding Bind(ModuleConfig input)
        {
            return new EmptyMessageModuleBinding(input);
        }
    }

    class EmptyMessageModuleBinding : MessageQueue, IModuleBinding
    {
        readonly ModuleConfig _config;

        public EmptyMessageModuleBinding(ModuleConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
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
            Logs.AddNetworkLog(LogMessageType.Warning, NetworkDirection.Upload, "NoImplementation", string.Empty, message);
        }
    }
}
