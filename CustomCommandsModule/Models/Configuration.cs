using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CustomCommandsModule.Models
{
    [XmlRoot("Configuration")]
    class Configuration
    {
        [XmlArray("Commands")]
        public List<CustomCommand> Commands { get; set; } = new List<CustomCommand>
        {
            new CustomCommand
            {
                Key = "test",
                Command = "http://www.google.com#q={u*}",
                CommandKind = CommandKinds.Url,
                Comment = "Test command"
            }
        };
    }
}
