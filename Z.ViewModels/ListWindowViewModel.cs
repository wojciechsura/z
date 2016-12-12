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

        private IEnumerable<SuggestionDTO> suggestions;

        // Private methods ----------------------------------------------------

        private void SetSuggestions(IEnumerable<SuggestionDTO> value)
        {
            suggestions = value;
            OnPropertyChanged(nameof(Suggestions));
        }

        // IListWindowViewModelAccess implementation --------------------------

        IEnumerable<SuggestionDTO> IListWindowViewModelAccess.Suggestions
        {
            set
            {
                SetSuggestions(value);
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
