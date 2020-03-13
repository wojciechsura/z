using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;
using Z.BusinessLogic.Types.Launcher;

namespace Z.BusinessLogic.Models.Configuration
{
    public class LauncherShortcut
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Command")]
        public string Command { get; set; }
        [XmlElement("IconData")]
        public string IconData { get; set; } = null;
        [XmlElement("IconMode")]
        public IconMode IconMode { get; set; } = IconMode.Default;
        [XmlArray("SubItems")]
        public List<LauncherShortcut> SubItems { get; set; } = new List<LauncherShortcut>();
    }
}
