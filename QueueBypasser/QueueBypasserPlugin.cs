using Rocket.API;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.QueueBypasser
{
    public class QueueBypasserPlugin : RocketPlugin
    {
        protected override void Load()
        {
            InvokeRepeating("Check", 1, 1);
            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()    
        {
            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        void Check()
        {
            foreach (var pending in Provider.pending.ToList())
            {
                if (R.Permissions.HasPermission(new RocketPlayer(pending.playerID.steamID.ToString()), "queuebypasser"))
                {
                    if (!pending.hasSentVerifyPacket)
                    {
                        pending.sendVerifyPacket();
                    } 
                }
            }
        }
    }
}
