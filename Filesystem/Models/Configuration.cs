using Filesystem.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Filesystem.Models
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        [XmlElement("FileSearchStrategy")]
        public FileSearchStrategy FileSearchStrategy { get; set; } = FileSearchStrategy.Direct;
    }
}
