using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Config;

namespace Networking.Server
{
    public interface ISocketConfigProvider
    {
        ServerConfig GetConfig();
    }
}
