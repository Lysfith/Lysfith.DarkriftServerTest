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
            ClientManager.ClientConnected += ClientManager_ClientConnected;
            ClientManager.ClientDisconnected += ClientManager_ClientDisconnected;
        }

        private void ClientManager_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine($"ClientManager_ClientDisconnected");
        }

        void ClientManager_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            e.Client.MessageReceived += Client_MessageReceived;
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine($"MessageReceived: {e.Tag}");
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
                case NetworkTags.C_OpenRoom:
                    OpenRoomEvent(e);
                    break;
                case NetworkTags.C_CloseRoom:
                    CloseRoomEvent(e);
                    break;
            }
        }

        private void CreateRoomEvent(MessageReceivedEventArgs e)
        {
            if (_rooms.ContainsKey(e.Client.ID))
            {
                var r = _rooms[e.Client.ID];
                var roomResultAlreadyCreated = new CreateRoomResult(false, r.Code, "Room already created");
                using (var message = NetworkMessage.CreateMessage(roomResultAlreadyCreated))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
                return;
            }

            var createRoomMessage = CreateRoom.ReadMessageServer<CreateRoom>(e);

            var key = Room.GenerateKey(_roomKeys);
            var room = new Room(key, e.Client.ID, createRoomMessage.LocalIp, createRoomMessage.LocalPort, createRoomMessage.WanIp, createRoomMessage.WanPort);
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
                return;
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
            var joinRoomMessage = JoinRoom.ReadMessageServer<JoinRoom>(e);

            var room = _rooms.Values.Where(r => r.Code == joinRoomMessage.Code.ToUpperInvariant()).FirstOrDefault();

            if (room == null)
            {
                var roomResultNotFound = new JoinRoomError(e.Client.ID, "Room not found");
                using (var message = NetworkMessage.CreateMessage(roomResultNotFound))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
                return;
            }

            var result = room.CanAddPlayer(e.Client.ID);

            //Notify response
            if (result != null)
            {
                var roomResult = new JoinRoomError(e.Client.ID, result);
                using (var message = NetworkMessage.CreateMessage(roomResult))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
            }
            else
            {
                room.AddPlayer(e.Client.ID);

                var roomResult = new JoinRoomSuccess(e.Client.ID, room.LocalIp, room.LocalPort, room.WanIp, room.WanPort);

                using (var message = NetworkMessage.CreateMessage(roomResult))
                {
                    ClientManager.GetClient(room.HostId).SendMessage(message, SendMode.Reliable);
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
                return;
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
                    ClientManager.GetClient(room.HostId).SendMessage(message, SendMode.Reliable);
                    foreach (var p in room.Players)
                    {
                        ClientManager.GetClient(p).SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }

        private void OpenRoomEvent(MessageReceivedEventArgs e)
        {
            if (!_rooms.ContainsKey(e.Client.ID))
            {
                var roomResultNotFound = new DestroyRoomResult(false, "Room not found");
                using (var message = NetworkMessage.CreateMessage(roomResultNotFound))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
                return;
            }

            var room = _rooms[e.Client.ID];
            room.Open();
        }

        private void CloseRoomEvent(MessageReceivedEventArgs e)
        {
            if (!_rooms.ContainsKey(e.Client.ID))
            {
                var roomResultNotFound = new DestroyRoomResult(false, "Room not found");
                using (var message = NetworkMessage.CreateMessage(roomResultNotFound))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
                return;
            }

            var room = _rooms[e.Client.ID];
            room.Close();
        }
    }
}
