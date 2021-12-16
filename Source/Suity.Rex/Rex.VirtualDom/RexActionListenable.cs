// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.VirtualDom
{
    public sealed class RexActionListenable<TListen> : IRexTreeInstance<ActionArgument<Action<TListen>>>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexActionListenable(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }
        public RexActionListenable(RexTree model, RexActionDefine define)
        {
            _model = model;
            _path = define.Path;
        }

        public RexTree Tree { get { return _model; } }
        public RexPath Path { get { return _path; } }

        public IRexListener<TListen> Invoke()
        {
            ActionListener<TListen> listener = new ActionListener<TListen>();
            _model.DoAction(_path, new Action<TListen>(listener.HandleCallBack));
            return listener;
        }
        public IRexListener<TListen> InvokeQueued()
        {
            ActionListener<TListen> listener = new ActionListener<TListen>();
            _model.DoActionQueued(_path, new Action<TListen>(listener.HandleCallBack));
            return listener;
        }
        public void AddActionListener(Action<Action<TListen>> action, string tag = null)
        {
            _model.AddActionListener(_path, action, tag);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }
}
