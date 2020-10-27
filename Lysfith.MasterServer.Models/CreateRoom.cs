using DarkRift;
using DarkRift.Server;
using System;

namespace Lysfith.MasterServer.Models
{
    public class CreateRoom : NetworkMessage
    {
        public int LocalPort { get; set; }
        public int WanPort { get; set; }
        public string LocalIp { get; set; }
        public string WanIp { get; set; }

        public CreateRoom()
            :base(NetworkTags.C_CreateRoom)
        {

        }

        public CreateRoom(string localIp, int localPort, string wanIp, int wanPort)
            : base(NetworkTags.C_CreateRoom)
        {
            LocalPort = localPort;
            LocalIp = localIp;
            WanPort = wanPort;
            WanIp = wanIp;
        }

        public override void Deserialize(DeserializeEvent e)
        {
            LocalPort = e.Reader.ReadInt32();
            LocalIp = e.Reader.ReadString();
            WanPort = e.Reader.ReadInt32();
            WanIp = e.Reader.ReadString();
        }

        public override void Serialize(SerializeEvent e)
        {
            e.Writer.Write(LocalPort);
            e.Writer.Write(LocalIp);
            e.Writer.Write(WanPort);
            e.Writer.Write(WanIp);
        }
    }
}
