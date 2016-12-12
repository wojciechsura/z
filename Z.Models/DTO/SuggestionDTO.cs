using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Models.DTO
{
    public class SuggestionDTO
    {
        public SuggestionDTO(string display, string comment, string module, int index)
        {
            Display = display;
            Index = index;
            Comment = comment;
            Module = module;
        }

        public string Display { get; private set; }
        public string Comment { get; private set; }
        public string Module { get; private set; }
        public int Index { get; private set; }
    }
}
