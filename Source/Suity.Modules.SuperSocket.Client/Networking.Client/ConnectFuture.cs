using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Engine;
using Suity.Networking;

namespace Suity.Networking.Client
{
    class ConnectFuture : IFuture
    {
        internal Action<object> _onResult;
        internal Action<ErrorResult> _onError;

        #region IFuture 成员

        public IFuture OnResult(Action<object> onComplete)
        {
            if (onComplete != null)
            {
                _onResult += onComplete;
            }
            return this;
        }

        public IFuture OnError(Action<ErrorResult> onError)
        {
            if (onError != null)
            {
                _onError += onError;
            }
            return this;
        }

        public IFuture OnProgress(Action<object> onProgress)
        {
            return this;
        }

        #endregion
    }
}
