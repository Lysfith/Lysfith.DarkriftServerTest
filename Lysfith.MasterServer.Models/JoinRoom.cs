using DarkRift;
using DarkRift.Server;
using System;

namespace Lysfith.MasterServer.Models
{
    public class JoinRoom : NetworkMessage
    {
        public string Code { get; set; }

        public JoinRoom()
            :base(NetworkTags.C_JoinRoom)
        {

        }

        public JoinRoom(string code)
            : base(NetworkTags.C_JoinRoom)
        {
            Code = code;
        }

        public override void Deserialize(DeserializeEvent e)
        {
            Code = e.Reader.ReadString();
        }

        public override void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Code);
        }
    }
}
