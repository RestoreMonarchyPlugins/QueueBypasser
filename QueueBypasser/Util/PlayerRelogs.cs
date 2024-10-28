using Steamworks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestoreMonarchy.QueueBypasser.Util
{
    internal class PlayerRelogs(int seconds)
    {
        private HashSet<CSteamID> players = new HashSet<CSteamID>();
        private int cooldown = seconds;
        public async void Add(CSteamID playerId)
        {
            if (players.Add(playerId))
            {
                await Task.Delay(cooldown * 1000);
                players.Remove(playerId);
            }
        }
        public bool Contains(CSteamID playerId)
        {
            return players.Contains(playerId);
        }
        public bool Remove(CSteamID playerId)
        {
            return players.Remove(playerId);
        }
    }
}
