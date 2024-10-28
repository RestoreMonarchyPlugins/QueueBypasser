using RestoreMonarchy.QueueBypasser.Util;
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
    public class QueueBypasserPlugin : RocketPlugin<Config>
    {
        internal static QueueBypasserPlugin Instance;
        internal Timer checker;
        internal HashSet<CSteamID> playersFromQueue;
        internal KnownPlayers knownPlayers;
        internal PlayerRelogs playersRelogs;
        protected override void Load()
        {
            Instance = this;
            checker = new Timer(state => Check(), null, 0, 1000);
            playersFromQueue = new HashSet<CSteamID>();

            Provider.onCheckBanStatusWithHWID += OnCheckBanStatusWithHWID;
            Provider.onRejectingPlayer += OnRejectingPlayer;

            if (Configuration.Instance.newPlayersBypassQueue)
            {
                knownPlayers = new KnownPlayers();
                Provider.onEnemyConnected += OnConnected;
            }

            if (Configuration.Instance.playerRelogBypassQueue.enabled)
            {
                playersRelogs = new PlayerRelogs(Configuration.Instance.playerRelogBypassQueue.cooldown);
                Provider.onEnemyDisconnected += OnDisconnected;
            }

            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }
        protected override void Unload()
        {
            Instance = null;
            checker.Change(Timeout.Infinite, Timeout.Infinite); // Stops timer
            checker.Dispose();
            checker = null;
            playersFromQueue = null;

            Provider.onCheckBanStatusWithHWID -= OnCheckBanStatusWithHWID;
            Provider.onRejectingPlayer -= OnRejectingPlayer;

            if (Configuration.Instance.newPlayersBypassQueue)
            {
                Provider.onEnemyConnected -= OnConnected;
                knownPlayers = null;
            }

            if (Configuration.Instance.playerRelogBypassQueue.enabled)
            {
                Provider.onEnemyDisconnected -= OnDisconnected;
                playersRelogs = null;
            }

            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }
        private void OnCheckBanStatusWithHWID(SteamPlayerID playerID, uint remoteIP, ref bool isBanned, ref string banReason, ref uint banRemainingDuration)
        {
            MaxPlayersBack(playerID.steamID);
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
            var playerId = player.playerID.steamID;
            playersRelogs.Add(playerId);
        }
        private void OnConnected(SteamPlayer player)
        {
            var playerId = player.playerID.steamID;
            if (!knownPlayers.Contains(playerId)) knownPlayers.Add(playerId);
        }
        private void Check()
        {
            if (Provider.clients.Count >= Provider.maxPlayers && Provider.pending.Count > 0)
            {
                var pending = Provider.pending;

                for (int i = 0; i < pending.Count; i++)
                {
                    var player = pending[i];
                    string acceptReason = ResolveAcceptReason(player);

                    if (acceptReason != null)
                    {
                        if (Configuration.Instance.enableLogging) Logger.Log($"Player {player.playerID.characterName} skipped queue, Reason: {acceptReason}");
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
            RocketPlayer player = new RocketPlayer(steamId.ToString(), null, SteamAdminlist.checkAdmin(steamId));

            bool newPlayerPass = Configuration.Instance.newPlayersBypassQueue && !knownPlayers.Contains(steamId);
            if (newPlayerPass) return "player is a new player";

            bool playerJustLeft = Configuration.Instance.playerRelogBypassQueue.enabled && playersRelogs.Contains(steamId);
            if (playerJustLeft)
            {
                playersRelogs.Remove(steamId);
                return "player relogged";
            }

            bool adminPass = Configuration.Instance.adminsBypassQueue && player.IsAdmin;
            if (adminPass) return "player is admin";

            bool hasPermission = R.Permissions.HasPermission(player, "queuebypasser");
            if (hasPermission) return "player has permission";

            return null;
        }
    }
}
