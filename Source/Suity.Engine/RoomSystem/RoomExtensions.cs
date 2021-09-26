// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.RoomSystem
{
    public static class RoomExtensions
    {
        public const string Context_Arg_Room = "Room";

        public static void SetRoom(this FunctionContext ctx, Room room)
        {
            ctx.SetArgument(Context_Arg_Room, room);
        }
        public static Room GetRoom(this FunctionContext ctx, Room room)
        {
            return ctx.GetArgument(Context_Arg_Room) as Room;
        }
    }
}
