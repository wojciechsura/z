using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FavoritesModule.Models
{   
    public class Favorite
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Comment")]
        public string Comment { get; set; }
        [XmlElement("Command")]
        public string Command { get; set; }
    }
}
