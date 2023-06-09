﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CustomCommandsModule.Models
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        [XmlArray("Commands")]
        public List<CustomCommand> Commands { get; set; } = new List<CustomCommand>();
    }
}
