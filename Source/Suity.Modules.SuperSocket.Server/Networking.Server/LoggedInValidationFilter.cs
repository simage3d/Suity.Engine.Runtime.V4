using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suity.Engine;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Metadata;

namespace Suity.Networking.Server
{
    public class LoggedInValidationFilter : CommandFilterAttribute
    {
        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {
            var session = commandContext.Session as SsAppSession;

            if (session == null || session.User == null)
            {
                commandContext.Cancel = true;
                SsRequestInfo requestInfo = commandContext.RequestInfo as SsRequestInfo;
                if (requestInfo != null && requestInfo.Channel > 0)
                {
                    session.Send(new ErrorResult
                    {
                        StatusCode = StatusCodes.NotLoggedIn.ToString(),
                        Location = NodeApplication.Current?.ServiceId,
                    }, requestInfo.Method, requestInfo.Channel);
                }
            }
        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {

        }
    }
}
