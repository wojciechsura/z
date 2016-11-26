using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Viewmodels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string enteredText;
        private bool keywordVisible;

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel()
        {
            keywordVisible = false;
            enteredText = null;
        }

        // Public properties --------------------------------------------------

        public string EnteredText
        {
            get
            {
                return enteredText;
            }
            set
            {
                enteredText = value;
                OnPropertyChanged(nameof(EnteredText));
            }
        }

        public bool KeywordVisible
        {
            get
            {
                return keywordVisible;
            }
            set
            {
                keywordVisible = value;
                OnPropertyChanged(nameof(KeywordVisible));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void HotkeyPressed()
        {
            throw new NotImplementedException();
        }
    }
}
