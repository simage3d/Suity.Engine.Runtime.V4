// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Engine;
using Suity.Engine.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Engine.LocalMessaging
{
    public class LocalMessageModule : Module
    {
        public static readonly LocalMessageModule Instance = new LocalMessageModule();

        public LocalMessageModule() : base(ModuleBindingNames.MessageQueue, "Local implementation for [MessageQueue]")
        {
        }

        protected override IModuleBinding Bind(ModuleConfig input)
        {
            LocalMessageBus bus = Suity.Environment.GetService<LocalMessageBus>();
            if (bus == null)
            {
                Logs.LogWarning("LocalMessageBus not found.");
                return new EmptyMessageModuleBinding(input);
            }

            return new LocalMessageBinding(input, bus);
        }
    }

    
}
