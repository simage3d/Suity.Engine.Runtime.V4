using Suity.Helpers;
using Suity.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity
{
    public static class LogsExtensions
    {
        public static void LogError(this Exception exception)
        {
            Logs.LogError(exception);
        }
        public static void LogError(this Exception exception, string message)
        {
            Logs.LogError(new ExceptionLogItem(message, exception));
        }
        public static void LogError(this Exception exception, string message, object obj)
        {
            Logs.LogError(new ExceptionLogItem(message, exception, obj));
        }
        public static void LogError(this ErrorResult err)
        {
            Logs.LogError(err.ToString());
        }

        public static void LogWarning(this Exception exception)
        {
            Logs.LogWarning(exception);
        }
        public static void LogWarning(this Exception exception, string message)
        {
            Logs.LogWarning(new ExceptionLogItem(message, exception));
        }
        public static void LogWarning(this Exception exception, string message, object obj)
        {
            Logs.LogWarning(new ExceptionLogItem(message, exception, obj));
        }
        public static void LogWarning(this ErrorResult err)
        {
            Logs.LogWarning(err.ToString());
        }
    }

    public class ActionLogItem
    {
        public string Message;
        public Action Action;

        public ActionLogItem()
        {
        }
        public ActionLogItem(string message, Action action)
        {
            Message = message;
            Action = action;
        }
    }

    public class ObjectLogItem : INavigable
    {
        public string Message;
        public object Obj;

        public ObjectLogItem()
        {
        }
        public ObjectLogItem(string message, object obj)
        {
            Message = message;
            Obj = obj;
        }

        public object GetNavigationTarget()
        {
            return Obj;
        }
    }

    public class ExceptionLogItem : ObjectLogItem
    {
        public Exception Exception;

        public ExceptionLogItem()
        {
        }
        public ExceptionLogItem(string message, Exception exception)
            : base(message, null)
        {
            Exception = exception;
        }
        public ExceptionLogItem(string message, Exception exception, object obj)
            : base(message, obj)
        {
            Exception = exception;
        }
    }
}
