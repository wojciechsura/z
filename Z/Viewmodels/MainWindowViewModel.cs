using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.Common;
using Z.Models;
using Z.Services.Interfaces;
using Z.Viewmodels.Interfaces;

namespace Z.Viewmodels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private class KeywordData
        {
            public KeywordData(KeywordAction action, string storedText)
            {
                Action = action;
                StoredText = storedText;
            }

            public KeywordAction Action { get; private set; }
            public string StoredText { get; private set; }
        }

        private readonly IHotkeyService hotkeyService;
        private readonly IMainViewModelAccess access;
        private readonly IKeywordService keywordService;

        private int mainHotkeyId;

        private string enteredText;
        private KeywordData keywordData;

        // Private methods ----------------------------------------------------

        private void ClearInput()
        {
            EnteredText = null;
            ClearKeywordData();
        }

        private void ShowWindow()
        {
            ClearInput();

            access.Show();
        }

        private void HideWindow()
        {
            access.Hide();
        }

        private void Initialize()
        {
            if (!hotkeyService.Register(Key.Space, KeyModifier.Alt, HotkeyPressed, ref mainHotkeyId))
            {
                access.ShowError(Z.Resources.Resources.CannotRegisterHotkey);
                access.Shutdown();
            }
        }

        private void ClearKeywordData()
        {
            keywordData = null;

            NotifyKeywordDataChanged();
        }

        private void SetKeywordData(KeywordAction action, string possibleKeyword)
        {
            keywordData = new KeywordData(action, possibleKeyword + " ");

            NotifyKeywordDataChanged();
        }

        private void NotifyKeywordDataChanged()
        {
            OnPropertyChanged(nameof(Keyword));
            OnPropertyChanged(nameof(KeywordVisible));
        }

        private bool IsInputEmpty()
        {
            return keywordData == null && String.IsNullOrEmpty(EnteredText);
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

        public MainWindowViewModel(IMainViewModelAccess access, IHotkeyService hotkeyService, IKeywordService keywordService)
        {
            this.hotkeyService = hotkeyService;
            this.keywordService = keywordService;
            this.access = access;

            enteredText = null;
            keywordData = null;

            Initialize();
        }

        public bool BackspacePressed()
        {
            if (access.CaretPosition == 0 && keywordData != null)
            {
                int keywordTextLength = keywordData.StoredText.Length;

                EnteredText = keywordData.StoredText + EnteredText;
                access.CaretPosition = keywordTextLength;

                ClearKeywordData();
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
            if (access.CaretPosition > 0 && (indexOfSpace >= access.CaretPosition || indexOfSpace == -1))
            {
                string possibleKeyword = EnteredText.Substring(0, access.CaretPosition);

                KeywordAction action = keywordService.GetKeywordAction(possibleKeyword);
                if (action != null)
                {
                    SetKeywordData(action, possibleKeyword);
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
            if (keywordData != null)
            {
                // Executing keyword action
                keywordData.Action.Module.ExecuteKeywordAction(keywordData.Action.ActionName, EnteredText);
                HideWindow();
                return true;
            }

            return false;
        }

        public void HotkeyPressed()
        {
            ShowWindow();
        }

        public bool EscapePressed()
        {
            if (!IsInputEmpty())
            {
                ClearInput();
            }
            else
            {
                HideWindow();
            }
            
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
                return keywordData != null;
            }            
        }

        public string Keyword
        {
            get
            {
                return keywordData?.Action?.DisplayName ?? null;
            }            
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
