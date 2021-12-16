// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Mapping
{
    public class RexMapperEventArgs : EventArgs
    {
        public Type RequestType { get; }
        public Type ResolvedType { get; }

        public RexMapperEventArgs(Type requestType, Type resolvedType)
        {
            RequestType = requestType;
            ResolvedType = resolvedType;
        }
    }

    public class RexMapperHandlerEventArgs : RexMapperEventArgs
    {
        public object RequestObject { get; }

        public RexMapperHandlerEventArgs(Type requestType, Type resolvedType, object requestObject)
            : base(requestType, resolvedType)
        {
            RequestObject = requestType;
        }
    }

    public class RexMapperProducerEventArgs : RexMapperEventArgs
    {
        public string Name { get; }
        public object ProductObject { get; }

        public RexMapperProducerEventArgs(Type requestType, Type resolvedType, string name, object productObject) : base(requestType, resolvedType)
        {
            Name = name;
            ProductObject = productObject;
        }
    }

    public class RexMapperAssemblerEventArgs : RexMapperEventArgs
    {
        public object TargetObject { get; }
        public object ResultObject { get; }

        public RexMapperAssemblerEventArgs(Type requestType, Type resolvedType, object targetObject, object resultObject) : base(requestType, resolvedType)
        {
            TargetObject = targetObject;
            ResultObject = resultObject;
        }
    }

    public class RexMapperReducerEventArgs : RexMapperEventArgs
    {
        public object OldState { get; }
        public object NewState { get; }

        public RexMapperReducerEventArgs(Type requestType, Type resolvedType, object oldState, object newState) : base(requestType, resolvedType)
        {
            OldState = oldState;
            NewState = newState;
        }
    }

    public class RexMapperUnsolvedEventArgs : EventArgs
    {
        public Type RequestType { get; }

        public RexMapperUnsolvedEventArgs(Type requestType)
        {
            RequestType = requestType;
        }
    }
}
