using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Z.Models.Configuration
{
    public class Launcher
    {
        [XmlElement("Root")]
        public LauncherShortcut Root { get; set; }
    }
}
