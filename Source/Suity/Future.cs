// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public class Future<T> : IFuture<T>
    {
        enum ResultTypes
        {
            None,
            Result,
            Error,
        }

        Action<T> _onResult;
        Action<ErrorResult> _onError;
        Action<object> _onProgress;

        ResultTypes _type = ResultTypes.None;
        T _result;
        ErrorResult _error;

        IFuture IFuture.OnResult(Action<object> onResult)
        {
            if (onResult == null)
            {
                return this;
            }

            switch (_type)
            {
                case ResultTypes.Result:
                    onResult(_result);
                    break;
                case ResultTypes.Error:
                    break;
                case ResultTypes.None:
                default:
                    _onResult += o => onResult(o);
                    break;
            }

            return this;
        }

        IFuture IFuture.OnError(Action<ErrorResult> onError)
        {
            if (onError == null)
            {
                return this;
            }

            switch (_type)
            {
                case ResultTypes.Result:
                    break;
                case ResultTypes.Error:
                    onError(_error);
                    break;
                case ResultTypes.None:
                default:
                    _onError += onError;
                    break;
            }

            return this;
        }

        IFuture IFuture.OnProgress(Action<object> onProgress)
        {
            if (onProgress != null)
            {
                _onProgress += onProgress;
            }
            return this;
        }


        public IFuture<T> OnResult(Action<T> onResult)
        {
            if (onResult == null)
            {
                return this;
            }

            switch (_type)
            {
                case ResultTypes.Result:
                    onResult(_result);
                    break;
                case ResultTypes.Error:
                    break;
                case ResultTypes.None:
                default:
                    _onResult += onResult;
                    break;
            }

            return this;
        }

        public IFuture<T> OnError(Action<ErrorResult> onError)
        {
            if (onError == null)
            {
                return this;
            }

            switch (_type)
            {
                case ResultTypes.Result:
                    break;
                case ResultTypes.Error:
                    onError(_error);
                    break;
                case ResultTypes.None:
                default:
                    _onError += onError;
                    break;
            }

            return this;
        }

        public IFuture<T> OnProgress(Action<object> onProgress)
        {
            if (onProgress != null)
            {
                _onProgress += onProgress;
            }
            return this;
        }


        public void SetResult(T obj, bool queued = false)
        {
            //保证不会重复设置结果
            if(_type != ResultTypes.None)
            {
                return;
            }

            _type = ResultTypes.Result;
            _result = obj;

            if (_onResult != null)
            {
                if (queued)
                {
                    QueuedAction.Do(() => _onResult(obj));
                }
                else
                {
                    _onResult(obj);
                }
            }
        }
        public void SetError(ErrorResult error, bool queued = false)
        {
            //保证不会重复设置结果
            if (_type != ResultTypes.None)
            {
                return;
            }

            _type = ResultTypes.Error;
            _error = error;

            if (_onError != null)
            {
                if (queued)
                {
                    QueuedAction.Do(() => _onError(error));
                }
                else
                {
                    _onError(error);
                }
            }
        }

        public void SetError(StatusCodes code, string message = null, string location = null, bool queued = false)
        {
            SetError(new ErrorResult { StatusCode = code.ToString(), Message = message, Location = location }, queued);
        }
        public void SetError(string code, string message = null, string location = null, bool queued = false)
        {
            SetError(new ErrorResult { StatusCode = code, Message = message, Location = location }, queued);
        }
        public void SetProgress(object progress, bool queued = false)
        {
            //保证不会重复设置结果
            if (_type != ResultTypes.None)
            {
                return;
            }

            if (_onProgress != null)
            {
                if (queued)
                {
                    QueuedAction.Do(() => _onProgress(progress));
                }
                else
                {
                    _onProgress(progress);
                }
            }
        }


    }
}
