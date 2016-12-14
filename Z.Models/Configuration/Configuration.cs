using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Z.Models.Configuration
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        [XmlElement("Hotkey")]
        public Hotkey Hotkey { get; set; } = new Hotkey();
    }
}
