using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;

namespace Z.Models
{
    public class SuggestionData
    {
        public SuggestionData(SuggestionInfo suggestion, IZModule module)
        {
            this.Suggestion = suggestion;
            this.Module = module;
        }

        public SuggestionInfo Suggestion { get; private set; }
        public IZModule Module { get; private set; }
    }
}
