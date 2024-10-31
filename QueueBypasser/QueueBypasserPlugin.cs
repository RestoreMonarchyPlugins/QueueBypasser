using RestoreMonarchy.QueueBypasser.Utils;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RestoreMonarchy.QueueBypasser
{
    public class QueueBypasserPlugin : RocketPlugin<QueueBypasserConfiguration>
    {
        public static QueueBypasserPlugin Instance { get; private set; }

        private Timer checker;
        private HashSet<CSteamID> playersFromQueue;
        private KnownPlayers knownPlayers;
        private PlayerRelogs playersRelogs;

        protected override void Load()
        {
            Instance = this;
            checker = new Timer(state => Check(), null, 0, 1000);
            playersFromQueue = [];

            Provider.onEnemyConnected += OnConnectedMaxPlayersBack;
            Provider.onRejectingPlayer += OnRejectingPlayer;

            if (Configuration.Instance.ShouldBypassQueueForNewPlayers)
            {
                knownPlayers = new KnownPlayers();
                Provider.onEnemyConnected += OnConnected;
            }

            if (Configuration.Instance.RelogBypassSettings.Enabled)
            {
                playersRelogs = new PlayerRelogs(Configuration.Instance.RelogBypassSettings.Cooldown);
                Provider.onEnemyDisconnected += OnDisconnected;
            }

            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
            Logger.Log($"Check out more Unturned plugins at restoremonarchy.com");
        }

        protected override void Unload()
        {
            Instance = null;
            checker.Change(Timeout.Infinite, Timeout.Infinite); // Stops timer
            checker.Dispose();
            checker = null;
            playersFromQueue = null;

            Provider.onEnemyConnected -= OnConnectedMaxPlayersBack;
            Provider.onRejectingPlayer -= OnRejectingPlayer;

            if (Configuration.Instance.ShouldBypassQueueForNewPlayers)
            {
                Provider.onEnemyConnected -= OnConnected;
                knownPlayers = null;
            }

            if (Configuration.Instance.RelogBypassSettings.Enabled)
            {
                Provider.onEnemyDisconnected -= OnDisconnected;
                playersRelogs = null;
            }

            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        private void OnConnectedMaxPlayersBack(SteamPlayer player)
        {
            MaxPlayersBack(player.playerID.steamID);
        }

        private void OnRejectingPlayer(CSteamID steamID, ESteamRejection rejection, string explanation)
        {
            MaxPlayersBack(steamID);
        }

        private void MaxPlayersBack(CSteamID playerId)
        {
            if (playersFromQueue.Contains(playerId))
            {
                Provider.maxPlayers--;
                playersFromQueue.Remove(playerId);
            }
        }

        private void OnDisconnected(SteamPlayer player)
        {
            CSteamID playerId = player.playerID.steamID;
            playersRelogs.Add(playerId);
        }

        private void OnConnected(SteamPlayer player)
        {
            CSteamID playerId = player.playerID.steamID;
            if (!knownPlayers.Contains(playerId))
            {
                knownPlayers.Add(playerId);
            }
        }

        private void Check()
        {
            if (Provider.clients.Count >= Provider.maxPlayers && Provider.pending.Count > 0)
            {
                List<SteamPending> pending = Provider.pending;

                for (int i = 0; i < pending.Count; i++)
                {
                    SteamPending player = pending[i];
                    string acceptReason = ResolveAcceptReason(player);

                    if (acceptReason != null)
                    {
                        if (Configuration.Instance.IsLoggingEnabled) 
                        {
                            Logger.Log($"Player {player.playerID.characterName} skipped the queue: {acceptReason}.");
                        }

                        playersFromQueue.Add(player.playerID.steamID);
                        Provider.maxPlayers++;
                        player.sendVerifyPacket();
                    }
                }
            }
        }

        private string ResolveAcceptReason(SteamPending pending)
        {
            CSteamID steamId = pending.playerID.steamID;
            RocketPlayer player = new(steamId.ToString(), null, SteamAdminlist.checkAdmin(steamId));

            bool newPlayerPass = Configuration.Instance.ShouldBypassQueueForNewPlayers && !knownPlayers.Contains(steamId);
            if (newPlayerPass) 
            {
                return "New player";
            }

            bool playerJustLeft = Configuration.Instance.RelogBypassSettings.Enabled && playersRelogs.Contains(steamId);
            if (playerJustLeft)
            {
                playersRelogs.Remove(steamId);
                return "Player relogged";
            }

            bool adminPass = Configuration.Instance.ShouldBypassQueueForAdmins && player.IsAdmin;
            if (adminPass)
            {
                return "Admin player";
            }

            bool hasPermission = R.Permissions.HasPermission(player, "queuebypasser");
            if (hasPermission)
            {
                return "Player has 'queuebypasser' permission";
            }

            return null;
        }
    }
}
