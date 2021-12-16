using Suity.Collections;
using Suity.Engine;
using Suity.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules
{
    public class ServerSocketModule : Module
    {
        public ServerSocketModule()
            : base(ModuleBindingNames.TcpServerSocket, "Editor implementation for [TcpServerSocket]")
        {
        }

        protected override IModuleBinding Bind(ModuleConfig config)
        {
            return new ServerSocketBinding(config);
        }
    }
}
