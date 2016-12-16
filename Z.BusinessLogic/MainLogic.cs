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
using System.Diagnostics;
using Z.Api.Interfaces;
using Z.Api.Types;

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

        private class HelpModule : IZModule
        {
            private const string MODULE_DISPLAY_NAME = "Help";
            private const string MODULE_NAME = "Help";
            private readonly MainLogic logic;

            public HelpModule(MainLogic logic)
            {
                this.logic = logic;
            }

            public void CollectSuggestions(string enteredText, string keywordAction, ISuggestionCollector collector)
            {
                if (enteredText == "?")
                {
                    logic.keywordService.GetKeywords()
                        .Select(k => new SuggestionInfo(k.Keyword, k.Keyword, k.Comment, null, k))
                        .ToList()
                        .ForEach(s => collector.AddSuggestion(s));
                }
            }

            public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
            {
                KeywordData keyword = logic.keywordService.GetKeywords()
                    .Where(k => k.Keyword.ToUpper() == expression.ToUpper())
                    .FirstOrDefault();

                if (keyword != null)
                {
                    logic.SetKeywordData(keyword, keyword.Keyword, "");
                    logic.CollectSuggestions();
                }

                options.PreventClose = true;
            }

            public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
            {
                KeywordData keyword = suggestion.Data as KeywordData;
                if (keyword != null)
                {
                    logic.SetKeywordData(keyword, keyword.Keyword, "");
                    logic.CollectSuggestions();
                }

                options.PreventClose = true;
            }

            public IEnumerable<KeywordInfo> GetKeywordActions()
            {
                return null;
            }

            public string DisplayName
            {
                get
                {
                    return MODULE_DISPLAY_NAME;
                }
            }

            public string InternalName
            {
                get
                {
                    return MODULE_NAME;
                }
            }
        }

        // Private constants --------------------------------------------------

        private readonly TimeSpan timerInterval = TimeSpan.FromMilliseconds(200);

        // Private fields -----------------------------------------------------

        private readonly IGlobalHotkeyService globalHotkeyService;
        private readonly IKeywordService keywordService;
        private readonly IModuleService moduleService;
        private readonly IConfigurationService configurationSerivice;

        private CurrentKeyword currentKeyword;
        private List<SuggestionData> suggestions;

        private readonly DispatcherTimer enteredTextTimer;

        private IMainWindowViewModelAccess mainWindowViewModel;
        private IListWindowViewModelAccess listWindowViewModel;

        private readonly HelpModule helpModule;

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
                            suggestionsDTO.Add(new SuggestionDTO(suggestions[i].Suggestion.Display, 
                                suggestions[i].Suggestion.Comment, 
                                suggestions[i].Module.DisplayName, 
                                suggestions[i].Suggestion.Image,
                                i));

                        listWindowViewModel.Suggestions = suggestionsDTO;
                        mainWindowViewModel.ShowList();
                    }
                    else
                        ClearSuggestions();
                }
                else
                    ClearSuggestions();
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
         
        private void SetKeywordData(Models.KeywordData action, string keywordText, string enteredText)
        {
            Safe(mainWindowViewModel =>
            {
                mainWindowViewModel.EnteredText = enteredText;
                SetKeywordData(action, keywordText);
            });           
        }

        private void SetKeywordData(Models.KeywordData action, string keywordText)
        {
            currentKeyword = new CurrentKeyword(action, $"{keywordText} ");
            UpdateViewmodelKeyword();
        }

        private void ShowWindow()
        {
            ClearInput();
            Safe(mainWindowViewModel => {
                mainWindowViewModel.ShowWindow();
                mainWindowViewModel.ShowHint = true;
            });
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

        // IListWindowLogic implementation ------------------------------------

        void IListWindowLogic.SelectedSuggestionChanged()
        {
            Safe((mainWindowViewModel, listWindowViewModel) => {
                SuggestionDTO suggestion = listWindowViewModel.SelectedSuggestion;
                if (suggestion != null)
                {
                    var text = suggestions[suggestion.Index].Suggestion.Text;
                    mainWindowViewModel.EnteredText = text;
                    mainWindowViewModel.CaretPosition = text.Length;
                }
            });
        }

        IListWindowViewModelAccess IListWindowLogic.ListWindowViewModel
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

        // IMainWindowLogic implementation ------------------------------------

        void IMainWindowLogic.EnteredTextChanged()
        {
            StartEnteredTextTimer();
            Safe((IMainWindowViewModelAccess mainActivityViewModel) => {
                mainWindowViewModel.ShowHint = false;
            });
        }

        bool IMainWindowLogic.BackspacePressed()
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

        bool IMainWindowLogic.SpacePressed()
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

                    // Module may want to provide suggestions on empty text
                    enteredTextTimer.Start();

                    return true;
                }
            }

            return false;
        }

        bool IMainWindowLogic.TabPressed()
        {
            return false;
        }

        bool IMainWindowLogic.EnterPressed()
        {
            // Stopping timer
            enteredTextTimer.Stop();

            Safe((mainWindowViewModel, listWindowViewModel) =>
            {

                ExecuteOptions options = new ExecuteOptions();

                if (currentKeyword != null)
                {
                    // Executing keyword action
                    currentKeyword.Keyword.Module.ExecuteKeywordAction(currentKeyword.Keyword.ActionName, mainWindowViewModel.EnteredText, options);
                }
                else if (listWindowViewModel.SelectedSuggestion != null)
                {
                    SuggestionData suggestion = suggestions[listWindowViewModel.SelectedSuggestion.Index];
                    suggestion.Module.ExecuteSuggestion(suggestion.Suggestion, options);
                }
                else
                {
                    // Executing entered word
                    try
                    {
                        Process.Start(mainWindowViewModel.EnteredText);
                    }
                    catch (Exception e)
                    {
                        // TODO handle commands, which are invalid
                    }
                }

                if (!options.PreventClose)
                    HideWindow();
            });

            return true;
        }

        bool IMainWindowLogic.EscapePressed()
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

        bool IMainWindowLogic.UpPressed()
        {
            Safe(listWindowViewModel => listWindowViewModel.SelectPreviousSuggestion());
            return true;
        }

        bool IMainWindowLogic.DownPressed()
        {
            Safe(listWindowViewModel => listWindowViewModel.SelectNextSuggestion());
            return true;
        }

        void IMainWindowLogic.WindowLostFocus()
        {
#if !DEBUG
            HideWindow();
#endif
        }

        bool IMainWindowLogic.WindowClosing()
        {
            // Store window position
            Safe((mainWindowViewModel, listWindowViewModel) =>
            {
                configurationSerivice.Configuration.MainWindow.Position = mainWindowViewModel.Position;
            });

            configurationSerivice.Save();
            return true;
        }

        void IMainWindowLogic.WindowInitialized()
        {
            Safe(mainWindowViewModel => mainWindowViewModel.Position = configurationSerivice.Configuration.MainWindow.Position);
        }

        IMainWindowViewModelAccess IMainWindowLogic.MainWindowViewModel
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

        // Public methods -----------------------------------------------------

        public MainLogic(IGlobalHotkeyService globalHotkeyService, IKeywordService keywordService, IModuleService moduleService, IConfigurationService configurationService)
        {
            this.globalHotkeyService = globalHotkeyService;
            this.keywordService = keywordService;
            this.moduleService = moduleService;
            this.configurationSerivice = configurationService;

            this.suggestions = null;
        
            currentKeyword = null;

            this.enteredTextTimer = new DispatcherTimer();
            enteredTextTimer.Interval = timerInterval;
            enteredTextTimer.Tick += EnteredTextTimerTick;

            globalHotkeyService.HotkeyHit += HotkeyPressed;

            this.helpModule = new HelpModule(this);
            moduleService.AddModule(helpModule);
        }
    }
}
