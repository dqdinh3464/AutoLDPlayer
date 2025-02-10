using System.Collections.Generic;
using System.Xml.Serialization;

namespace Auto_LDPlayer.Models.XML
{
    [XmlRoot("hierarchy")]
    public class Hierarchy
    {
        [XmlElement("node")]
        public List<Node> Node = new List<Node>();

        public Hierarchy()
        {
        }
    }
}
