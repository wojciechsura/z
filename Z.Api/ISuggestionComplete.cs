using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Types;

namespace Z.Api
{
    public interface IZSuggestionComplete
    {
        /// <summary>
        /// Checks, if suggestion can be completed, when user presses Tab key.
        /// </summary>
        /// <param name="suggestion">Suggestion selected by user</param>
        /// <returns>True, if suggestion can be completed, otherwise false.</returns>
        bool CanComplete(string action, SuggestionInfo suggestion);

        /// <summary>
        /// Generates text, which replaces current input
        /// </summary>
        /// <param name="suggestion"></param>
        /// <returns></returns>
        string Complete(string action, SuggestionInfo suggestion);
    }
}
