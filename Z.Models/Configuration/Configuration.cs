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
        [XmlElement("MainWindow")]
        public MainWindow MainWindow { get; set; } = new MainWindow();
        [XmlElement("Behavior")]
        public Behavior Behavior { get; set; } = new Behavior();
        [XmlElement("Keywords")]
        public Keywords Keywords { get; set; } = new Keywords();
    }
}
