using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Z.Models.Configuration
{
    public class MainWindow
    {
        [XmlElement("Position")]
        public Point Position { get; set; }        
    }
}
