using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Models;
using Z.BusinessLogic.ViewModels.Base;

namespace Z.BusinessLogic.ViewModels.Main
{
    public class ListViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private List<SuggestionViewModel> suggestions;
        private int selectedItemIndex;
        private IListWindowAccess listWindowAccess;
        private readonly IMainHandler handler;

        // Private methods ----------------------------------------------------

        private void HandleSuggestionsChanged()
        {
            SelectedItemIndex = -1;
        }

        private void HandleSelectedItemChanged()
        {           
            listWindowAccess.EnsureSelectedIsVisible();
        }

        // Public methods -----------------------------------------------------

        public ListViewModel(IMainHandler handler)
        {
            suggestions = null;
            selectedItemIndex = -1;
            this.handler = handler;
        }

        public void SelectPreviousSuggestion()
        {
            if (suggestions != null && suggestions.Any())
            {
                if (selectedItemIndex > 0)
                    SelectedItemIndex = selectedItemIndex - 1;
                else
                    SelectedItemIndex = suggestions.Count - 1;
            }
        }

        public void SelectNextSuggestion()
        {
            if (suggestions != null && suggestions.Any())
            {
                if (selectedItemIndex >= 0 && selectedItemIndex < suggestions.Count - 1)
                    SelectedItemIndex = selectedItemIndex + 1;
                else
                    SelectedItemIndex = 0;
            }
        }

        public void ListWindowEnterPressed()
        {
            handler.ExecuteCurrentAction();
        }

        public void ListDoubleClick()
        {
            handler.ExecuteCurrentAction();
        }

        // Public properties --------------------------------------------------

        public IListWindowAccess ListWindowAccess
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (listWindowAccess != null)
                    throw new InvalidOperationException("Access can be set only once!");

                listWindowAccess = value;
            }
        }

        public List<SuggestionViewModel> Suggestions
        {
            get => suggestions;
            set => Set(ref suggestions, () => Suggestions, value, HandleSuggestionsChanged);
        }

        public int SelectedItemIndex
        {
            get => selectedItemIndex;
            set
            {
                if (suggestions != null && (value < -1 || value >= suggestions.Count))
                    throw new ArgumentOutOfRangeException(nameof(value));

                Set(ref selectedItemIndex, () => SelectedItemIndex, value, HandleSelectedItemChanged);
            }
        }

        public SuggestionViewModel SelectedSuggestion => selectedItemIndex >= 0 ? suggestions[selectedItemIndex] : null;
    }
}
