using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Api.Utils
{
    public static class SuggestionUtils
    {
        public static byte EvalMatch(string enteredText, string suggestion)
        {
            if (!suggestion.ToUpper().Contains(enteredText.ToUpper()))
                return 0;

            return (byte)(100 * enteredText.Length / suggestion.Length);
        }

        public static byte EvalMatch(string enteredText, params string[] suggestions)
        {
            return suggestions
                .Select(s => EvalMatch(enteredText, s))
                .Max();
        }
    }
}
