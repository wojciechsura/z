using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectsModule.Models
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        [XmlArray("ProjectRoots")]
        public List<string> ProjectRoots { get; set; } = new List<string>();
    }
}
