// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Helpers;
using Suity.Networking;
using Suity.NodeQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.RoomSystem
{
    public enum RoomState
    {
        None,
        Started,
        Released,
    }

    public enum RoomOpState
    {
        Successful,
        NotInRoom,
        RoomIsFull,
        RoomIsClosed,
    }

    public abstract class Room : ObjectWithId, IInfoNode
    {
        readonly SingleThreadActionQueue _actionQueue = new SingleThreadActionQueue();
        readonly HashSet<NetworkSession> _users = new HashSet<NetworkSession>();

        internal long _id;

        public override long Id => _id;
        public RoomState State { get; private set; } = RoomState.None;

        public event Action<Room, NetworkSession> UserAdded;
        public event Action<Room, NetworkSession, string> UserRemoved;

        protected readonly object _sync = new object();

        public Room()
        {
        }
        ~Room()
        {
            Stop();
        }

        public void Start(Action started = null)
        {
            if (State != RoomState.None)
            {
                return;
            }

            State = RoomState.Started;

            _actionQueue.QueueAction(() =>
            {
                try
                {
                    OnStart();
                    Logs.LogInfo($"Start {GetType().Name} : {Id}.");
                    started?.Invoke();
                }
                catch (Exception err)
                {
                    Logs.LogError(err);
                }
            });
        }
        public void Stop(Action stopped = null)
        {
            if (State != RoomState.Started)
            {
                return;
            }

            State = RoomState.Released;

            lock (_sync)
            {
                _users.Clear();
                _actionQueue.QueueAction(() =>
                {
                    try
                    {
                        OnStop();
                    }
                    catch (Exception err)
                    {
                        Logs.LogError(err);
                    }
                    finally
                    {
                        _actionQueue.Clear();

                        Logs.LogInfo($"Stop {GetType().Name} : {Id}.");

                        _actionQueue.Dispose();
                        stopped?.Invoke();
                    }
                });
            }
        }

        public void SetRoomId(long id)
        {
            _id = id;
        }
        public void Update()
        {
            _actionQueue.QueueAction(() =>
            {
                OnUpdate();
            });
        }

        public void DoAction(Action action)
        {
            _actionQueue.QueueAction(action);
        }

        public RoomOpState AddUser(NetworkSession session, Action added = null)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            if (State != RoomState.Started)
            {
                return RoomOpState.RoomIsClosed;
            }

            lock (_sync)
            {
                if (_users.Add(session))
                {
                    session.SetProperty<Room>(this);

                    DoAction(() =>
                    {
                        OnUserAdded(session);
                        added?.Invoke();
                    });
                }
                return RoomOpState.Successful;
            }
        }
        public RoomOpState RemoveUser(NetworkSession session, string reason, Action removed = null)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            if (State != RoomState.Started)
            {
                return RoomOpState.RoomIsClosed;
            }

            lock (_sync)
            {
                if (_users.Remove(session))
                {
                    DoAction(() =>
                    {
                        OnUserRemoved(session, reason);
                        removed?.Invoke();
                        session.SetProperty<Room>(null);
                    });
                }
                return RoomOpState.Successful;
            }
        }

        public int UserCount => _users.Count;





        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
        protected virtual void OnUpdate() { }

        protected virtual void OnUserAdded(NetworkSession session)
        {
            UserAdded?.Invoke(this, session);
        }
        protected virtual void OnUserRemoved(NetworkSession session, string reason)
        {
            UserRemoved?.Invoke(this, session, reason);
        }

        public virtual void WriteInfo(INodeWriter writer)
        {
        }
    }
}
