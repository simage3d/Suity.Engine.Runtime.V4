using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.VirtualDom
{
    public class RexActionWrapperEvent : IRexEvent
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexActionWrapperEvent(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }
        public RexActionWrapperEvent(RexTree model, RexActionDefine define)
        {
            _model = model;
            _path = define.Path;
        }
        public RexActionWrapperEvent(RexAction action)
        {
            _model = action.Tree;
            _path = action.Path;
        }

        public void AddListener(Action action)
        {
            _model.AddActionListener(_path, action);
        }

        public void RemoveListener(Action action)
        {
            _model.RemoveListener(_path, action);
        }
    }

    public class RexActionWrapperEvent<T> : IRexEvent<T>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexActionWrapperEvent(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }
        public RexActionWrapperEvent(RexTree model, RexActionDefine define)
        {
            _model = model;
            _path = define.Path;
        }
        public RexActionWrapperEvent(RexAction<T> action)
        {
            _model = action.Tree;
            _path = action.Path;
        }

        public void AddListener(Action<T> action)
        {
            _model.AddActionListener(_path, action);
        }

        public void RemoveListener(Action<T> action)
        {
            _model.RemoveListener(_path, action);
        }
    }

    public class RexActionWrapperEvent<T1, T2> : IRexEvent<T1, T2>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexActionWrapperEvent(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }
        public RexActionWrapperEvent(RexTree model, RexActionDefine define)
        {
            _model = model;
            _path = define.Path;
        }
        public RexActionWrapperEvent(RexAction<T1, T2> action)
        {
            _model = action.Tree;
            _path = action.Path;
        }

        public void AddListener(Action<T1, T2> action)
        {
            _model.AddActionListener(_path, action);
        }

        public void RemoveListener(Action<T1, T2> action)
        {
            _model.RemoveListener(_path, action);
        }
    }

    public class RexActionWrapperEvent<T1, T2, T3> : IRexEvent<T1, T2, T3>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexActionWrapperEvent(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }
        public RexActionWrapperEvent(RexTree model, RexActionDefine define)
        {
            _model = model;
            _path = define.Path;
        }
        public RexActionWrapperEvent(RexAction<T1, T2, T3> action)
        {
            _model = action.Tree;
            _path = action.Path;
        }

        public void AddListener(Action<T1, T2, T3> action)
        {
            _model.AddActionListener(_path, action);
        }

        public void RemoveListener(Action<T1, T2, T3> action)
        {
            _model.RemoveListener(_path, action);
        }
    }

    public class RexActionWrapperEvent<T1, T2, T3, T4> : IRexEvent<T1, T2, T3, T4>
    {
        readonly RexTree _model;
        readonly RexPath _path;

        public RexActionWrapperEvent(RexTree model, RexPath path)
        {
            _model = model;
            _path = path;
        }
        public RexActionWrapperEvent(RexTree model, RexActionDefine define)
        {
            _model = model;
            _path = define.Path;
        }
        public RexActionWrapperEvent(RexAction<T1, T2, T3, T4> action)
        {
            _model = action.Tree;
            _path = action.Path;
        }


        public void AddListener(Action<T1, T2, T3, T4> action)
        {
            _model.AddActionListener(_path, action);
        }

        public void RemoveListener(Action<T1, T2, T3, T4> action)
        {
            _model.RemoveListener(_path, action);
        }
    }
}
