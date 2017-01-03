using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Z.Models.DTO
{
    public class SuggestionDTO
    {
        public SuggestionDTO(string display, string comment, string module, ImageSource image, byte match, int index)
        {
            Display = display;
            Index = index;
            Comment = comment;
            Module = module;
            Match = match;
            Image = image;
        }

        public string Display { get; private set; }
        public string Comment { get; private set; }
        public string Module { get; private set; }
        public int Index { get; private set; }
        public byte Match { get; private set; }
        public ImageSource Image { get; private set; }
    }
}
