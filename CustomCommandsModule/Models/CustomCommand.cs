using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CustomCommandsModule.Models
{   
    public class CustomCommand
    {
        [XmlElement("Key")]
        public string Key { get; set; }
        [XmlElement("Comment")]
        public string Comment { get; set; }
        [XmlElement("Command")]
        public string Command { get; set; }
        [XmlElement("CommandKind")]
        public CommandKinds CommandKind { get; set; }
    }
}
