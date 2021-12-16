using NsqSharp.Bus;
using NsqSharp.Bus.Logging;
using Suity.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules.Nsq
{
    public class MessageAuditor : IMessageAuditor
    {

        public void OnFailed(IBus bus, IFailedMessageInformation failedInfo)
        {
            string action;
            if (failedInfo.FailedAction == FailedMessageQueueAction.Requeue)
                action = "Requeueing...";
            else
                action = "Permanent failure.";


            Logs.LogError(string.Format("[{0}] {1} Message ID {2} on topic {3} channel {4} failed - {5}", DateTime.Now,
                action, failedInfo.Message.Id, failedInfo.Topic, failedInfo.Channel, failedInfo.Exception));
        }

        public void OnReceived(IBus bus, IMessageInformation info) { }
        public void OnSucceeded(IBus bus, IMessageInformation info) { }
    }
}
