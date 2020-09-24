using DarkRift;
using DarkRift.Server;
using Lysfith.MasterServer.Models;
using Lysfith.MasterServer.Plugins.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lysfith.MasterServer.Plugins
{
    public class RoomManager : Plugin
    {
        public override bool ThreadSafe => true;

        public override Version Version => new Version(1, 0, 0);

        private Dictionary<ushort, Room> _rooms = new Dictionary<ushort, Room>();
        private List<string> _roomKeys = new List<string>();

        public RoomManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {

        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch (e.Tag)
            {
                case NetworkTags.C_CreateRoom:
                    CreateRoomEvent(e);
                    break;
                case NetworkTags.C_DestroyRoom:
                    DestroyRoomEvent(e);
                    break;
                case NetworkTags.C_JoinRoom:
                    JoinRoomEvent(e);
                    break;
                case NetworkTags.C_QuitRoom:
                    QuitRoomEvent(e);
                    break;
            }
        }

        private void CreateRoomEvent(MessageReceivedEventArgs e)
        {
            if (_rooms.ContainsKey(e.Client.ID))
            {
                var roomResultAlreadyCreated = new CreateRoomResult(false, string.Empty, "Room already created");
                using (var message = NetworkMessage.CreateMessage(roomResultAlreadyCreated))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
            }

            var key = Room.GenerateKey(_roomKeys);
            var room = new Room(key, e.Client.ID);
            _rooms.Add(e.Client.ID, room);

            //Notify response
            var roomResultSuccess = new CreateRoomResult(true, key, string.Empty);
            using (var message = NetworkMessage.CreateMessage(roomResultSuccess))
            {
                e.Client.SendMessage(message, SendMode.Reliable);
            }
        }

        private void DestroyRoomEvent(MessageReceivedEventArgs e)
        {
            if (!_rooms.ContainsKey(e.Client.ID))
            {
                var roomResultNotFound = new DestroyRoomResult(false, "Room not found");
                using (var message = NetworkMessage.CreateMessage(roomResultNotFound))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
            }

            var roomToDestroy = _rooms[e.Client.ID];
            _rooms.Remove(e.Client.ID);

            //Notify response
            var roomResultDestroySuccess = new DestroyRoomResult(true, string.Empty);
            using (var message = NetworkMessage.CreateMessage(roomResultDestroySuccess))
            {
                e.Client.SendMessage(message, SendMode.Reliable);

            }

            using (Message playerMessage = Message.CreateEmpty(NetworkTags.S_ExcludeFromRoom))
            {
                foreach (var p in roomToDestroy.Players)
                {
                    ClientManager.GetClient(p).SendMessage(playerMessage, SendMode.Reliable);
                }
            }
        }

        private void JoinRoomEvent(MessageReceivedEventArgs e)
        {
            var joinRoomMessage = JoinRoom.ReadMessage<JoinRoom>(e);

            var room = _rooms.Values.Where(r => r.Code == joinRoomMessage.Code.ToUpperInvariant()).FirstOrDefault();

            if (room == null)
            {
                var roomResultNotFound = new JoinRoomResult(false, "Room not found");
                using (var message = NetworkMessage.CreateMessage(roomResultNotFound))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
            }

            var result = room.AddPlayer(e.Client.ID);

            //Notify response
            JoinRoomResult roomResult = null;
            if (!result)
            {
                roomResult = new JoinRoomResult(false, "Room full");
                using (var message = NetworkMessage.CreateMessage(roomResult))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
            }
            else
            {
                roomResult = new JoinRoomResult(true, string.Empty);

                using (var message = NetworkMessage.CreateMessage(roomResult))
                {
                    ClientManager.GetClient(room.Host).SendMessage(message, SendMode.Reliable);
                    foreach (var p in room.Players)
                    {
                        ClientManager.GetClient(p).SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }

        private void QuitRoomEvent(MessageReceivedEventArgs e)
        {
            var room = _rooms.Values.Where(r => r.Players.Contains(e.Client.ID)).FirstOrDefault();

            if (room == null)
            {
                var roomResultNotFound = new QuitRoomResult(false, "Room not found");
                using (var message = NetworkMessage.CreateMessage(roomResultNotFound))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
            }

            var result = room.RemovePlayer(e.Client.ID);

            //Notify response
            QuitRoomResult roomResult = null;
            if (!result)
            {
                roomResult = new QuitRoomResult(false, "You are not in the room");
                using (var message = NetworkMessage.CreateMessage(roomResult))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
            }
            else
            {
                roomResult = new QuitRoomResult(true, string.Empty);

                using (var message = NetworkMessage.CreateMessage(roomResult))
                {
                    ClientManager.GetClient(room.Host).SendMessage(message, SendMode.Reliable);
                    foreach (var p in room.Players)
                    {
                        ClientManager.GetClient(p).SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }
    }
}
