using Suity.Engine;
using Suity.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules
{
    public class ClientSocketModule : Module
    {
        public ClientSocketModule()
            : base(ModuleBindingNames.TcpClientSocket, "Editor implementation for [TcpClientSocket]")
        {
        }


        protected override IModuleBinding Bind(ModuleConfig config)
        {
            return new ClientSocketBinding(config);
        }
    }
}
