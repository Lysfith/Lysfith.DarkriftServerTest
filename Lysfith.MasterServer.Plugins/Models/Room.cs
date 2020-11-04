using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lysfith.MasterServer.Plugins.Models
{
    public class Room
    {
        private const int PLAYER_LIMIT = 7;

        public bool IsOpen { get; private set; }
        public string Code { get; private set; }
        public int LocalPort { get; private set; }
        public string LocalIp { get; private set; }
        public int WanPort { get; private set; }
        public string WanIp { get; private set; }
        public ushort HostId { get; private set; }
        public List<ushort> Players => _players.ToList();
        private List<ushort> _players;

        public Room(string code, ushort hostId, string localIp, int localPort, string wanIp, int wanPort)
        {
            Code = code;
            HostId = hostId;
            LocalIp = localIp;
            LocalPort = localPort;
            WanIp = wanIp;
            WanPort = wanPort;

            IsOpen = true;
            _players = new List<ushort>();
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }

        public string CanAddPlayer(ushort playerId)
        {
            if (!IsOpen)
            {
                return "Room is closed";
            }

            if (_players.Count >= PLAYER_LIMIT)
            {
                return "Room full";
            }

            if (_players.Contains(playerId))
            {
                return "Already in room";
            };
            return null;
        }

        public bool AddPlayer(ushort playerId)
        {
            if(_players.Count >= PLAYER_LIMIT)
            {
                return false;
            }

            _players.Add(playerId);
            return true;
        }

        public bool RemovePlayer(ushort playerId)
        {
            if (!_players.Contains(playerId))
            {
                return false;
            }

            _players.Remove(playerId);
            return true;
        }

        public static string GenerateKey(IEnumerable<string> keys)
        {
            var key = Guid.NewGuid().ToString().Split('-').First().ToUpperInvariant();

            while(keys.Contains(key))
            {
                key = Guid.NewGuid().ToString().Split('-').First().ToUpperInvariant();
            }

            return key;
        }
    }
}
