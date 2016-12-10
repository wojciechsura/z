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
        private readonly IMainWindowAccess access;
        private IMainWindowLogic logic;

        private string keyword;
        private string enteredText;
        private bool keywordVisible;

        // IMainWindowViewModel implementation --------------------------------

        void IMainWindowViewModelAccess.ShowWindow()
        {
            access.Show();
        }

        void IMainWindowViewModelAccess.HideWindow()
        {
            access.Hide();
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
                return access.CaretPosition;
            }
            set
            {
                access.CaretPosition = value;
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

        public MainWindowViewModel(IMainWindowAccess access)
        {
            this.access = access;

            var logicFactory = Z.Dependencies.Container.Instance.Resolve<ILogicFactory>();
            this.logic = logicFactory.GenerateMainWindowLogic(this);
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

        public event PropertyChangedEventHandler PropertyChanged;    
    }
}
