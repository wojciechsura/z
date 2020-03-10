using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Models;

namespace Z.BusinessLogic.ViewModels.Main
{
    public class SuggestionChoiceViewModel
    {
        private ISuggestionChoiceWindowAccess access;

        private readonly List<SuggestionData> suggestionData;
        private readonly List<SuggestionViewModel> suggestions;

        public SuggestionChoiceViewModel(List<SuggestionViewModel> suggestions)
        {
            if (suggestions.Count < 2)
                throw new ArgumentException(nameof(suggestions));

            this.suggestions = suggestions;

            SelectedItemIndex = 0;
        }

        public IReadOnlyCollection<SuggestionViewModel> Suggestions => suggestions;

        public void EnterPressed()
        {
            access.CloseWindow(true);
        }

        public void EscapePressed()
        {
            access.CloseWindow(false);
        }

        public int SelectedItemIndex { get; set; }

        public ISuggestionChoiceWindowAccess SuggestionChoiceWindowAccess
        {
            set
            {
                if (access != null)
                    throw new InvalidOperationException("Access can be set only once!");
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                access = value;
            }
        }
    }
}
