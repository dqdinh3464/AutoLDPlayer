using System.Xml.Serialization;

namespace Auto_LDPlayer.Models.XML
{
    public class Node
    {
        [XmlAttribute("bounds")]
        public string Bounds
        {
            get;
            set;
        }

        [XmlAttribute("class")]
        public string Class
        {
            get;
            set;
        }

        [XmlAttribute("clickable")]
        public string Clickable
        {
            get;
            set;
        }

        [XmlAttribute("content-desc")]
        public string ContentDesc
        {
            get;
            set;
        }

        [XmlAttribute("checkable")]
        public string Checkable
        {
            get;
            set;
        }

        [XmlAttribute("checked")]
        public string Checked
        {
            get;
            set;
        }

        [XmlAttribute("enabled")]
        public string Enabled
        {
            get;
            set;
        }

        [XmlAttribute("focusable")]
        public string Focusable
        {
            get;
            set;
        }

        [XmlAttribute("focused")]
        public string Focused
        {
            get;
            set;
        }

        [XmlAttribute("index")]
        public string Index
        {
            get;
            set;
        }

        [XmlAttribute("long-clickable")]
        public string LongClickable
        {
            get;
            set;
        }

        [XmlAttribute("package")]
        public string Package
        {
            get;
            set;
        }

        [XmlAttribute("password")]
        public string Password
        {
            get;
            set;
        }

        [XmlAttribute("resource-id")]
        public string ResourceId
        {
            get;
            set;
        }

        [XmlAttribute("scrollable")]
        public string Scrollable
        {
            get;
            set;
        }

        [XmlAttribute("selected")]
        public string Selected
        {
            get;
            set;
        }

        [XmlAttribute("text")]
        public string Text
        {
            get;
            set;
        }

        public Node()
        {
        }
    }
}
