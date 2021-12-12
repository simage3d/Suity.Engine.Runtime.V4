// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using Suity.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Suity.Engine;


namespace Suity.Engine.LocalMessaging
{
    public class LocalMessageBus : MarshalByRefObject
    {
        readonly UniqueMultiDictionary<string, LocalMessageHandler> _handlers = new UniqueMultiDictionary<string, LocalMessageHandler>();

        public LocalMessageBus()
        {
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Subscribe(LocalMessageHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (string.IsNullOrEmpty(handler.TypeName))
            {
                throw new ArgumentException("handler.TypeName is empty.", nameof(handler.TypeName));
            }

            lock (this)
            {
                _handlers.Add(handler.TypeName, handler);
            }
        }
        public void Unsubscribe(LocalMessageHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (string.IsNullOrEmpty(handler.TypeName))
            {
                throw new ArgumentException("handler.TypeName is empty.", nameof(handler.TypeName));
            }

            lock (this)
            {
                _handlers.Remove(handler.TypeName, handler);
            }
        }
        public void Clear()
        {
            lock (this)
            {
                _handlers.Clear();
            }
        }

        public void Send(string channel, string message)
        {
            if (string.IsNullOrEmpty(channel))
            {
                throw new ArgumentException("channel is empty.", nameof(channel));
            }

            lock (this)
            {
                foreach (var handler in _handlers[channel])
                {
                    try
                    {
                        handler.HandleMessage(message);
                    }
                    catch (Exception err)
                    {
                        err.LogError("Send message failed.");
                    }
                }
            }
        }
    }
}
