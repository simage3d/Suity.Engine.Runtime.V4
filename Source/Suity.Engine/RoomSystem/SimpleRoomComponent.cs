// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using Suity.Engine;
using Suity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Suity.RoomSystem
{
    public class SimpleRoomComponent : NodeComponent
    {
        long _roomAlloc = 0;

        readonly Dictionary<long, Room> _rooms = new Dictionary<long, Room>();

        public void AddRoom(Room room)
        {
            if (room == null)
            {
                throw new ArgumentNullException();
            }
            if (room.Id > 0)
            {
                throw new InvalidOperationException("Room is in component.");
            }

            room.SetRoomId(AllocateRoomId());
            lock (_rooms)
            {
                _rooms.Add(room.Id, room);
            }

            room.Start();
        }
        public void RemoveRoom(Room room)
        {
            if (room == null)
            {
                throw new ArgumentNullException();
            }
            if (room.Id > 0)
            {
                throw new InvalidOperationException("Room is not in this component.");
            }

            lock (_rooms)
            {
                _rooms.Remove(room.Id);
            }

            room.Stop();
        }
        public Room GetRoom(long roomId)
        {
            lock (_rooms)
            {
                return _rooms.GetValueOrDefault(roomId);
            }
        }
        public int RoomCount
        {
            get { return _rooms.Count; }
        }



        private long AllocateRoomId()
        {
            return Interlocked.Increment(ref _roomAlloc);
        }
    }
}
