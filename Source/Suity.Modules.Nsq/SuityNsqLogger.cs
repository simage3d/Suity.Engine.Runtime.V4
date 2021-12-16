using NsqSharp.Core;
using Suity.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules.Nsq
{
    class SuityNsqLogger : ILogger
    {
        public void Flush()
        {
        }

        public void Output(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Logs.LogDebug(message);
                    break;
                case LogLevel.Info:
                    Logs.LogInfo(message);
                    break;
                case LogLevel.Warning:
                    Logs.LogWarning(message);
                    break;
                case LogLevel.Error:
                    Logs.LogError(message);
                    break;
                case LogLevel.Critical:
                    Logs.LogError(message);
                    break;
                default:
                    break;
            }
        }
    }
}
