using Rocket.Core.Logging;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RestoreMonarchy.QueueBypasser.Utils
{
    internal class KnownPlayers
    {
        private HashSet<CSteamID> players;

        public KnownPlayers()
        {
            players = LoadExistingPlayers();
        }

        private static HashSet<CSteamID> LoadExistingPlayers()
        {
            HashSet<CSteamID> loaded = [];
            try
            {
                string location = GetPlayersDirectory();

                if (Directory.Exists(location))
                {
                    var scan = Directory.GetDirectories(location).Select(Path.GetFileName);
                    foreach (var i in scan)
                    {
                        if (i != null && i.Contains("_") && ulong.TryParse(i.Split('_')[0], out ulong num))
                        {
                            CSteamID playerId = new CSteamID(num);
                            if (!loaded.Contains(playerId))
                            {
                                loaded.Add(playerId);
                            }
                        }
                    }
                }                

                if (QueueBypasserPlugin.Instance.Configuration.Instance.IsLoggingEnabled)
                {
                    if (loaded.Count == 0)
                    {
                        Logger.Log("No players have been on the server yet");
                    } else
                    {
                        Logger.Log($"{loaded.Count} players have been on the server");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading known players: {ex.Message}");
            }

            return loaded;
        }

        private static string GetPlayersDirectory()
        {
            if (PlayerSavedata.hasSync)
            {
                return Path.Combine(UnturnedPaths.RootDirectory.FullName, "Sync");
            }

            string playersPath = Path.Combine(UnturnedPaths.RootDirectory.FullName, ServerSavedata.transformPath("Players"));
            return playersPath;
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