using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Z.BusinessLogic.Interfaces;
using Z.ViewModels.Interfaces;
using Microsoft.Practices.Unity;
using System.ComponentModel;

namespace Z.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged, IMainWindowViewModel, IMainWindowViewModelAccess
    {
        private IMainWindowAccess access;
        private IMainWindowLogic logic;

        private string keyword;
        private string enteredText;
        private bool keywordVisible;

        // Private methods ----------------------------------------------------

        private T Safe<T>(Func<IMainWindowAccess, T> func)
        {
            if (access != null)
                return func(access);
            else
                return default(T);
        }

        private void Safe(Action<IMainWindowAccess> action)
        {
            if (access != null)
                action(access);
        }

        // IMainWindowViewModel implementation --------------------------------

        void IMainWindowViewModelAccess.ShowWindow()
        {
            Safe(access => access.Show());
        }

        void IMainWindowViewModelAccess.HideWindow()
        {
            Safe(access => access.Hide());
        }

        void IMainWindowViewModelAccess.ShowList()
        {
            Safe(access => access.ShowList());
        }

        void IMainWindowViewModelAccess.HideList()
        {
            Safe(access => access.HideList());
        }

        string IMainWindowViewModelAccess.EnteredText
        {
            get
            {
                return enteredText;
            }
            set
            {
                if (enteredText != value)
                {
                    enteredText = value;
                    OnPropertyChanged(nameof(EnteredText));
                }
            }
        }

        string IMainWindowViewModelAccess.Keyword
        {
            set
            {
                if (keyword != value)
                { 
                    keyword = value;
                    OnPropertyChanged(nameof(Keyword));
                }
            }
        }

        bool IMainWindowViewModelAccess.KeywordVisible
        {
            set
            {
                if (keywordVisible != value)
                {
                    keywordVisible = value;
                    OnPropertyChanged(nameof(KeywordVisible));
                }
            }
        }

        int IMainWindowViewModelAccess.CaretPosition
        {
            get
            {
                return Safe(access => access.CaretPosition);
            }
            set
            {
                Safe(access => access.CaretPosition = value);
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnEnteredTextChanged()
        {
            logic.EnteredTextChanged();
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowLogic logic)
        {
            this.logic = logic;
            logic.MainWindowViewModel = this;
        }

        public bool EscapePressed()
        {
            return logic.EscapePressed();
        }

        public bool SpacePressed()
        {
            return logic.SpacePressed();
        }

        public bool BackspacePressed()
        {
            return logic.BackspacePressed();
        }

        public bool TabPressed()
        {
            return logic.TabPressed();
        }

        public bool EnterPressed()
        {
            return logic.EnterPressed();
        }

        public bool UpPressed()
        {
            return logic.UpPressed();
        }

        public bool DownPressed()
        {
            return logic.DownPressed();
        }

        public void WindowLostFocus()
        {
            logic.WindowLostFocus();
        }

        // Public properties --------------------------------------------------

        public string Keyword
        {
            get
            {
                return keyword;
            }
        }

        public bool KeywordVisible
        {
            get
            {
                return keywordVisible;
            }
        }

        public string EnteredText
        {
            get
            {
                return enteredText;
            }
            set
            {
                enteredText = value;
                OnEnteredTextChanged();
            }
        }

        public IMainWindowAccess MainWindowAccess
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (access != null)
                    throw new InvalidOperationException("Only one main window access is allowed!");

                access = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;    
    }
}
