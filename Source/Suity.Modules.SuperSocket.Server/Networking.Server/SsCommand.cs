using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suity.Engine;
using SuperSocket.SocketBase.Command;

namespace Suity.Networking.Server
{
    public abstract class SsCommand<TRequest, TResult> : CommandBase<SsAppSession, SsRequestInfo>
    {
        public static bool SendErrorMessage = true;

        public override void ExecuteCommand(SsAppSession session, SsRequestInfo requestInfo)
        {
            session.LogIncomingPackage(requestInfo.Body, requestInfo.Method, requestInfo.Channel);

            TRequest request = (TRequest)requestInfo.Body;

            session.EnterExecute();

            try
            {
                TResult result = Execute(session, request);
                if (result != null)
                {
                    session.Send(result, requestInfo.Method, requestInfo.Channel);
                }
                else
                {
                    session.Send(new ErrorResult
                    {
                        StatusCode = StatusCodes.ServerNoResponse.ToString(),
                        Location = NodeApplication.Current?.ServiceId,
                    }, requestInfo.Method, requestInfo.Channel);
                }
            }
            catch (ExecuteException e)
            {
                session.Send(new ErrorResult
                {
                    StatusCode = e.StatusCode,
                    Message = e.Message,
                    Location = NodeApplication.Current?.ServiceId,
                }, requestInfo.Method, requestInfo.Channel);
            }
            catch (NotImplementedException)
            {
                session.Send(new ErrorResult
                {
                    StatusCode = StatusCodes.ServerNotImplemented.ToString(),
                    Location = NodeApplication.Current?.ServiceId,
                }, requestInfo.Method, requestInfo.Channel);
            }
            catch (Exception e)
            {
                if (SendErrorMessage)
                {
                    session.Send(new ErrorResult
                    {
                        StatusCode = StatusCodes.ServerInternalError.ToString(),
                        Message = e.Message,
                        Location = NodeApplication.Current?.ServiceId,
                    }, requestInfo.Method, requestInfo.Channel);
                }
                else
                {
                    session.Send(new ErrorResult
                    {
                        StatusCode = StatusCodes.ServerInternalError.ToString(),
                        Location = NodeApplication.Current?.ServiceId,
                    }, requestInfo.Method, requestInfo.Channel);
                }
                throw;
            }
            finally
            {
                session.ExitExecute();
            }
        }

        protected abstract TResult Execute(SsAppSession session, TRequest request);
    }
}
