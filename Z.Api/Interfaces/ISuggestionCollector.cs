using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Types;

namespace Z.Api.Interfaces
{
    public interface ISuggestionCollector
    {
        void AddSuggestion(SuggestionInfo suggestion);
    }
}
