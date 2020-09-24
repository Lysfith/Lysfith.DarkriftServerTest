using DarkRift;
using DarkRift.Server;
using System;

namespace Lysfith.MasterServer.Models
{
    public class CreateRoomResult : NetworkMessage
    {
        public string Code { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }

        public CreateRoomResult()
            :base(NetworkTags.S_CreateRoomResult)
        {

        }

        public CreateRoomResult(bool result, string code, string message)
            : base(NetworkTags.S_CreateRoomResult)
        {
            Code = code;
            Result = result;
            Message = message;
        }

        public override void Deserialize(DeserializeEvent e)
        {
            Result = e.Reader.ReadBoolean();
            Code = e.Reader.ReadString();
            Message = e.Reader.ReadString();
        }

        public override void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Result);
            e.Writer.Write(Code);
            e.Writer.Write(Message);
        }
    }
}
