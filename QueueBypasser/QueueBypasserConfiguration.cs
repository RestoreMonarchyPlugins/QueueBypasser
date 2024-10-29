using RestoreMonarchy.QueueBypasser.Models;
using Rocket.API;

namespace RestoreMonarchy.QueueBypasser
{
    public class QueueBypasserConfiguration : IRocketPluginConfiguration
    {
        public bool IsLoggingEnabled { get; set; }
        public bool ShouldBypassQueueForAdmins { get; set; }
        public bool ShouldBypassQueueForNewPlayers { get; set; }
        public QueueBypassCooldownOptions RelogBypassSettings { get; set; }

        public void LoadDefaults()
        {
            IsLoggingEnabled = true;
            ShouldBypassQueueForAdmins = true;
            ShouldBypassQueueForNewPlayers = false;
            RelogBypassSettings = new QueueBypassCooldownOptions
            {
                Enabled = false,
                Cooldown = 900
            };
        }
    }
}
