using Suity.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules
{
    public class NettyModule : Module
    {
        public NettyModule()
            : base(ModuleBindingNames.TcpServerSocket, "DotNetty Tcp Server Socket")
        {
        }

        protected override IModuleBinding Bind(ModuleConfig input)
        {
            return new NettyBinding(input);
        }
    }
}
