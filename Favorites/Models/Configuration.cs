using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FavoritesModule.Models
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        [XmlArray("Favorites")]
        public List<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
