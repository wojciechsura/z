using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Z.Common.Types;

namespace Z.Models.Configuration
{    
    public class Hotkey
    {
        [XmlElement("KeyModifier")]
        public KeyModifier KeyModifier { get; set; } = KeyModifier.Alt;
        [XmlElement("Key")]
        public Key Key { get; set; } = Key.Space;
        [XmlElement("HotkeySwitchesVisibility")]
        public bool HotkeySwitchesVisibility { get; set; } = false;
    }
}
