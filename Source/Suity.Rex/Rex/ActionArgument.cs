// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex
{
    public abstract class ActionArguments
    {
        internal ActionArguments()
        {
        }

        public abstract int Count { get; }
        public abstract object GetArgument(int index);
    }


    public class ActionArgument : ActionArguments
    {
        public static readonly ActionArgument Empty = new ActionArgument();

        public override int Count { get { return 0; } }
        public override object GetArgument(int index)
        {
            return null;
        }

        public override string ToString()
        {
            return "()";
        }
    }
    public class ActionArgument<T1> : ActionArguments
    {
        public readonly T1 Arg1;

        public ActionArgument(T1 arg1)
        {
            Arg1 = arg1;
        }

        public override int Count { get { return 1; } }
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg1;
                default:
                    return null;
            }
        }

        public override string ToString()
        {
            return string.Format("({0})", Arg1);
        }
    }
    public class ActionArgument<T1, T2> : ActionArguments
    {
        public readonly T1 Arg1;
        public readonly T2 Arg2;

        public ActionArgument(T1 arg1, T2 arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public override int Count { get { return 2; } }
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg1;
                case 1:
                    return Arg2;
                default:
                    return null;
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", Arg1, Arg2);
        }
    }
    public class ActionArgument<T1, T2, T3> : ActionArguments
    {
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;

        public ActionArgument(T1 arg1, T2 arg2, T3 arg3)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }

        public override int Count { get { return 3; } }
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg1;
                case 1:
                    return Arg2;
                case 2:
                    return Arg3;
                default:
                    return null;
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", Arg1, Arg2, Arg3);
        }
    }
    public class ActionArgument<T1, T2, T3, T4> : ActionArguments
    {
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;

        public ActionArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }

        public override int Count { get { return 4; } }
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg1;
                case 1:
                    return Arg2;
                case 2:
                    return Arg3;
                case 3:
                    return Arg4;
                default:
                    return null;
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", Arg1, Arg2, Arg3, Arg4);
        }
    }

    public static class ActionArgumentExtensions
    {
        public static T GetArgumentDefault<T>(this ActionArguments arguments, int index, T defaultValue = default(T))
        {
            object obj = arguments.GetArgument(index);
            if (obj is T t)
            {
                return t;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
