using DarkRift;
using DarkRift.Server;
using System;

namespace Lysfith.MasterServer.Models
{
    public class JoinRoomError : NetworkMessage
    {
        public ushort PlayerId { get; set; }
        public string Message { get; set; }

        public JoinRoomError()
            :base(NetworkTags.S_JoinRoomError)
        {

        }

        public JoinRoomError(ushort playerId, string message)
            : base(NetworkTags.S_JoinRoomError)
        {
            Message = message;
            PlayerId = playerId;
        }

        public override void Deserialize(DeserializeEvent e)
        {
            PlayerId = e.Reader.ReadUInt16();
            Message = e.Reader.ReadString();
        }

        public override void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerId);
            e.Writer.Write(Message);
        }
    }
}
