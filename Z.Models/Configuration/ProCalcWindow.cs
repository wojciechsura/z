using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Z.Models.Configuration
{
    public class ProCalcWindow
    {
        [XmlElement("RelativePosition")]
        public Point RelativePosition { get; set; }
    }
}
