// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Suity.Helpers
{
    public class SingleThreadActionQueue : IDisposable
    {
        readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
        Thread _thread;
        readonly EventWaitHandle _wait = new AutoResetEvent(true);

        bool _runing = true;

        public Thread WorkingThread => _thread;


        public SingleThreadActionQueue()
        {
            _thread = new Thread(Update);
            _thread.Start();
        }

        public void QueueAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            _actions.Enqueue(action);
            _wait.Set();
        }

        void Update()
        {
            while (true)
            {
                while (_actions.TryDequeue(out Action action))
                {
                    try
                    {
                        action();
                    }
                    catch (Exception err)
                    {
                        Logs.LogError(err);
                    }
                }
                if (!_runing)
                {
                    break;
                }

                _wait.WaitOne();
            }


            _thread = null;
        }

        public void Clear()
        {
            while (_actions.TryDequeue(out Action action))
            {
            }
        }

        public void Dispose()
        {
            _runing = false;
            _wait.Set();
        }
    }
}
