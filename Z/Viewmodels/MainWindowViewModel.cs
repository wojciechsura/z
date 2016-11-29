using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.Common;
using Z.Services.Interfaces;
using Z.Viewmodels.Interfaces;

namespace Z.Viewmodels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IHotkeyService hotkeyService;
        private readonly IMainViewModelAccess access;

        private int mainHotkeyId;

        private string enteredText;
        private bool keywordVisible;
        private string keyword;
        private string keywordStoredText;

        // Private methods ----------------------------------------------------

        private void ShowWindow()
        {
            EnteredText = null;
            KeywordVisible = false;
            Keyword = null;

            access.Show();
        }

        private void HideWindow()
        {
            access.Hide();
        }

        private void Initialize()
        {
            if (!hotkeyService.Register(Key.Z, KeyModifier.Win, HotkeyPressed, ref mainHotkeyId))
            {
                access.ShowError(Z.Resources.Resources.CannotRegisterHotkey);
                access.Shutdown();
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

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainViewModelAccess access, IHotkeyService hotkeyService)
        {
            this.hotkeyService = hotkeyService;
            this.access = access;

            keywordVisible = false;
            enteredText = null;

            Initialize();
        }

        internal bool BackspacePressed()
        {
            if (access.CaretPosition == 0 && keywordVisible)
            {
                int keywordTextLength = keywordStoredText.Length;

                Keyword = null;
                KeywordVisible = false;
                EnteredText = keywordStoredText + EnteredText;
                access.CaretPosition = keywordTextLength;
                keywordStoredText = null;

                return true;
            }

            return false;
        }

        public bool SpacePressed()
        {
            if (String.IsNullOrEmpty(EnteredText))
                return false;

            int indexOfSpace = EnteredText.IndexOf(' ');

            // Check if potential keyword is entered
            if (access.CaretPosition > 0 && (indexOfSpace <= access.CaretPosition || indexOfSpace == -1))
            {
                string possibleKeyword = EnteredText.Substring(0, access.CaretPosition);

                // TODO Recognizing keywords

                if (possibleKeyword.ToLower() == "g")
                {
                    Keyword = "Google";
                    KeywordVisible = true;
                    keywordStoredText = possibleKeyword + " ";
                    EnteredText = EnteredText.Substring(access.CaretPosition);
                    access.CaretPosition = 0;
                    return true;
                }
            }

            return false;
        }

        public bool TabPressed()
        {
            return false;
        }

        public bool EnterPressed()
        {
            return false;
        }

        public void HotkeyPressed()
        {
            ShowWindow();
        }

        public bool EscapePressed()
        {
            HideWindow();
            return true;
        }

        public void WindowLostFocus()
        {
#if !DEBUG
            HideWindow();
#endif
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

        public string Keyword
        {
            get
            {
                return keyword;
            }
            private set
            {
                keyword = value;
                OnPropertyChanged(nameof(Keyword));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
