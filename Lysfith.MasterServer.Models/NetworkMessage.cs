using DarkRift;
using DarkRift.Server;
using System;

namespace Lysfith.MasterServer.Models
{
    public class NetworkMessage : IDarkRiftSerializable
    {
        public readonly ushort TAG;

        public NetworkMessage()
        {

        }

        public NetworkMessage(ushort tag)
        {
            TAG = tag;
        }

        public virtual void Deserialize(DeserializeEvent e)
        {

        }

        public virtual void Serialize(SerializeEvent e)
        {

        }

        public static T ReadMessage<T>(MessageReceivedEventArgs e) where T : NetworkMessage, new()
        {
            using (var message = e.GetMessage())
            using (var reader = message.GetReader())
            {
                return reader.ReadSerializable<T>();
            }
        }

        public static Message CreateMessage<T>(T t) where T : NetworkMessage
        {
            using (var playerWriter = DarkRiftWriter.Create())
            {
                playerWriter.Write(t);

                return Message.Create(t.TAG, playerWriter);
            }
        }
    }
}
