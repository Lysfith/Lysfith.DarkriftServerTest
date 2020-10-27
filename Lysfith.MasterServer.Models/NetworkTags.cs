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
        public const ushort S_JoinRoomSuccess = 3;
        public const ushort S_JoinRoomError = 4;

        public const ushort C_DestroyRoom = 5;
        public const ushort S_DestroyRoomResult = 6;

        public const ushort S_ExcludeFromRoom = 7;
        public const ushort C_QuitRoom = 8;
        public const ushort S_QuitRoomResult = 9;

        public const ushort C_CloseRoom = 10;
        public const ushort C_OpenRoom = 11;
    }
}
