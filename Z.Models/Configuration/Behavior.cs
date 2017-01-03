using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Z.Common.Types;

namespace Z.Models.Configuration
{

    public class Behavior
    {
        private const int DEFAULT_SUGGESTION_DELAY = 200;

        [XmlElement("EnterBehavior")]
        public EnterBehavior EnterBehavior { get; set; } = EnterBehavior.ShellExecute;
        [XmlElement("SuggestionSorting")]
        public SuggestionSorting SuggestionSorting { get; set; } = SuggestionSorting.ByModule;
        [XmlElement("SuggestionDelay")]
        public int SuggestionDelay { get; set; } = DEFAULT_SUGGESTION_DELAY;
    }
}
