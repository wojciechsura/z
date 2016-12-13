using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Interfaces;
using Z.Models;
using Z.ViewModels.Interfaces;
using Z.Models.DTO;
using System.ComponentModel;

namespace Z.ViewModels
{
    class ListWindowViewModel : INotifyPropertyChanged, IListWindowViewModel, IListWindowViewModelAccess
    {
        // Private fields -----------------------------------------------------

        private IListWindowAccess access;
        private IListWindowLogic logic;

        private List<SuggestionDTO> suggestions;
        private int selectedItemIndex;

        // Private methods ----------------------------------------------------

        private void SetSuggestions(List<SuggestionDTO> value)
        {
            suggestions = value;
            OnPropertyChanged(nameof(Suggestions));
            SetSelectedItemIndex(-1);
        }

        private void SetSelectedItemIndex(int index)
        {
            if (suggestions != null && (index < -1 || index >= suggestions.Count))
                throw new ArgumentOutOfRangeException(nameof(index));

            selectedItemIndex = index;
            OnPropertyChanged(nameof(SelectedItemIndex));

            access.EnsureSelectedIsVisible();
        }

        private void OnSelectedItemIndexChanged()
        {
            logic.SelectedSuggestionChanged();
        }

        // IListWindowViewModelAccess implementation --------------------------

        List<SuggestionDTO> IListWindowViewModelAccess.Suggestions
        {
            set
            {
                SetSuggestions(value);
            }
        }

        void IListWindowViewModelAccess.SelectPreviousSuggestion()
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

        void IListWindowViewModelAccess.SelectNextSuggestion()
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

        SuggestionDTO IListWindowViewModelAccess.SelectedSuggestion
        {
            get
            {
                if (selectedItemIndex < 0)
                    return null;
                else
                    return suggestions[selectedItemIndex];
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public ListWindowViewModel(IListWindowLogic logic)
        {
            suggestions = null;

            this.logic = logic;
            logic.ListWindowViewModel = this;
        }

        // Public properties --------------------------------------------------

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
