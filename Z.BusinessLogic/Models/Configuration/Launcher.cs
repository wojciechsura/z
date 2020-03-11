using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Z.BusinessLogic.Models.Configuration
{
    public class Launcher
    {
        [XmlArray("Items")]
        public List<LauncherShortcut> Items { get; set; } = new List<LauncherShortcut>();
    }
}
