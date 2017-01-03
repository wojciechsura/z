using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.ViewModels.Interfaces;
using Z.Models;
using Z.Models.DTO;

namespace Z.BusinessLogic.ViewModels
{
    public class SuggestionChoiceViewModel
    {
        private ISuggestionChoiceWindowAccess access;

        private readonly List<SuggestionData> suggestionData;
        private readonly List<SuggestionDTO> suggestions;

        public SuggestionChoiceViewModel(List<SuggestionData> suggestions)
        {
            if (suggestions.Count < 2)
                throw new ArgumentException(nameof(suggestions));

            this.suggestionData = suggestions;

            this.suggestions = new List<SuggestionDTO>();
            for (int i = 0; i < suggestionData.Count; i++)
                this.suggestions.Add(new SuggestionDTO(suggestionData[i].Suggestion.Display,
                    suggestionData[i].Suggestion.Comment,
                    suggestionData[i].Module.DisplayName,
                    suggestionData[i].Suggestion.Image,
                    suggestionData[i].Suggestion.Match,
                    i));

            SelectedItemIndex = 0;
        }

        public IEnumerable<SuggestionDTO> Suggestions => suggestions;

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
