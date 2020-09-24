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

        public string Code { get; private set; }
        public ushort Host { get; private set; }
        public List<ushort> Players => _players.ToList();
        private List<ushort> _players;

        public Room(string code, ushort host)
        {
            Code = code;
            Host = host;
            _players = new List<ushort>();
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
