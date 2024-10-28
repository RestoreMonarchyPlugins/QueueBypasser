using Rocket.API;
using System.Xml.Serialization;

namespace RestoreMonarchy.QueueBypasser
{
    public class QueueBypasserConfiguration : IRocketPluginConfiguration
    {
        public bool EnableLogging { get; set; }
        public bool AdminsBypassQueue { get; set; }
        public bool NewPlayersBypassQueue { get; set; }
        public QueueBypassCooldownOptions PlayerRelogBypassQueue { get; set; }
        public void LoadDefaults()
        {
            EnableLogging = true;
            AdminsBypassQueue = true;
            NewPlayersBypassQueue = false;
            PlayerRelogBypassQueue = new QueueBypassCooldownOptions
            {
                Enabled = false,
                Cooldown = 900
            };
        }
    }
    public class QueueBypassCooldownOptions
    {
        [XmlAttribute]
        public bool Enabled { get; set; }
        [XmlAttribute]
        public int Cooldown { get; set; }
    }
}
