using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Practices.Unity;
using System.Diagnostics;
using Z.Api.Interfaces;
using Z.Api.Types;
using System.ComponentModel;
using System.Windows.Media;
using Z.Wpf.Types;
using Z.Common.Types;
using Z.Api;
using Z.BusinessLogic.Events;
using System.Windows;
using Z.BusinessLogic.ViewModels.Base;
using Z.BusinessLogic.Services.EventBus;
using Z.BusinessLogic.Services.Module;
using Z.BusinessLogic.Services.GlobalHotkey;
using Z.BusinessLogic.Services.Keyword;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.Services.Application;
using Z.BusinessLogic.Services.AppWindows;
using Z.BusinessLogic.Models;

namespace Z.BusinessLogic.ViewModels.Main
{
    public class MainViewModel : BaseViewModel, IMainHandler,
        IEventListener<ShuttingDownEvent>, 
        IEventListener<ConfigurationChangedEvent>, 
        IEventListener<PositionChangedEvent>
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
        private readonly IAppWindowService windowService;

        private readonly LauncherViewModel launcherViewModel;
        private readonly ListViewModel listViewModel;

        private readonly DispatcherTimer enteredTextTimer;

        private readonly HelpModule helpModule;

        private CurrentKeyword currentKeyword;
        private List<SuggestionData> suggestionData;

        private IMainWindowAccess mainWindowAccess;

        // Main window

        private string enteredText;
        private bool showHint;
        private string keyword;
        private bool keywordVisible;
        private bool completeHintVisible;
        private string errorText;
        private bool suspendPositionChangeNotifications = false;

        // Private methods ----------------------------------------------------

        private void ClearInput()
        {
            EnteredText = null;
            ErrorText = null;
            CompleteHintVisible = false;
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
            listViewModel.Suggestions = null;

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

                    List<SuggestionViewModel> suggestions = suggestionData
                        .Select(sd => new SuggestionViewModel(sd))
                        .ToList();

                    listViewModel.Suggestions = suggestions;
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
            if (!suspendPositionChangeNotifications)
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
            var suggestion = listViewModel.SelectedSuggestion;
            if (suggestion != null)
            {
                var selectedSuggestionData = suggestion.SuggestionData;
                IZModule module = selectedSuggestionData.Module;

                if (module is IZSuggestionComplete && 
                    ((IZSuggestionComplete)module).CanComplete(currentKeyword?.Keyword.ActionName, selectedSuggestionData.Suggestion))
                {
                    string replace = ((IZSuggestionComplete)module).Complete(currentKeyword?.Keyword.ActionName, selectedSuggestionData.Suggestion);

                    EnteredText = replace;
                    if (replace.Length > 0)
                        mainWindowAccess.CaretPosition = replace.Length;
                    StartEnteredTextTimer();

                    CompleteHintVisible = false;
                }
            }
        }

        private void EnteredTextChanged()
        {
            StartEnteredTextTimer();
            ShowHint = false;
            CompleteHintVisible = false;
            ErrorText = null;
        }

        private void EnteredTextTimerTick(object sender, EventArgs e)
        {
            StopEnteredTextTimer();
            CollectSuggestions();
        }

        private void DoExecuteCurrentAction()
        {
            // Stopping timer
            enteredTextTimer.Stop();

            ExecuteOptions options = new ExecuteOptions();

            if (listViewModel.SelectedSuggestion != null)
            {
                // TODO store suggestion data in the DTO
                SuggestionData suggestion = listViewModel.SelectedSuggestion.SuggestionData;
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
                            if (listViewModel.Suggestions != null && listViewModel.Suggestions.Count > 0)
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
                                SuggestionChoiceViewModel suggestionChoiceViewModel = new SuggestionChoiceViewModel(listViewModel.Suggestions);
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

            ErrorText = options.ErrorText;
        }

        private void HandleConfigurationChanged()
        {
            enteredTextTimer.Interval = TimeSpan.FromMilliseconds(configurationService.Configuration.Behavior.SuggestionDelay);
        }

        private void HandleListViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ListViewModel.SelectedItemIndex))
            {
                SuggestionViewModel suggestion = listViewModel.SelectedSuggestion;

                if (suggestion != null)
                {
                    var text = suggestion.SuggestionData.Suggestion.Text;
                    
                    // TODO Silent change is required here - implementation is not perfect though
                    enteredText = text;
                    OnPropertyChanged(() => EnteredText);
                    
                    mainWindowAccess.CaretPosition = text.Length;

                    var selectedSuggestionData = suggestion.SuggestionData;
                    if (selectedSuggestionData.Module is IZSuggestionComplete)
                    {
                        CompleteHintVisible = (selectedSuggestionData.Module as IZSuggestionComplete).CanComplete(currentKeyword?.Keyword.ActionName, selectedSuggestionData.Suggestion);
                    }
                    else
                        CompleteHintVisible = false;
                }
            }
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

        private void SetKeywordData(Models.KeywordData action, string keywordText, string enteredText)
        {
            EnteredText = enteredText;
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
            ShowHint = true;
            CompleteHintVisible = false;
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
            Keyword = currentKeyword?.Keyword.DisplayName;
            KeywordVisible = currentKeyword != null;
        }

        private void DoSwitchToZ()
        {
            windowService.ShowMainWindow();
        }

        private void DoSwitchToProCalc()
        {
            windowService.ShowProCalcWindow();
        }

        // IMainHandler implementation ----------------------------------------

        void IMainHandler.ExecuteCurrentAction()
        {
            DoExecuteCurrentAction();
        }

        // IEventListener implementations -------------------------------------

        void IEventListener<ShuttingDownEvent>.Receive(ShuttingDownEvent @event)
        {
            configurationService.Configuration.MainWindow.RelativePosition = mainWindowAccess.RelativePosition;
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
            IAppWindowService windowService)
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

            // Dependent viewmodels

            listViewModel = new ListViewModel(this);
            listViewModel.PropertyChanged += HandleListViewModelPropertyChanged;
            
            launcherViewModel = new LauncherViewModel(configurationService);
        }

        public bool BackspacePressed()
        {
            if (mainWindowAccess.CaretPosition == 0 && currentKeyword != null)
            {
                int keywordTextLength = currentKeyword.StoredText.Length;

                EnteredText = currentKeyword.StoredText + enteredText;
                mainWindowAccess.CaretPosition = keywordTextLength;

                ClearKeywordData();
                return true;
            }

            return false;
        }

        public bool DownPressed()
        {
            listViewModel.SelectNextSuggestion();
            return true;
        }

        public bool EnterPressed()
        {
            DoExecuteCurrentAction();
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
            mainWindowAccess.RelativePosition = configurationService.Configuration.MainWindow.RelativePosition;
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
                    EnteredText = enteredText.Substring(mainWindowAccess.CaretPosition);
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
            listViewModel.SelectPreviousSuggestion();
            return true;
        }

        public void WindowLostFocus()
        {
            // HideWindow();
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
            get => enteredText;
            set => Set(ref enteredText, () => EnteredText, value, EnteredTextChanged);            
        }

        public bool ShowHint
        {
            get => showHint;
            set => Set(ref showHint,() => ShowHint, value);
        }

        public string Keyword
        {
            get => keyword;
            set => Set(ref keyword, () => Keyword, value);
        }

        public bool KeywordVisible
        {
            get => keywordVisible;
            set => Set(ref keywordVisible, () => KeywordVisible, value);
        }

        public bool CompleteHintVisible
        {
            get => completeHintVisible;
            set => Set(ref completeHintVisible, () => CompleteHintVisible, value);
        }

        public ICommand ConfigurationCommand { get; private set; }

        public ICommand CloseCommand { get; private set; }

        public ICommand SwitchToZCommand { get; private set; }

        public ICommand SwitchToProCalcCommand { get; private set; }

        public string ErrorText
        {
            get => errorText;
            set => Set(ref errorText, () => ErrorText, value);
        }

        // List window

        public ListViewModel ListViewModel => listViewModel;
    }
}
