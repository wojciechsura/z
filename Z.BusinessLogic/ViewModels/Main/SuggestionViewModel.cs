using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Z.BusinessLogic.Models;

namespace Z.BusinessLogic.ViewModels.Main
{
    public class SuggestionViewModel
    {
        private SuggestionData suggestionData;

        public SuggestionViewModel(SuggestionData suggestionData)
        {
            this.suggestionData = suggestionData;
        }

        public string Display => suggestionData.Suggestion.Display;
        public string Comment => suggestionData.Suggestion.Comment;
        public string Module => suggestionData.Module.DisplayName;
        public byte Match => suggestionData.Suggestion.Match;
        public ImageSource Image => suggestionData.Suggestion.Image;
        public SuggestionData SuggestionData => suggestionData;
    }
}
