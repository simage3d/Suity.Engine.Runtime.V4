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
    public class RoleValidationFilter : CommandFilterAttribute
    {
        public string Role { get; set; }

        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {
            if (string.IsNullOrEmpty(Role))
            {
                return;
            }

            var session = commandContext.Session as SsAppSession;

            if (session == null || !session.GetRole(Role))
            {
                commandContext.Cancel = true;
                SsRequestInfo requestInfo = commandContext.RequestInfo as SsRequestInfo;
                if (requestInfo != null && requestInfo.Channel > 0)
                {
                    session.Send(new ErrorResult
                    {
                        StatusCode = StatusCodes.NoPermission.ToString(),
                        Message = "Require role : " + Role + ".",
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
