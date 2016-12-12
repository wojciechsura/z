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
using Z.Models.DTO;

namespace Z.BusinessLogic
{
    public class MainLogic : IMainWindowLogic, IListWindowLogic
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

        private readonly IGlobalHotkeyService globalHotkeyService;
        private readonly IKeywordService keywordService;
        private readonly IModuleService moduleService;

        private CurrentKeyword currentKeyword;

        private SuggestionData currentSuggestion;
        private List<SuggestionData> suggestions;

        private readonly DispatcherTimer enteredTextTimer;

        private IMainWindowViewModelAccess mainWindowViewModel;
        private IListWindowViewModelAccess listWindowViewModel;

        // Private methods ----------------------------------------------------

        private T Safe<T>(Func<IMainWindowViewModelAccess, T> func, T defaultValue = default(T))
        {
            if (mainWindowViewModel != null)
                return func(mainWindowViewModel);
            else
                return defaultValue;
        }

        private void Safe(Action<IMainWindowViewModelAccess> action)
        {
            if (mainWindowViewModel != null)
                action(mainWindowViewModel);
        }

        private T Safe<T>(Func<IListWindowViewModelAccess, T> func, T defaultValue = default(T))
        {
            if (mainWindowViewModel != null)
                return func(listWindowViewModel);
            else
                return defaultValue;
        }

        private void Safe(Action<IListWindowViewModelAccess> action)
        {
            if (mainWindowViewModel != null)
                action(listWindowViewModel);
        }

        private void Safe(Action<IMainWindowViewModelAccess, IListWindowViewModelAccess> action)
        {
            if (mainWindowViewModel != null && listWindowViewModel != null)
                action(mainWindowViewModel, listWindowViewModel);
        }

        private void ClearInput()
        {
            Safe(mainWindowViewModel => mainWindowViewModel.EnteredText = null);
            ClearSuggestions();
            ClearKeywordData();
        }

        private void ClearKeywordData()
        {
            currentKeyword = null;

            UpdateViewmodelKeyword();
        }

        private void ClearSuggestions()
        {
            suggestions = null;
            Safe((mainWindowViewModel, listWindowViewModel) => {
                listWindowViewModel.Suggestions = null;
                mainWindowViewModel.HideList();
            });            
        }

        private void CollectSuggestions()
        {
            Safe((mainWindowViewModel, listWindowViewModel) =>
            {
                if (!String.IsNullOrEmpty(mainWindowViewModel.EnteredText) || currentKeyword != null)
                {
                    suggestions = moduleService.GetSuggestionsFor(mainWindowViewModel.EnteredText, currentKeyword?.Keyword);

                    if (suggestions.Count > 0)
                    {
                        List<SuggestionDTO> suggestionsDTO = new List<SuggestionDTO>();
                        for (int i = 0; i < suggestions.Count; i++)
                            suggestionsDTO.Add(new SuggestionDTO(suggestions[i].Suggestion.Display, suggestions[i].Suggestion.Comment, suggestions[i].Module.DisplayName, i));

                        listWindowViewModel.Suggestions = suggestionsDTO;
                        mainWindowViewModel.ShowList();
                    }
                    else
                        ClearSuggestions();
                }
                else
                    ClearSuggestions();

                currentSuggestion = null;                
            });
        }

        private void EnteredTextTimerTick(object sender, EventArgs e)
        {
            StopEnteredTextTimer();
            CollectSuggestions();
        }

        private void HideWindow()
        {
            Safe(mainWindowViewModel => mainWindowViewModel.HideWindow());
        }

        private void HotkeyPressed(object sender, EventArgs args)
        {
            ShowWindow();
        }

        private bool IsInputEmpty()
        {            
            return Safe(mainWindowViewModel => currentKeyword == null && String.IsNullOrEmpty(mainWindowViewModel.EnteredText), true);
        }

        private void SetKeywordData(Models.KeywordData action, string keywordText)
        {
            currentKeyword = new CurrentKeyword(action, $"{keywordText} ");
            UpdateViewmodelKeyword();
        }

        private void ShowWindow()
        {
            ClearInput();
            Safe(mainWindowViewModel => mainWindowViewModel.ShowWindow());
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
            Safe(mainWindowViewModel =>
            {
                mainWindowViewModel.Keyword = currentKeyword?.Keyword.DisplayName;
                mainWindowViewModel.KeywordVisible = currentKeyword != null;
            });
        }

        private void UpdateListWindowViewmodel()
        {
            // TODO
        }

        private void UpdateMainWindowViewmodel()
        {
            UpdateViewmodelKeyword();
        }

        // Public methods -----------------------------------------------------

        public MainLogic(IGlobalHotkeyService globalHotkeyService, IKeywordService keywordService, IModuleService moduleService)
        {
            this.globalHotkeyService = globalHotkeyService;
            this.keywordService = keywordService;
            this.moduleService = moduleService;
            
            this.suggestions = null;
        
            currentKeyword = null;

            this.enteredTextTimer = new DispatcherTimer();
            enteredTextTimer.Interval = timerInterval;
            enteredTextTimer.Tick += EnteredTextTimerTick;

            globalHotkeyService.HotkeyHit += HotkeyPressed;
        }

        public void EnteredTextChanged()
        {
            StartEnteredTextTimer();
        }

        public bool BackspacePressed()
        {
            if (mainWindowViewModel.CaretPosition == 0 && currentKeyword != null)
            {
                int keywordTextLength = currentKeyword.StoredText.Length;

                mainWindowViewModel.EnteredText = currentKeyword.StoredText + mainWindowViewModel.EnteredText;
                mainWindowViewModel.CaretPosition = keywordTextLength;

                ClearKeywordData();
                return true;
            }

            return false;
        }

        public bool SpacePressed()
        {
            if (String.IsNullOrEmpty(mainWindowViewModel.EnteredText))
                return false;

            int indexOfSpace = mainWindowViewModel.EnteredText.IndexOf(' ');

            // Check if potential keyword is entered
            if (mainWindowViewModel.CaretPosition > 0 && (indexOfSpace >= mainWindowViewModel.CaretPosition || indexOfSpace == -1))
            {
                string possibleKeyword = mainWindowViewModel.EnteredText.Substring(0, mainWindowViewModel.CaretPosition);

                Models.KeywordData action = keywordService.GetKeywordAction(possibleKeyword);
                if (action != null)
                {
                    SetKeywordData(action, possibleKeyword);
                    mainWindowViewModel.EnteredText = mainWindowViewModel.EnteredText.Substring(mainWindowViewModel.CaretPosition);
                    mainWindowViewModel.CaretPosition = 0;
                    return true;
                }
            }

            // Module may want to provide suggestions on empty text
            enteredTextTimer.Start();

            return false;
        }

        public bool TabPressed()
        {
            return false;
        }

        public bool EnterPressed()
        {
            // Stopping timer
            enteredTextTimer.Stop();

            if (currentKeyword != null)
            {
                // Executing keyword action
                currentKeyword.Keyword.Module.ExecuteKeywordAction(currentKeyword.Keyword.ActionName, mainWindowViewModel.EnteredText);
                HideWindow();
                return true;
            }

            return false;
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

        public IMainWindowViewModelAccess MainWindowViewModel
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (mainWindowViewModel != null)
                    throw new InvalidOperationException("MainWindowViewModel can be set only once!");

                mainWindowViewModel = value;
                UpdateMainWindowViewmodel();
            }
        }

        public IListWindowViewModelAccess ListWindowViewModel
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (listWindowViewModel != null)
                    throw new InvalidOperationException("ListWindowViewModel can be set only once!");

                listWindowViewModel = value;
                UpdateListWindowViewmodel();
            }
        }
    }
}
