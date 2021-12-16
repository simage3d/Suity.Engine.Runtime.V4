using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Engine;
using Suity.Networking;

namespace Suity.Networking.Client
{
    interface ISendFuture : IFuture
    {
        object Request { get; }
        DateTime SendTime { get; }
        void SetSendResult(object obj);
        void SetSendError(ErrorResult error);
    }

    class SendFuture<T> : Future<T>, ISendFuture
    {
        internal Action<object> _onResult;
        internal Action<ErrorResult> _onError;
        public object Request { get; private set; }

        public DateTime SendTime { get; private set; }

        public SendFuture(object request)
        {
            Request = request;
            SendTime = DateTime.Now;
        }

        public void SetSendResult(object obj)
        {
            if (obj is T)
            {
                base.SetResult((T)obj);
            }
            else
            {
                base.SetError(new ErrorResult
                {
                    StatusCode = StatusCodes.InvalidCast.ToString(),
                    Location = NodeApplication.Current?.ServiceId,
                });
            }
        }

        public void SetSendError(ErrorResult error)
        {
            base.SetError(error);
        }
    }
}
