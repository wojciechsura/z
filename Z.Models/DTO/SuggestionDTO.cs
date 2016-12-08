using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Models
{
    public class SuggestionDTO
    {
        public SuggestionDTO(string display, int index)
        {
            Display = display;
            Index = index;
        }

        public string Display { get; private set; }
        public int Index { get; private set; }
    }
}
