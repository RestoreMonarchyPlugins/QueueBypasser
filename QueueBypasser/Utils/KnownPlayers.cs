using Rocket.Core.Logging;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RestoreMonarchy.QueueBypasser.Utils
{
    internal class KnownPlayers
    {
        private HashSet<CSteamID> players = LoadExistingPlayers();

        private static HashSet<CSteamID> LoadExistingPlayers()
        {
            string location = (PlayerSavedata.hasSync ? "/Sync/" : UnturnedPaths.RootDirectory.FullName + ServerSavedata.transformPath("/Players/"));
            HashSet<CSteamID> loaded = new();

            var scan = Directory.GetDirectories(location).Select(Path.GetFileName);

            foreach (var i in scan)
            {
                if (ulong.TryParse(i.Split("_".ToCharArray())[0], out ulong num))
                {
                    CSteamID playerId = new CSteamID(num);
                    if (!loaded.Contains(playerId))
                    {
                        loaded.Add(playerId);
                    }
                }
            }

            if (QueueBypasserPlugin.Instance.Configuration.Instance.IsLoggingEnabled)
            {
                if (loaded.Count == 0)
                {
                    Logger.Log($"No players were been on the server yet");
                }
                else
                {
                    Logger.Log($"{loaded.Count} players were been already on the server");
                }
            }

            return loaded;
        }

        public bool Add(CSteamID playerId)
        {
            return players.Add(playerId);
        }

        public bool Contains(CSteamID playerId)
        {
            return players.Contains(playerId);
        }
    }
}
