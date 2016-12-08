using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Common.Types;

namespace Z.Common.Interfaces
{
    public interface ISuggestionCollector
    {
        void AddSuggestion(Suggestion suggestion);
    }
}
