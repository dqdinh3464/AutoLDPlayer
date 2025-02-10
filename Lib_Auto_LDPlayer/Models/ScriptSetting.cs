using System.Xml.Serialization;

namespace Auto_LDPlayer.Models
{
    [XmlRoot("ScriptSetting")]
    public class ScriptSetting
    {
        [XmlArray("Scriptes")]
        [XmlArrayItem("ScriptKey")]
        public ScriptKey[] SCRIPTKEY
        {
            get;
            set;
        }

        public ScriptSetting()
        {
        }
    }
}
