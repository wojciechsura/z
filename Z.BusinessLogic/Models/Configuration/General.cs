using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Z.BusinessLogic.Models.Configuration
{
    public class General
    {
        [XmlElement("EnterBehavior")]
        public bool SynchronizeWindowPositions { get; set; } = false;
    }
}
