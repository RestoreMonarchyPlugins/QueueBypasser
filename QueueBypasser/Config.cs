using Rocket.API;

namespace RestoreMonarchy.QueueBypasser
{
    public class Config : IRocketPluginConfiguration
    {
        public bool enableLogging;
        public bool adminsBypassQueue;
        public bool newPlayersBypassQueue;
        public QueueBypassCooldownOptions playerRelogBypassQueue;
        public void LoadDefaults()
        {
            enableLogging = true;
            adminsBypassQueue = true;
            newPlayersBypassQueue = false;
            playerRelogBypassQueue = new QueueBypassCooldownOptions
            {
                enabled = false,
                cooldown = 900
            };
        }
    }
    public class QueueBypassCooldownOptions
    {
        public bool enabled;
        public int cooldown;
    }
}
