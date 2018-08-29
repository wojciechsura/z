﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Z.Models;

using Microsoft.Practices.Unity;
using Z.BusinessLogic.Services.Interfaces;
using Z.Models.DTO;
using System.Diagnostics;
using Z.Api.Interfaces;
using Z.Api.Types;
using System.ComponentModel;
using System.Windows.Media;
using Z.Wpf.Types;
using Z.BusinessLogic.ViewModels.Interfaces;
using Z.Common.Types;
using Z.Api;
using Z.BusinessLogic.Events;
using System.Windows;

namespace Z.BusinessLogic.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IEventListener<ShuttingDownEvent>, IEventListener<ConfigurationChangedEvent>, IEventListener<PositionChangedEvent>
    {
        // Private types ------------------------------------------------------

        private class CurrentKeyword
        {
            public CurrentKeyword(KeywordData keyword, string storedText)
            {
                Keyword = keyword;
                StoredText = storedText;
            }

            public KeywordData Keyword { get; }
            public string StoredText { get; }
        }

        private class HelpModule : IZModule, IZExclusiveSuggestions
        {
            private const string MODULE_DISPLAY_NAME = "Help";
            private const string MODULE_NAME = "Help";
            private const string HELP_KEYWORD = "?";
            private readonly MainViewModel logic;

            public HelpModule(MainViewModel logic)
            {
                this.logic = logic;
            }

            public bool IsExclusiveText(string text)
            {
                return text == HELP_KEYWORD;
            }

            public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
            {
                if (enteredText == HELP_KEYWORD)
                {
                    logic.keywordService.GetKeywords()
                        .OrderBy(k => k.Keyword)
                        .Select(k => new SuggestionInfo(k.Keyword, k.Keyword, k.Comment, k.Module.Icon, 50, k))
                        .ToList()
                        .ForEach(collector.AddSuggestion);
                }
            }

            public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
            {
                KeywordData keyword = logic.keywordService
                    .GetKeywords()
                    .FirstOrDefault(k => k.Keyword.ToUpper() == expression.ToUpper());

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

            public string DisplayName => MODULE_DISPLAY_NAME;

            public string Name => MODULE_NAME;

            public ImageSource Icon => null;
        }

        // Private fields -----------------------------------------------------

        private readonly IGlobalHotkeyService globalHotkeyService;
        private readonly IKeywordService keywordService;
        private readonly IModuleService moduleService;
        private readonly IConfigurationService configurationService;
        private readonly IEventBus eventBus;
        private readonly IApplicationController applicationController;
        private readonly IWindowService windowService;

        private readonly DispatcherTimer enteredTextTimer;

        private readonly HelpModule helpModule;

        private CurrentKeyword currentKeyword;
        private List<SuggestionData> suggestionData;

        private IMainWindowAccess mainWindowAccess;
        private IListWindowAccess listWindowAccess;

        // Main window

        private string enteredText;
        private bool showHint;
        private string keyword;
        private bool keywordVisible;
        private bool completeHintVisible;
        private string errorText;
        private bool suspendPositionChangeNotifications = false;

        // List window

        private List<SuggestionDTO> suggestions;
        private int selectedItemIndex;

        // Private methods ----------------------------------------------------

        private void ClearInput()
        {
            PublishEnteredText(null);
            PublishErrorText(null);
            PublishCompleteHintVisible(false);
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
            suggestionData = null;

            PublishSuggestions(null);
            mainWindowAccess.HideList();
        }

        private void CollectSuggestions()
        {
            Func<SuggestionData, SuggestionData, int> compareGroups = (x, y) =>
            {
                if (String.IsNullOrEmpty(x.Suggestion.SortGroup) && String.IsNullOrEmpty(y.Suggestion.SortGroup))
                    return 0;
                else if (String.IsNullOrEmpty(x.Suggestion.SortGroup))
                    return 1;
                else if (String.IsNullOrEmpty(y.Suggestion.SortGroup))
                    return -1;
                else
                    return String.Compare(x.Suggestion.SortGroup, y.Suggestion.SortGroup);
            };

            Func<SuggestionData, SuggestionData, int> compareDisplayNames = (x, y) => String.Compare(x.Suggestion.Display, y.Suggestion.Display);

            Func<SuggestionData, SuggestionData, int> compareModuleDisplayNames = (x, y) => String.Compare(x.Module.DisplayName, y.Module.DisplayName);

            Func<SuggestionData, SuggestionData, int> compareMatches = (x, y) => (int)y.Suggestion.Match - (int)x.Suggestion.Match;

            Func<SuggestionData, SuggestionData, IEnumerable<Func<SuggestionData, SuggestionData, int>>, int> combineCompares =
                (x, y, compares) =>
                {
                    return compares.Select(c => c(x, y))
                        .FirstOrDefault(r => r != 0);
                };
                
            if (!String.IsNullOrEmpty(enteredText) || currentKeyword != null)
            {
                suggestionData = moduleService.GetSuggestionsFor(enteredText, currentKeyword?.Keyword);

                if (suggestionData.Count > 0)
                {
                    switch (configurationService.Configuration.Behavior.SuggestionSorting)
                    {
                        case SuggestionSorting.ByModule:
                            {
                                suggestionData.Sort((x, y) => combineCompares(x, y, new[] { compareGroups, compareModuleDisplayNames, compareDisplayNames }));
                                break;
                            }
                        case SuggestionSorting.ByDisplay:
                            {
                                suggestionData.Sort((x, y) => combineCompares(x, y, new[] { compareGroups, compareDisplayNames }));
                                break;
                            }
                        case SuggestionSorting.ByMatch:
                            {
                                suggestionData.Sort((x, y) => combineCompares(x, y, new[] { compareGroups, compareMatches, compareDisplayNames }));
                                break;
                            }
                        default:
                            throw new InvalidEnumArgumentException("Not supported suggestion sorting!");
                    }

                    List<SuggestionDTO> suggestionsDTO = new List<SuggestionDTO>();
                    for (int i = 0; i < suggestionData.Count; i++)
                        suggestionsDTO.Add(new SuggestionDTO(suggestionData[i].Suggestion.Display,
                            suggestionData[i].Suggestion.Comment,
                            suggestionData[i].Module.DisplayName,
                            suggestionData[i].Suggestion.Image,
                            suggestionData[i].Suggestion.Match,
                            i));

                    PublishSuggestions(suggestionsDTO);
                    mainWindowAccess.ShowList();
                }
                else
                    ClearSuggestions();
            }
            else
                ClearSuggestions();
        }

        public void NotifyPositionChanged(int left, int top)
        {
            eventBus.Send(new PositionChangedEvent(left, top, this));
        }

        public void Dismiss()
        {
            InternalDismissWindow();
        }

        public void Summon()
        {
            InternalSummonWindow();
        }

        private void CompleteSuggestion()
        {
            var suggestion = GetSelectedSuggestion();
            if (suggestion != null)
            {
                var selectedSuggestionData = suggestionData[suggestion.Index];
                IZModule module = selectedSuggestionData.Module;

                if (module is IZSuggestionComplete && 
                    ((IZSuggestionComplete)module).CanComplete(currentKeyword?.Keyword.ActionName, selectedSuggestionData.Suggestion))
                {
                    string replace = ((IZSuggestionComplete)module).Complete(currentKeyword?.Keyword.ActionName, selectedSuggestionData.Suggestion);

                    PublishEnteredText(replace);
                    if (replace.Length > 0)
                        mainWindowAccess.CaretPosition = replace.Length;
                    StartEnteredTextTimer();

                    PublishCompleteHintVisible(false);
                }
            }
        }

        private void EnteredTextChanged()
        {
            StartEnteredTextTimer();
            PublishShowHint(false);
            PublishCompleteHintVisible(false);
            PublishErrorText(null);
        }

        private void EnteredTextTimerTick(object sender, EventArgs e)
        {
            StopEnteredTextTimer();
            CollectSuggestions();
        }

        private void ExecuteCurrentAction()
        {
            // Stopping timer
            enteredTextTimer.Stop();

            ExecuteOptions options = new ExecuteOptions();

            if (GetSelectedSuggestion() != null)
            {
                SuggestionData suggestion = suggestionData[GetSelectedSuggestion().Index];
                suggestion.Module.ExecuteSuggestion(suggestion.Suggestion, options);
            }
            else if (currentKeyword != null)
            {
                // Executing keyword action
                currentKeyword.Keyword.Module.ExecuteKeywordAction(currentKeyword.Keyword.ActionName, enteredText, options);
            }
            else
            {
                switch (configurationService.Configuration.Behavior.EnterBehavior)
                {
                    case EnterBehavior.ShellExecute:
                        {
                            // Executing entered word
                            try
                            {
                                Process.Start(enteredText);
                            }
                            catch (Exception e)
                            {
                                options.ErrorText = $"Cannot execute: {e.Message}";
                                options.PreventClose = true;
                            }

                            break;
                        }
                    case EnterBehavior.ChooseFirst:
                        {
                            if (suggestions != null && suggestions.Count > 0)
                            {
                                SuggestionData suggestion = suggestionData[0];
                                suggestion.Module.ExecuteSuggestion(suggestion.Suggestion, options);
                            }
                            else
                            {
                                System.Media.SystemSounds.Beep.Play();
                                options.PreventClose = true;
                            }

                            break;
                        }
                    case EnterBehavior.ChoosePerfectlyMatching:
                        {
                            // Collecting perfectly matched results

                            List<SuggestionData> matchedSuggestions = moduleService.GetSuggestionsFor(enteredText, currentKeyword?.Keyword, true);

                            if (matchedSuggestions.Count == 0)
                            {
                                System.Media.SystemSounds.Beep.Play();
                                options.PreventClose = true;
                            }
                            else if (matchedSuggestions.Count == 1)
                            {
                                SuggestionData suggestion = matchedSuggestions[0];
                                suggestion.Module.ExecuteSuggestion(suggestion.Suggestion, options);
                            }
                            else
                            {
                                SuggestionChoiceViewModel suggestionChoiceViewModel = new SuggestionChoiceViewModel(suggestionData);
                                bool? result = mainWindowAccess.SelectSuggestion(suggestionChoiceViewModel);

                                if (result == true)
                                {
                                    SuggestionData suggestion = matchedSuggestions[suggestionChoiceViewModel.SelectedItemIndex];
                                    suggestion.Module.ExecuteSuggestion(suggestion.Suggestion, options);
                                }
                            }

                            break;
                        }
                    default:
                        throw new InvalidEnumArgumentException("Unsupported enter key behavior!");
                }

            }

            if (!options.PreventClose)
                InternalDismissWindow();

            PublishErrorText(options.ErrorText);
        }

        private SuggestionDTO GetSelectedSuggestion()
        {
            return selectedItemIndex >= 0 ? suggestions[selectedItemIndex] : null;
        }

        private void HandleConfigurationChanged()
        {
            enteredTextTimer.Interval = TimeSpan.FromMilliseconds(configurationService.Configuration.Behavior.SuggestionDelay);
        }

        private void InternalDismissWindow()
        {
            mainWindowAccess.Hide();
        }

        private bool IsInputEmpty()
        {
            return currentKeyword == null && String.IsNullOrEmpty(enteredText);
        }

        private void OpenConfiguration()
        {
            ClearInput();
            mainWindowAccess.OpenConfiguration();
        }

        private void PublishCompleteHintVisible(bool value)
        {
            completeHintVisible = value;
            OnPropertyChanged(nameof(CompleteHintVisible));
        }

        private void PublishEnteredText(string text)
        {
            enteredText = text;
            OnPropertyChanged(nameof(EnteredText));
        }

        private void PublishErrorText(string error)
        {
            errorText = error;
            OnPropertyChanged(nameof(ErrorText));
        }

        private void PublishKeyword(string text)
        {
            keyword = text;
            OnPropertyChanged(nameof(Keyword));
        }

        private void PublishKeywordVisible(bool value)
        {
            keywordVisible = value;
            OnPropertyChanged(nameof(KeywordVisible));
        }

        private void PublishSelectedItemIndex(int index)
        {
            if (suggestions != null && (index < -1 || index >= suggestions.Count))
                throw new ArgumentOutOfRangeException(nameof(index));

            selectedItemIndex = index;
            OnPropertyChanged(nameof(SelectedItemIndex));
            SelectedSuggestionChanged();

            listWindowAccess.EnsureSelectedIsVisible();
        }

        private void PublishShowHint(bool value)
        {
            showHint = value;
            OnPropertyChanged(nameof(ShowHint));
        }

        private void PublishSuggestions(List<SuggestionDTO> suggestions)
        {
            this.suggestions = suggestions;
            OnPropertyChanged(nameof(Suggestions));
            PublishSelectedItemIndex(-1);
        }

        private void SelectedSuggestionChanged()
        {
            SuggestionDTO suggestion = GetSelectedSuggestion();

            if (suggestion != null)
            {
                var text = suggestionData[suggestion.Index].Suggestion.Text;
                PublishEnteredText(text);
                mainWindowAccess.CaretPosition = text.Length;

                var selectedSuggestionData = suggestionData[suggestion.Index];
                if (selectedSuggestionData.Module is IZSuggestionComplete)
                {
                    PublishCompleteHintVisible((selectedSuggestionData.Module as IZSuggestionComplete).CanComplete(currentKeyword?.Keyword.ActionName, selectedSuggestionData.Suggestion));
                }
                else
                    PublishCompleteHintVisible(false);
            }
        }

        private void SelectPreviousSuggestion()
        {
            if (suggestions != null && suggestions.Any())
            {
                if (selectedItemIndex > 0)
                    PublishSelectedItemIndex(selectedItemIndex - 1);
                else
                    PublishSelectedItemIndex(suggestions.Count - 1);
            }
        }

        private void SelectNextSuggestion()
        {
            if (suggestions != null && suggestions.Any())
            {
                if (selectedItemIndex >= 0 && selectedItemIndex < suggestions.Count - 1)
                    PublishSelectedItemIndex(selectedItemIndex + 1);
                else
                    PublishSelectedItemIndex(0);
            }
        }

        private void SetKeywordData(Models.KeywordData action, string keywordText, string enteredText)
        {
            PublishEnteredText(enteredText);
            SetKeywordData(action, keywordText);
        }

        private void SetKeywordData(Models.KeywordData action, string keywordText)
        {
            currentKeyword = new CurrentKeyword(action, $"{keywordText} ");
            UpdateViewmodelKeyword();
        }

        private void InternalSummonWindow()
        {
            ClearInput();

            mainWindowAccess.Show();
            PublishShowHint(true);
            PublishCompleteHintVisible(false);
        }

        private void Shutdown()
        {
            applicationController.Shutdown();
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
            PublishKeyword(currentKeyword?.Keyword.DisplayName);
            PublishKeywordVisible(currentKeyword != null);
        }

        private void DoSwitchToZ()
        {
            windowService.ShowMainWindow();
        }

        private void DoSwitchToProCalc()
        {
            windowService.ShowProCalcWindow();
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        // IEventListener implementations -------------------------------------

        void IEventListener<ShuttingDownEvent>.Receive(ShuttingDownEvent @event)
        {
            configurationService.Configuration.MainWindow.Position = mainWindowAccess.Position;
        }

        void IEventListener<ConfigurationChangedEvent>.Receive(ConfigurationChangedEvent @event)
        {
            HandleConfigurationChanged();
        }

        void IEventListener<PositionChangedEvent>.Receive(PositionChangedEvent @event)
        {
            if (configurationService.Configuration.General.SynchronizeWindowPositions && 
                @event.Origin != this && 
                    ((int)@event.X != (int)mainWindowAccess.Position.X || 
                    (int)@event.Y != (int)mainWindowAccess.Position.Y))
            {
                try
                {
                    // Don't propagate this as an event
                    suspendPositionChangeNotifications = true;
                    mainWindowAccess.Position = new Point(@event.X, @event.Y);
                }
                finally
                {
                    suspendPositionChangeNotifications = false;
                }
            }
        }

        // Public methods -----------------------------------------------------

        public MainViewModel(IGlobalHotkeyService globalHotkeyService,
            IKeywordService keywordService,
            IModuleService moduleService,
            IConfigurationService configurationService,
            IEventBus eventBus,
            IApplicationController applicationController,
            IWindowService windowService)
        {
            this.globalHotkeyService = globalHotkeyService;
            this.keywordService = keywordService;
            this.moduleService = moduleService;
            this.configurationService = configurationService;
            this.eventBus = eventBus;
            this.applicationController = applicationController;
            this.windowService = windowService;

            this.eventBus.Register((IEventListener<ShuttingDownEvent>)this);
            this.eventBus.Register((IEventListener<ConfigurationChangedEvent>)this);
            this.eventBus.Register((IEventListener<PositionChangedEvent>)this);

            this.suggestionData = null;

            currentKeyword = null;

            this.enteredTextTimer = new DispatcherTimer();
            enteredTextTimer.Interval = TimeSpan.FromMilliseconds(this.configurationService.Configuration.Behavior.SuggestionDelay);
            enteredTextTimer.Tick += EnteredTextTimerTick;

            this.helpModule = new HelpModule(this);
            moduleService.AddModule(helpModule);

            ConfigurationCommand = new SimpleCommand((obj) => OpenConfiguration());
            CloseCommand = new SimpleCommand((obj) => Shutdown());
            SwitchToZCommand = new SimpleCommand((obj) => DoSwitchToZ());
            SwitchToProCalcCommand = new SimpleCommand((obj) => DoSwitchToProCalc());

            // Default values

            enteredText = null;
            showHint = true;
            keyword = null;
            keywordVisible = false;
            errorText = null;

            suggestions = null;
            selectedItemIndex = -1;
        }

        public bool BackspacePressed()
        {
            if (mainWindowAccess.CaretPosition == 0 && currentKeyword != null)
            {
                int keywordTextLength = currentKeyword.StoredText.Length;

                PublishEnteredText(currentKeyword.StoredText + enteredText);
                mainWindowAccess.CaretPosition = keywordTextLength;

                ClearKeywordData();
                return true;
            }

            return false;
        }

        public bool DownPressed()
        {
            SelectNextSuggestion();
            return true;
        }

        public bool EnterPressed()
        {
            ExecuteCurrentAction();
            return true;
        }

        public bool EscapePressed()
        {
            if (!IsInputEmpty())
            {
                ClearInput();
            }
            else
            {
                InternalDismissWindow();
            }

            return true;
        }

        public void Initialized()
        {
            mainWindowAccess.Position = configurationService.Configuration.MainWindow.Position;
        }

        public bool SpacePressed()
        {
            if (String.IsNullOrEmpty(enteredText))
                return false;

            int indexOfSpace = enteredText.IndexOf(' ');

            // Check if potential keyword is entered
            if (mainWindowAccess.CaretPosition > 0
                && (indexOfSpace >= mainWindowAccess.CaretPosition || indexOfSpace == -1))
            {
                string possibleKeyword = enteredText.Substring(0, mainWindowAccess.CaretPosition);

                Models.KeywordData action = keywordService.GetKeywordAction(possibleKeyword);
                if (action != null)
                {
                    SetKeywordData(action, possibleKeyword);
                    PublishEnteredText(enteredText.Substring(mainWindowAccess.CaretPosition));
                    mainWindowAccess.CaretPosition = 0;

                    // Module may want to provide suggestions on empty text
                    enteredTextTimer.Start();

                    return true;
                }
            }

            return false;
        }

        public bool TabPressed()
        {
            CompleteSuggestion();
            return true;
        }

        public bool UpPressed()
        {
            SelectPreviousSuggestion();
            return true;
        }

        public void WindowLostFocus()
        {
            // HideWindow();
        }

        public void ListWindowEnterPressed()
        {
            ExecuteCurrentAction();
        }

        public void ListDoubleClick()
        {
            ExecuteCurrentAction();
        }

        // Public properties --------------------------------------------------

        // Main window

        public IMainWindowAccess MainWindowAccess
        {
            set
            {
                if (mainWindowAccess != null)
                    throw new InvalidOperationException("MainWindowAccess can be set only once!");
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                mainWindowAccess = value;
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
                EnteredTextChanged();
            }
        }

        public bool ShowHint => showHint;

        public string Keyword => keyword;

        public bool KeywordVisible => keywordVisible;

        public bool CompleteHintVisible => completeHintVisible;

        public ICommand ConfigurationCommand { get; private set; }

        public ICommand CloseCommand { get; private set; }

        public ICommand SwitchToZCommand { get; private set; }

        public ICommand SwitchToProCalcCommand { get; private set; }

        public string ErrorText => errorText;

        // List window

        public IListWindowAccess ListWindowAccess
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (listWindowAccess != null)
                    throw new InvalidOperationException("Access can be set only once!");

                listWindowAccess = value;
            }
        }

        public IEnumerable<SuggestionDTO> Suggestions => suggestions;

        public int SelectedItemIndex
        {
            get
            {
                return selectedItemIndex;
            }
            set
            {
                selectedItemIndex = value;
                SelectedSuggestionChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
