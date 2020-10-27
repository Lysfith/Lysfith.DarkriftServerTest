using DarkRift;
using DarkRift.Server;
using System;

namespace Lysfith.MasterServer.Models
{
    public class JoinRoomSuccess : NetworkMessage
    {
        public ushort PlayerId { get; set; }
        public string HostLocalIp { get; set; }
        public int HostLocalPort { get; set; }
        public string HostWanIp { get; set; }
        public int HostWanPort { get; set; }

        public JoinRoomSuccess()
            : base(NetworkTags.S_JoinRoomError)
        {

        }

        public JoinRoomSuccess(ushort playerId, string hostLocalIp, int hostLocalPort, string hostWanIp, int hostWanPort)
            : base(NetworkTags.S_JoinRoomError)
        {
            HostLocalIp = hostLocalIp;
            HostLocalPort = hostLocalPort;
            HostWanIp = hostWanIp;
            HostWanPort = hostWanPort;
            PlayerId = playerId;
        }

        public override void Deserialize(DeserializeEvent e)
        {
            HostLocalIp = e.Reader.ReadString();
            HostLocalPort = e.Reader.ReadInt32();
            HostWanIp = e.Reader.ReadString();
            HostWanPort = e.Reader.ReadInt32();
            PlayerId = e.Reader.ReadUInt16();
        }

        public override void Serialize(SerializeEvent e)
        {
            e.Writer.Write(HostLocalIp);
            e.Writer.Write(HostLocalPort);
            e.Writer.Write(HostWanIp);
            e.Writer.Write(HostWanPort);
            e.Writer.Write(PlayerId);
        }
    }
}
