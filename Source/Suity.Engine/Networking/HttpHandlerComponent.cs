// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    /// <summary>
    /// Http处理组件
    /// </summary>
    [NodeService(typeof(HttpHandlerComponent))]
    public abstract class HttpHandlerComponent : NodeComponent
    {
        public delegate TResult CommandHandler<TRequest, TResult>(NetworkSession session, INetworkInfo info, TRequest request);
        public delegate TResult CommandHandler<TResult>(NetworkSession session, INetworkInfo info);


        readonly HashSet<NetworkCommand> _commands = new HashSet<NetworkCommand>();
        HttpCommandFamily _family;


        public HttpHandlerComponent()
        {
        }
        public HttpHandlerComponent(string basePath)
            : this()
        {
            if (!string.IsNullOrEmpty(basePath))
            {
                BasePath = basePath.Trim('/', '\\');
            }
        }

        public override string Icon => "*CoreIcon|Network";

        public string BasePath { get; }

        public NetworkCommandFamily Family => _family;


        protected void Get<TRequest, TResult>(string path, CommandHandler<TRequest, TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Get, func, requireLogin, claim);
        }
        protected void Post<TRequest, TResult>(string path, CommandHandler<TRequest, TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Post, func, requireLogin, claim);
        }
        protected void Put<TRequest, TResult>(string path, CommandHandler<TRequest, TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Put, func, requireLogin, claim);
        }
        protected void Patch<TRequest, TResult>(string path, CommandHandler<TRequest, TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Patch, func, requireLogin, claim);
        }
        protected void Delete<TRequest, TResult>(string path, CommandHandler<TRequest, TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Delete, func, requireLogin, claim);
        }
        protected void Options<TRequest, TResult>(string path, CommandHandler<TRequest, TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Options, func, requireLogin, claim);
        }
        protected void Head<TRequest, TResult>(string path, CommandHandler<TRequest, TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Head, func, requireLogin, claim);
        }


        protected void Get<TResult>(string path, CommandHandler<TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Get, func, requireLogin, claim);
        }
        protected void Post<TResult>(string path, CommandHandler<TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Post, func, requireLogin, claim);
        }
        protected void Put<TResult>(string path, CommandHandler<TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Put, func, requireLogin, claim);
        }
        protected void Patch<TResult>(string path, CommandHandler<TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Patch, func, requireLogin, claim);
        }
        protected void Delete<TResult>(string path, CommandHandler<TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Delete, func, requireLogin, claim);
        }
        protected void Options<TResult>(string path, CommandHandler<TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Options, func, requireLogin, claim);
        }
        protected void Head<TResult>(string path, CommandHandler<TResult> func, bool requireLogin = false, string claim = null)
        {
            RegisterCustomCommand(path, HttpMethod.Head, func, requireLogin, claim);
        }

        protected void GetHtml(string path, string text, bool requireLogin = false, string claim = null)
        {
            Get<HtmlResult>(path, (s, i) => text, requireLogin, claim);
        }
        protected void GetObject<TResult>(string path, TResult obj, bool requireLogin = false, string claim = null)
        {
            Get<TResult>(path, (s, i) => obj, requireLogin, claim);
        }
        protected void GetStatic<TResult>(string path, Func<TResult> func, bool requireLogin = false, string claim = null)
        {
            Get(path, (s, i) => func(), requireLogin, claim);
        }


        private void RegisterCustomCommand<TRequest, TResult>(string path, HttpMethod method, CommandHandler<TRequest, TResult> func, bool requireLogin = false, string claim = null)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            path = CombinePath(path);

            HttpCommand<TRequest, TResult> command = new HttpCommand<TRequest, TResult>(this, path, method, func)
            {
                _isPublicCommand = !requireLogin,
                _claim = claim
            };

            _commands.Add(command);
        }
        private void RegisterCustomCommand<TResult>(string path, HttpMethod method, CommandHandler<TResult> func, bool requireLogin = false, string claim = null)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            path = CombinePath(path);

            HttpCommand<TResult> command = new HttpCommand<TResult>(this, path, method, func)
            {
                _isPublicCommand = !requireLogin,
                _claim = claim
            };

            _commands.Add(command);
        }

        private string CombinePath(string path)
        {
            path = path?.Trim('/', '\\') ?? string.Empty;
            if (!string.IsNullOrEmpty(BasePath))
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = BasePath;
                }
                else
                {
                    path = BasePath + "/" + path;
                }
            }
            if (string.IsNullOrEmpty(path))
            {
                path = "/";
            }
            return path;
        }

        internal override void Awake()
        {
            base.Awake();

            string name = this.Name;
            if (string.IsNullOrEmpty(name))
            {
                name = this.GetType().FullName;
            }

            _family = new HttpCommandFamily(this, name, name, _commands);
        }
        internal override void Stop()
        {
            base.Stop();
            _family = null;
        }

        #region HttpMethod
        enum HttpMethod
        {
            Get,
            Post,
            Put,
            Patch,
            Delete,
            Options,
            Head,
        }
        #endregion

        #region HttpCommandFamily
        class HttpCommandFamily : NetworkCommandFamily
        {
            readonly HttpHandlerComponent _handler;

            public HttpCommandFamily(HttpHandlerComponent handler, string key, string name, IEnumerable<NetworkCommand> commands)
                : base(key, name)
            {
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));
                foreach (var command in commands)
                {
                    RegisterCommand(command);
                }
            }
        }
        #endregion

        #region HttpCommand<TRequest, TResult>
        class HttpCommand<TRequest, TResult> : NetworkCommand<TRequest, TResult>
        {
            readonly HttpHandlerComponent _handler;
            readonly string _path;
            readonly string _desc;
            readonly HttpMethod _method;
            readonly CommandHandler<TRequest, TResult> _func;

            internal bool _isPublicCommand;
            internal string _claim;

            public override string Key => _path;
            public override string Description => _desc;
            public override string Method => _method.ToString();
            public override bool IsPublicCommand => _isPublicCommand;
            public override string Claim => _claim;
            public HttpMethod Mode => _method;

            protected override string GetName()
            {
                return _path;
            }

            public HttpCommand(HttpHandlerComponent handler, string path, HttpMethod method, CommandHandler<TRequest, TResult> func)
            {
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));
                _path = path ?? string.Empty;
                _method = method;
                _func = func ?? throw new ArgumentNullException(nameof(func));
                _desc = $"{_method}:{_path}";
            }

            protected override TResult Execute(NetworkSession session, INetworkInfo info, TRequest request)
            {
                return _func(session, info, request);
            }
        }
        #endregion

        #region HttpCommand<TResult>
        class HttpCommand<TResult> : NetworkCommand<string, TResult>
        {
            readonly HttpHandlerComponent _handler;
            readonly string _path;
            readonly string _desc;
            readonly HttpMethod _method;
            readonly CommandHandler<TResult> _func;

            internal bool _isPublicCommand;
            internal string _claim;

            public override string Key => _path;
            public override string Path => _path;
            public override string Description => _desc;
            public override string Method => _method.ToString();
            public override bool IsPublicCommand => _isPublicCommand;
            public override string Claim => _claim;
            public HttpMethod Mode => _method;
            protected override string GetName()
            {
                return _path;
            }

            public HttpCommand(HttpHandlerComponent handler, string path, HttpMethod method, CommandHandler<TResult> func)
            {
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));
                _path = path ?? string.Empty;
                _method = method;
                _func = func ?? throw new ArgumentNullException(nameof(func));
                _desc = $"{_method}:{_path}";
            }

            protected override TResult Execute(NetworkSession session, INetworkInfo info, string request)
            {
                return _func(session, info);
            }
        }
        #endregion
    }

}
