using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Z.Models;

using Microsoft.Practices.Unity;
using Z.BusinessLogic.Interfaces;
using Z.BusinessLogic.Services.Interfaces;
using Z.BusinessLogic.Common;

namespace Z.BusinessLogic
{
    public class MainWindowLogic : IMainWindowLogic
    {
        // Private types ------------------------------------------------------

        private class CurrentKeyword
        {
            public CurrentKeyword(KeywordData keyword, string storedText)
            {
                Keyword = keyword;
                StoredText = storedText;
            }

            public KeywordData Keyword { get; private set; }
            public string StoredText { get; private set; }
        }
        
        // Private constants --------------------------------------------------

        private readonly TimeSpan timerInterval = TimeSpan.FromMilliseconds(500);

        // Private fields -----------------------------------------------------

        private readonly IHotkeyService hotkeyService;
        private readonly IKeywordService keywordService;
        private readonly IModuleService moduleService;

        private int mainHotkeyId;
        private CurrentKeyword currentKeyword;

        private SuggestionData currentSuggestion;
        private List<SuggestionData> suggestions;

        private readonly DispatcherTimer enteredTextTimer;

        private IMainWindowViewModelAccess viewModel;

        // Private methods ----------------------------------------------------

        private void ClearInput()
        {
            viewModel.EnteredText = null;
            ClearKeywordData();
        }

        private void ClearKeywordData()
        {
            currentKeyword = null;

            UpdateViewmodelKeyword();
        }

        private void CollectSuggestions()
        {
            suggestions = moduleService.GetSuggestionsFor(viewModel.EnteredText, currentKeyword?.Keyword);
            currentSuggestion = null;
        }

        private void EnteredTextTimerTick(object sender, EventArgs e)
        {
            StopEnteredTextTimer();
            CollectSuggestions();
        }

        private void HideWindow()
        {
            viewModel.HideWindow();
        }

        private void Initialize()
        {
            if (!hotkeyService.Register(Key.Space, KeyModifier.Alt, HotkeyPressed, ref mainHotkeyId))
            {
                viewModel.ShowError(Z.BusinessLogic.Resources.Text.CannotRegisterHotkey);
                viewModel.Shutdown();
            }
        }

        private bool IsInputEmpty()
        {
            return currentKeyword == null && String.IsNullOrEmpty(viewModel.EnteredText);
        }

        private void SetKeywordData(Models.KeywordData action, string keywordText)
        {
            currentKeyword = new CurrentKeyword(action, $"{keywordText} ");
            UpdateViewmodelKeyword();
        }

        private void ShowWindow()
        {
            ClearInput();
            viewModel.ShowWindow();
        }

        private void StartEnteredTextTimer()
        {
            // Reset timer if it is already counting down
            if (enteredTextTimer.IsEnabled)
                enteredTextTimer.Stop();

            enteredTextTimer.Start();
        }

        private void StopEnteredTextTimer()
        {
            enteredTextTimer.Stop();
        }

        private void UpdateViewmodelKeyword()
        {
            viewModel.Keyword = currentKeyword?.Keyword.DisplayName;
            viewModel.KeywordVisible = currentKeyword != null;
        }

        // Public methods -----------------------------------------------------

        public MainWindowLogic(IMainWindowViewModelAccess viewModel, IHotkeyService hotkeyService, IKeywordService keywordService, IModuleService moduleService)
        {
            this.hotkeyService = hotkeyService;
            this.keywordService = keywordService;
            this.moduleService = moduleService;
            
            this.suggestions = null;
        
            currentKeyword = null;

            this.enteredTextTimer = new DispatcherTimer();
            enteredTextTimer.Interval = timerInterval;
            enteredTextTimer.Tick += EnteredTextTimerTick;

            this.viewModel = viewModel;

            Initialize();
        }

        public void EnteredTextChanged()
        {
            StartEnteredTextTimer();
        }

        public bool BackspacePressed()
        {
            if (viewModel.CaretPosition == 0 && currentKeyword != null)
            {
                int keywordTextLength = currentKeyword.StoredText.Length;

                viewModel.EnteredText = currentKeyword.StoredText + viewModel.EnteredText;
                viewModel.CaretPosition = keywordTextLength;

                ClearKeywordData();
                return true;
            }

            return false;
        }

        public bool SpacePressed()
        {
            if (String.IsNullOrEmpty(viewModel.EnteredText))
                return false;

            int indexOfSpace = viewModel.EnteredText.IndexOf(' ');

            // Check if potential keyword is entered
            if (viewModel.CaretPosition > 0 && (indexOfSpace >= viewModel.CaretPosition || indexOfSpace == -1))
            {
                string possibleKeyword = viewModel.EnteredText.Substring(0, viewModel.CaretPosition);

                Models.KeywordData action = keywordService.GetKeywordAction(possibleKeyword);
                if (action != null)
                {
                    SetKeywordData(action, possibleKeyword);
                    viewModel.EnteredText = viewModel.EnteredText.Substring(viewModel.CaretPosition);
                    viewModel.CaretPosition = 0;
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
            if (currentKeyword != null)
            {
                // Executing keyword action
                currentKeyword.Keyword.Module.ExecuteKeywordAction(currentKeyword.Keyword.ActionName, viewModel.EnteredText);
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
    }
}
