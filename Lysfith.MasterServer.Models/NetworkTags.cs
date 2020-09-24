using System;
using System.Collections.Generic;
using System.Text;

namespace Lysfith.MasterServer.Models
{
    public static class NetworkTags
    {
        public const ushort C_CreateRoom = 0;
        public const ushort S_CreateRoomResult = 1;

        public const ushort C_JoinRoom = 2;
        public const ushort S_JoinRoomResult = 3;

        public const ushort C_DestroyRoom = 4;
        public const ushort S_DestroyRoomResult = 5;

        public const ushort S_ExcludeFromRoom = 6;
        public const ushort C_QuitRoom = 7;
        public const ushort S_QuitRoomResult = 8;


    }
}
