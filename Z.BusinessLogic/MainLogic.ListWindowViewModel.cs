using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Interfaces.ViewModels;
using Z.Models.DTO;

namespace Z.BusinessLogic
{
    partial class MainLogic
    {
        class ListWindowViewModelImplementation : INotifyPropertyChanged, IListWindowViewModel
        {
            // Private fields -------------------------------------------------

            private IListWindowAccess access;
            private MainLogic logic;

            private List<SuggestionDTO> suggestions;
            private int selectedItemIndex;

            // Private methods ------------------------------------------------

            private void Safe(Action<IListWindowAccess> action)
            {
                if (access != null)
                    action(access);
            }

            private void SetSuggestions(List<SuggestionDTO> value)
            {
                suggestions = value;
                OnPropertyChanged(nameof(IListWindowViewModel.Suggestions));
                SetSelectedItemIndex(-1);
            }

            private void SetSelectedItemIndex(int index)
            {
                if (suggestions != null && (index < -1 || index >= suggestions.Count))
                    throw new ArgumentOutOfRangeException(nameof(index));

                selectedItemIndex = index;
                OnPropertyChanged(nameof(IListWindowViewModel.SelectedItemIndex));

                Safe(access => access.EnsureSelectedIsVisible());                
            }

            private void OnSelectedItemIndexChanged()
            {
                logic.SelectedSuggestionChanged();
            }

            // Protected methods ----------------------------------------------

            protected void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

            // Internal methods -----------------------------------------------

            internal void SelectPreviousSuggestion()
            {
                if (suggestions != null && suggestions.Count() > 0)
                {
                    if (selectedItemIndex > 0)
                        SetSelectedItemIndex(selectedItemIndex - 1);
                    else
                        SetSelectedItemIndex(suggestions.Count() - 1);
                    OnSelectedItemIndexChanged();
                }
            }

            internal void SelectNextSuggestion()
            {
                if (suggestions != null && suggestions.Count() > 0)
                {
                    if (selectedItemIndex >= 0 && selectedItemIndex < suggestions.Count() - 1)
                        SetSelectedItemIndex(selectedItemIndex + 1);
                    else
                        SetSelectedItemIndex(0);
                    OnSelectedItemIndexChanged();
                }
            }

            internal SuggestionDTO SelectedSuggestion
            {
                get
                {
                    if (selectedItemIndex < 0)
                        return null;
                    else
                        return suggestions[selectedItemIndex];
                }
            }

            // Internal properties --------------------------------------------

            internal List<SuggestionDTO> InternalSuggestions
            {
                set
                {
                    SetSuggestions(value);
                }
            }

            // Public methods -------------------------------------------------

            public ListWindowViewModelImplementation(MainLogic logic)
            {
                suggestions = null;
                this.logic = logic;
            }

            // IListWindowViewModel implementation properties -----------------

            public IListWindowAccess ListWindowAccess
            {
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");

                    if (access != null)
                        throw new InvalidOperationException("Access can be set only once!");

                    access = value;
                }
            }

            public IEnumerable<SuggestionDTO> Suggestions
            {
                get
                {
                    return suggestions;
                }
            }

            public int SelectedItemIndex
            {
                get
                {
                    return selectedItemIndex;
                }
                set
                {
                    selectedItemIndex = value;
                    OnSelectedItemIndexChanged();
                }
            }

            // Public properties ----------------------------------------------

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
