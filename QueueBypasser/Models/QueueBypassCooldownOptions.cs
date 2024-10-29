using System.Xml.Serialization;

namespace RestoreMonarchy.QueueBypasser.Models
{
    public class QueueBypassCooldownOptions
    {
        [XmlAttribute]
        public bool Enabled { get; set; }
        [XmlAttribute]
        public int Cooldown { get; set; }
    }
}
