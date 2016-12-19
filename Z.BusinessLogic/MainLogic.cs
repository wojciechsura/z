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
using Z.BusinessLogic.Interfaces.ViewModels;

namespace Z.BusinessLogic
{
    partial class MainLogic
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
                        .OrderBy(k => k.Keyword)
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

        private readonly MainWindowViewModelImplementation mainWindowViewModel;
        private readonly ListWindowViewModelImplementation listWindowViewModel;

        private readonly HelpModule helpModule;

        // Private methods ----------------------------------------------------

        private bool BackspacePressed()
        {
            if (mainWindowViewModel.CaretPosition == 0 && currentKeyword != null)
            {
                int keywordTextLength = currentKeyword.StoredText.Length;

                mainWindowViewModel.InternalEnteredText = currentKeyword.StoredText + mainWindowViewModel.InternalEnteredText;
                mainWindowViewModel.CaretPosition = keywordTextLength;

                ClearKeywordData();
                return true;
            }

            return false;
        }

        private void ClearInput()
        {
            mainWindowViewModel.InternalEnteredText = null;
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
            
            listWindowViewModel.InternalSuggestions = null;
            mainWindowViewModel.HideList();
        }

        private void CollectSuggestions()
        {
        if (!String.IsNullOrEmpty(mainWindowViewModel.InternalEnteredText) || currentKeyword != null)
            {
                suggestions = moduleService.GetSuggestionsFor(mainWindowViewModel.InternalEnteredText, currentKeyword?.Keyword);

                if (suggestions.Count > 0)
                {
                    List<SuggestionDTO> suggestionsDTO = new List<SuggestionDTO>();
                    for (int i = 0; i < suggestions.Count; i++)
                        suggestionsDTO.Add(new SuggestionDTO(suggestions[i].Suggestion.Display, 
                            suggestions[i].Suggestion.Comment, 
                            suggestions[i].Module.DisplayName, 
                            suggestions[i].Suggestion.Image,
                            i));

                    listWindowViewModel.InternalSuggestions = suggestionsDTO;
                    mainWindowViewModel.ShowList();
                }
                else
                    ClearSuggestions();
            }
            else
                ClearSuggestions();
        }

        private bool DownPressed()
        {
            listWindowViewModel.SelectNextSuggestion();
            return true;
        }

        private bool EnterPressed()
        {
            // Stopping timer
            enteredTextTimer.Stop();

            ExecuteOptions options = new ExecuteOptions();

            if (currentKeyword != null)
            {
                // Executing keyword action
                currentKeyword.Keyword.Module.ExecuteKeywordAction(currentKeyword.Keyword.ActionName, mainWindowViewModel.InternalEnteredText, options);
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
                    Process.Start(mainWindowViewModel.InternalEnteredText);
                }
                catch (Exception e)
                {
                    // TODO handle commands, which are invalid
                }
            }

            if (!options.PreventClose)
                HideWindow();

            return true;
        }

        private void EnteredTextChanged()
        {
            StartEnteredTextTimer();
            mainWindowViewModel.InternalShowHint = false;
        }

        private void EnteredTextTimerTick(object sender, EventArgs e)
        {
            StopEnteredTextTimer();
            CollectSuggestions();
        }

        private bool EscapePressed()
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

        private void HideWindow()
        {
            mainWindowViewModel.HideWindow();
        }

        private void HotkeyPressed(object sender, EventArgs args)
        {
            ShowWindow();
        }

        private bool IsInputEmpty()
        {            
            return currentKeyword == null && String.IsNullOrEmpty(mainWindowViewModel.InternalEnteredText);
        }

        private void OpenConfigurationPressed()
        {
            mainWindowViewModel.OpenConfiguration();
        }

        private void SelectedSuggestionChanged()
        {
            SuggestionDTO suggestion = listWindowViewModel.SelectedSuggestion;
            if (suggestion != null)
            {
                var text = suggestions[suggestion.Index].Suggestion.Text;
                mainWindowViewModel.InternalEnteredText = text;
                mainWindowViewModel.CaretPosition = text.Length;
            }
        }

        private void SetKeywordData(Models.KeywordData action, string keywordText, string enteredText)
        {
            mainWindowViewModel.InternalEnteredText = enteredText;
            SetKeywordData(action, keywordText);
        }

        private void SetKeywordData(Models.KeywordData action, string keywordText)
        {
            currentKeyword = new CurrentKeyword(action, $"{keywordText} ");
            UpdateViewmodelKeyword();
        }

        private void ShowWindow()
        {
            ClearInput();

            mainWindowViewModel.ShowWindow();
            mainWindowViewModel.InternalShowHint = true;
        }

        private bool SpacePressed()
        {
            if (String.IsNullOrEmpty(mainWindowViewModel.InternalEnteredText))
                return false;

            int indexOfSpace = mainWindowViewModel.InternalEnteredText.IndexOf(' ');

            // Check if potential keyword is entered
            if (mainWindowViewModel.CaretPosition > 0 && (indexOfSpace >= mainWindowViewModel.CaretPosition || indexOfSpace == -1))
            {
                string possibleKeyword = mainWindowViewModel.InternalEnteredText.Substring(0, mainWindowViewModel.CaretPosition);

                Models.KeywordData action = keywordService.GetKeywordAction(possibleKeyword);
                if (action != null)
                {
                    SetKeywordData(action, possibleKeyword);
                    mainWindowViewModel.InternalEnteredText = mainWindowViewModel.InternalEnteredText.Substring(mainWindowViewModel.CaretPosition);
                    mainWindowViewModel.CaretPosition = 0;

                    // Module may want to provide suggestions on empty text
                    enteredTextTimer.Start();

                    return true;
                }
            }

            return false;
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

        private bool TabPressed()
        {
            return false;
        }

        private void UpdateViewmodelKeyword()
        {
            mainWindowViewModel.InternalKeyword = currentKeyword?.Keyword.DisplayName;
            mainWindowViewModel.InternalKeywordVisible = currentKeyword != null;
        }

        private void UpdateListWindowViewmodel()
        {
            // TODO
        }

        private void UpdateMainWindowViewmodel()
        {
            UpdateViewmodelKeyword();
        }

        private bool UpPressed()
        {
            listWindowViewModel.SelectPreviousSuggestion();
            return true;
        }

        private bool WindowClosing()
        {
            // Store window position
            configurationSerivice.Configuration.MainWindow.Position = mainWindowViewModel.Position;

            configurationSerivice.Save();
            return true;
        }

        private void WindowInitialized()
        {
            mainWindowViewModel.Position = configurationSerivice.Configuration.MainWindow.Position;
        }

        private void WindowLostFocus()
        {
            // HideWindow();
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

            mainWindowViewModel = new MainWindowViewModelImplementation(this);
            listWindowViewModel = new ListWindowViewModelImplementation(this);
        }

        public IMainWindowViewModel MainWindowViewModel => mainWindowViewModel;

        public IListWindowViewModel ListWindowViewModel => listWindowViewModel;
    }
}
