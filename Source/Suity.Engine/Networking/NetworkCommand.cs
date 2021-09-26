// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Suity.NodeQuery;
using Suity.Json;
using Suity.Engine;

namespace Suity.Networking
{
    /// <summary>
    /// 网络命令
    /// </summary>
    [AssetDefinitionType(AssetDefinitionCodes.Struct)]
    public abstract class NetworkCommand : Suity.Object, IInfoNode
    {
        public static bool ReturnExceptionMessage = false;
        public static bool OperationLog_SingleQuote = true;

        protected int _counter;
        protected int _errorCounter;

        public abstract string Key { get; }
        public virtual string Path { get; }
        public virtual string Description => DataStorage.GetObject<TypeInfoDescriptor>(Key)?.Description ?? string.Empty;

        public virtual bool IsPublicCommand => false;
        public virtual bool IsNetworkGroupCommand => false;
        public virtual string Claim => null;
        public virtual string Method => null;
        public virtual int OperationLogLevel => -1;
        public virtual string OperationLogCategory => null;

        public abstract bool IsAsync { get; }

        public abstract Type RequestType { get; }
        public abstract Type ResultType { get; }

        public int Counter => _counter;
        public int ErrorCounter => _errorCounter;

        protected override string GetName() => RequestType?.FullName ?? Key;


        public object ExecuteCommand(NetworkSession session, INetworkInfo requestInfo)
        {
            try
            {
                session.LastActiveTime = DateTime.UtcNow;

                Interlocked.Increment(ref _counter);

                if (requestInfo.Body == null || requestInfo.Body.GetType() != RequestType)
                {
                    return HandleUnknownRequest(session, requestInfo);
                }

                if (!(NodeApplication.Current?.VerifyNetworkCommand(this) == true))
                {
                    return HandleServiceUnavailable(session, requestInfo);
                }

                var authErr = CheckAuthentication(session, requestInfo);
                if (authErr != null)
                {
                    return HandleErrorResult(session, requestInfo, authErr);
                }
                OnPreExecute(session, requestInfo);

                object result = Execute(session, requestInfo);
                if (result != null)
                {
                    OnPostExecute(session, requestInfo, result);
                    if (OperationLogLevel >= 0)
                    {
                        AddOperationLog(session, requestInfo.Body, true, OperationLogLevel, OperationLogCategory);
                    }
                    return result;
                }
                else if (ResultType != null)
                {
                    return HandleServerNoResponse(session, requestInfo);
                }
                else
                {
                    return null;
                }
            }
            catch (ExecuteException e)
            {
                return HandleExecuteException(session, requestInfo, e);
            }
            catch (CommandNotImplementedException e)
            {
                return HandleCommandNotImplementedException(session, requestInfo, e);
            }
            catch (Exception e)
            {
                return HandleException(session, requestInfo, e);
            }
        }
        public object ExecuteCommandTarget<TTarget>(NetworkSession session, INetworkInfo requestInfo, TTarget target)
        {
            try
            {
                session.LastActiveTime = DateTime.UtcNow;

                Interlocked.Increment(ref _counter);

                if (requestInfo.Body == null || requestInfo.Body.GetType() != RequestType)
                {
                    return HandleUnknownRequest(session, requestInfo);
                }

                if (!(NodeApplication.Current?.VerifyNetworkCommand(this) == true))
                {
                    return HandleServiceUnavailable(session, requestInfo);
                }

                var authErr = CheckAuthentication(session, requestInfo);
                if (authErr != null)
                {
                    return HandleErrorResult(session, requestInfo, authErr);
                }
                OnPreExecute(session, requestInfo);

                object result = ExecuteTarget<TTarget>(session, requestInfo, target);
                if (result != null)
                {
                    OnPostExecute(session, requestInfo, result);
                    if (OperationLogLevel >= 0)
                    {
                        AddOperationLog(session, requestInfo.Body, true, OperationLogLevel, OperationLogCategory);
                    }
                    return result;
                }
                else if (ResultType != null)
                {
                    return HandleServerNoResponse(session, requestInfo);
                }
                else
                {
                    return null;
                }
            }
            catch (ExecuteException e)
            {
                return HandleExecuteException(session, requestInfo, e);
            }
            catch (CommandNotImplementedException e)
            {
                return HandleCommandNotImplementedException(session, requestInfo, e);
            }
            catch (Exception e)
            {
                return HandleException(session, requestInfo, e);
            }
        }

        public ErrorResult CheckAuthentication(NetworkSession session, INetworkInfo requestInfo)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (!(NodeApplication.Current?.VerifyNetworkCommand(this) == true))
            {
                return HandleServiceUnavailable(session, requestInfo);
            }


            bool networkGroupUser = session.User is NetworkGroupUser;

            if (IsNetworkGroupCommand && !networkGroupUser)
            {
                return new ErrorResult
                {
                    StatusCode = nameof(StatusCodes.NetworkGroupUserOnly),
                    Message = $"{session.RemoteEndPoint.Address} is not in network group.",
                    Location = Suity.Environment.Location,
                };
            }

            if (networkGroupUser)
            {
                return null;
            }

            if (!IsPublicCommand && session.User == null)
            {
                return new ErrorResult
                {
                    StatusCode = nameof(StatusCodes.NotLoggedIn),
                    Message = nameof(StatusCodes.NotLoggedIn),
                    Location = Suity.Environment.Location,
                };
            }

            if (Claim != null && (session.User == null || !session.User.Claims.Contains(Claim)))
            {
                return new ErrorResult
                {
                    StatusCode = nameof(StatusCodes.NoPermission),
                    Message = nameof(StatusCodes.NoPermission),
                    Location = Suity.Environment.Location,
                };
            }

            return null;
        }


        private ErrorResult HandleExecuteException(NetworkSession session, INetworkInfo requestInfo, ExecuteException e)
        {
            return HandleErrorResult(
                session,
                requestInfo,
                new ErrorResult
                {
                    StatusCode = e.StatusCode,
                    Message = e.Message,
                    Location = Suity.Environment.Location,
                }, e);
        }
        private ErrorResult HandleCommandNotImplementedException(NetworkSession session, INetworkInfo requestInfo, CommandNotImplementedException e)
        {
            Logs.LogWarning($"Network command is not implemented : {this.GetType().FullName} ({this.GetType().Assembly.FullName})");
            return HandleErrorResult(
                session,
                requestInfo,
                new ErrorResult
                {
                    StatusCode = nameof(StatusCodes.ServerNotImplemented),
                    Message = "Network command is not implemented.",
                    Location = Suity.Environment.Location,
                });
        }
        private ErrorResult HandleException(NetworkSession session, INetworkInfo requestInfo, Exception e)
        {
            Logs.LogError(e);
            if (ReturnExceptionMessage)
            {
                return HandleErrorResult(
                    session,
                    requestInfo,
                    new ErrorResult
                    {
                        StatusCode = nameof(StatusCodes.ServerInternalError),
                        Message = e.Message,
                        Location = Suity.Environment.Location,
                    }, e);
            }
            else
            {
                return HandleErrorResult(
                    session,
                    requestInfo,
                    new ErrorResult
                    {
                        StatusCode = nameof(StatusCodes.ServerInternalError),
                        Message = nameof(StatusCodes.ServerInternalError),
                        Location = Suity.Environment.Location,
                    }, e);
            }
        }
        private ErrorResult HandleUnknownRequest(NetworkSession session, INetworkInfo requestInfo)
        {
            return new ErrorResult
            {
                StatusCode = nameof(StatusCodes.UnknownRequest),
                Location = Suity.Environment.Location,
            };
        }
        private ErrorResult HandleServiceUnavailable(NetworkSession session, INetworkInfo requestInfo)
        {
            return new ErrorResult
            {
                StatusCode = nameof(StatusCodes.ServiceUnavailable),
                Location = Suity.Environment.Location,
            };
        }
        private ErrorResult HandleServerNoResponse(NetworkSession session, INetworkInfo requestInfo)
        {
            return HandleErrorResult(
                session,
                requestInfo,
                new ErrorResult
                {
                    StatusCode = nameof(StatusCodes.ServerNoResponse),
                    Message = "Result is empty.",
                    Location = Suity.Environment.Location,
                });
        }
        private ErrorResult HandleErrorResult(NetworkSession session, INetworkInfo requestInfo, ErrorResult errorResult)
        {
            Interlocked.Increment(ref _errorCounter);
            OnError(session, requestInfo, errorResult);
            Logs.AddNetworkLog(LogMessageType.Warning, NetworkDirection.None, session.SessionId, this.GetType().Name, errorResult.Message);
            if (OperationLogLevel >= 0)
            {
                AddOperationLog(session, requestInfo.Body, false, OperationLogLevel, OperationLogCategory);
            }
            return errorResult;
        }
        private ErrorResult HandleErrorResult(NetworkSession session, INetworkInfo requestInfo, ErrorResult errorResult, Exception exception)
        {
            Interlocked.Increment(ref _errorCounter);
            OnError(session, requestInfo, errorResult);
            Logs.AddNetworkLog(LogMessageType.Warning, NetworkDirection.None, session.SessionId, this.GetType().Name, exception);
            if (OperationLogLevel >= 0)
            {
                AddOperationLog(session, requestInfo.Body, false, OperationLogLevel, OperationLogCategory);
            }
            return errorResult;
        }


        protected virtual object Execute(NetworkSession session, INetworkInfo requestInfo)
        {
            throw new NotImplementedException();
        }
        protected virtual object ExecuteTarget<TTarget>(NetworkSession session, INetworkInfo requestInfo, TTarget target)
        {
            return Execute(session, requestInfo);
        }

        protected virtual bool AddOperationLog(NetworkSession session, object requestObj, bool success, int level, string category)
        {
            if (session.User is NetworkGroupUser)
            {
                return false;
            }
            if (requestObj == null)
            {
                return false;
            }

            Logs.AddOperationLog(level, category, session.User?.UserId, session.RemoteEndPoint.ToString(), requestObj, success);
            return true;




            //var logger = session.GetService<INetworkOperationLog>();
            //if (logger == null)
            //{
            //    return false;
            //}

            //try
            //{
            //    JsonDataWriter writer = new JsonDataWriter();
            //    ObjectType.WriteObject(writer, requestObj);
            //    string data = writer.ToString(false).Replace('"', '\'');

            //    logger.AddOperationLog(level, category, session.User?.UserId, session.RemoteEndPoint.Address.ToString(), data, success);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            //return true;
        }

        protected virtual void OnPreExecute(NetworkSession session, INetworkInfo requestInfo)
        {
        }
        protected virtual void OnPostExecute(NetworkSession session, INetworkInfo requestInfo, object result)
        {
        }
        protected virtual void OnError(NetworkSession session, INetworkInfo requestInfo, ErrorResult errorResult)
        {

        }


        //#region IVisionTreeObject
        //void IVisionTreeObject.SetupVisionTree(IViewObjectSetup setup)
        //{
        //}
        //void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        //{
        //}
        //#endregion

        //#region ITextDisplay
        //string ITextDisplay.Text => Name;

        //object ITextDisplay.Icon => null;

        //TextStatus ITextDisplay.TextStatus => IsPublicCommand ? TextStatus.Anonymous : TextStatus.Normal;
        //#endregion

        //#region IPreviewDisplay
        //string IPreviewDisplay.PreviewText => this.GetType().FullName;

        //object IPreviewDisplay.PreviewIcon => null;
        //#endregion

        //#region Log
        //protected void LogDebug(object message) { NodeApplication.Current.AddLog(LogMessageType.Debug, message); }
        //protected void LogInfo(object message) { NodeApplication.Current.AddLog(LogMessageType.Info, message); }
        //protected void LogWarning(object message) { NodeApplication.Current.AddLog(LogMessageType.Warning, message); }
        //protected void LogError(object message) { NodeApplication.Current.AddLog(LogMessageType.Error, message); }
        //#endregion

        #region IInfoWriter
        void IInfoNode.WriteInfo(INodeWriter writer)
        {
            string icon = "*CoreIcon|Command";
            if (IsPublicCommand)
            {
                icon = "*CoreIcon|Login";
            }
            else if (!string.IsNullOrEmpty(Claim))
            {
                icon = "*CoreIcon|Role";
            }

            writer.WriteInfo("NetworkCommand", Name, this.GetType().Name, $"Counter:{_counter}", TextStatus.Normal, icon);
        }
        #endregion
    }

    /// <summary>
    /// 网络指令
    /// </summary>
    /// <typeparam name="TRequest">请求类型</typeparam>
    /// <typeparam name="TResult">结果类型</typeparam>
    public abstract class NetworkCommand<TRequest, TResult> : NetworkCommand
    {
        public override bool IsAsync => false;
        public sealed override Type RequestType => typeof(TRequest);
        public sealed override Type ResultType => typeof(TResult);

        protected override object Execute(NetworkSession session, INetworkInfo requestInfo)
        {
            return Execute(session, requestInfo, (TRequest)requestInfo.Body);
        }
        protected override object ExecuteTarget<TTarget>(NetworkSession session, INetworkInfo requestInfo, TTarget target)
        {
            return ExecuteTarget<TTarget>(session, requestInfo, target, (TRequest)requestInfo.Body);
        }

        protected virtual TResult Execute(NetworkSession session, INetworkInfo requestInfo, TRequest request)
        {
            throw new NotImplementedException();
        }
        protected virtual TResult ExecuteTarget<TTarget>(NetworkSession session, INetworkInfo requestInfo, TTarget target, TRequest request)
        {
            return Execute(session, requestInfo, request);
        }
    }

    /// <summary>
    /// 网络推送指令
    /// </summary>
    /// <typeparam name="TRequest">请求类型</typeparam>
    public abstract class NetworkCommand<TRequest> : NetworkCommand
    {
        public override bool IsAsync => false;
        public sealed override Type RequestType => typeof(TRequest);
        public sealed override Type ResultType => null;

        protected override object Execute(NetworkSession session, INetworkInfo requestInfo)
        {
            Execute(session, requestInfo, (TRequest)requestInfo.Body);
            return null;
        }
        protected override object ExecuteTarget<TTarget>(NetworkSession session, INetworkInfo requestInfo, TTarget target)
        {
            ExecuteTarget<TTarget>(session, requestInfo, target, (TRequest)requestInfo.Body);
            return null;
        }

        protected virtual void Execute(NetworkSession session, INetworkInfo requestInfo, TRequest request)
        {
            throw new NotImplementedException();
        }
        protected virtual void ExecuteTarget<TTarget>(NetworkSession session, INetworkInfo requestInfo, TTarget target, TRequest request)
        {
            Execute(session, requestInfo, request);
        }
    }

    /// <summary>
    /// 异步网络指令
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class FutureNetworkCommand<TRequest, TResult> : NetworkCommand
    {
        public override bool IsAsync => true;
        public sealed override Type RequestType => typeof(TRequest);
        public sealed override Type ResultType => typeof(TResult);

        protected override object Execute(NetworkSession session, INetworkInfo requestInfo)
        {
            return Execute(session, requestInfo, (TRequest)requestInfo.Body);
        }
        protected override object ExecuteTarget<TTarget>(NetworkSession session, INetworkInfo requestInfo, TTarget target)
        {
            return ExecuteTarget<TTarget>(session, requestInfo, target, (TRequest)requestInfo.Body);
        }

        protected virtual IFuture<TResult> Execute(NetworkSession session, INetworkInfo requestInfo, TRequest request)
        {
            throw new NotImplementedException();
        }
        protected virtual IFuture<TResult> ExecuteTarget<TTarget>(NetworkSession session, INetworkInfo requestInfo, TTarget target, TRequest request)
        {
            return Execute(session, requestInfo, request);
        }
    }

}
