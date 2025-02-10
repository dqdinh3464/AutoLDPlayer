using System.Xml.Serialization;

namespace Auto_LDPlayer.Models
{
    public class ScriptKey
    {
        [XmlElement("Key")]
        public string KEY
        {
            get;
            set;
        }

        [XmlElement("Value")]
        public string VALUE
        {
            get;
            set;
        }

        public ScriptKey()
        {
        }
    }
}