using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Z.BusinessLogic.Models.Configuration
{
    public class LauncherShortcut
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Command")]
        public string Command { get; set; }
        [XmlElement("Icon")]
        public string Base64Icon { get; set; }
        [XmlArray("SubItems")]
        public List<LauncherShortcut> SubItems { get; set; } = new List<LauncherShortcut>();
    }
}
