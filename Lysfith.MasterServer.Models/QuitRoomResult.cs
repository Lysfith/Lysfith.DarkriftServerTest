using DarkRift;
using DarkRift.Server;
using System;

namespace Lysfith.MasterServer.Models
{
    public class QuitRoomResult : NetworkMessage
    {
        public bool Result { get; set; }
        public string Message { get; set; }

        public QuitRoomResult()
            :base(NetworkTags.S_QuitRoomResult)
        {

        }

        public QuitRoomResult(bool result, string message)
            : base(NetworkTags.S_QuitRoomResult)
        {
            Result = result;
            Message = message;
        }

        public override void Deserialize(DeserializeEvent e)
        {
            Result = e.Reader.ReadBoolean();
            Message = e.Reader.ReadString();
        }

        public override void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Result);
            e.Writer.Write(Message);
        }
    }
}
