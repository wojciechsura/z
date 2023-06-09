﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
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
using Z.BusinessLogic.Types.Main;
using Z.BusinessLogic.ViewModels.Main.Launcher;
using Z.BusinessLogic.Services.Image;
using Z.Resources;

namespace Z.BusinessLogic.ViewModels.Main
{
    public class MainViewModel : BaseViewModel, IMainHandler, ILauncherHandler,
        IEventListener<ShuttingDownEvent>, 
        IEventListener<ConfigurationChangedEvent>, 
        IEventListener<PositionChangedEvent>
    {
        // Private types ------------------------------------------------------

        private class CurrentKeywordInfo
        {
            public CurrentKeywordInfo(KeywordData keyword, string storedText)
            {
                Keyword = keyword;
                StoredText = storedText;
            }

            public KeywordData Keyword { get; }
            public string StoredText { get; }
        }

        private class HelpModule : IZModule, IZExclusiveSuggestions
        {
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

            public string DisplayName => Strings.Z_HelpModule_ModuleDisplayName;

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
        private readonly IImageResources imageResources;
        private readonly LauncherViewModel launcherViewModel;
        private readonly ListViewModel listViewModel;

        private readonly DispatcherTimer enteredTextTimer;

        private readonly HelpModule helpModule;

        private CurrentKeywordInfo currentKeyword;
        private List<SuggestionData> suggestionData;

        private IMainWindowAccess mainWindowAccess;

        // Main window

        private MainWorkingMode workingMode;
        private string enteredText;
        private bool showHint;
        private string keyword;
        private bool keywordVisible;
        private bool completeHintVisible;
        private string errorText;
        private bool suspendPositionChangeNotifications = false;

        // Private methods ----------------------------------------------------

        private void SilentSetEnteredText(string newText)
        {
            enteredText = newText;
            OnPropertyChanged(() => EnteredText);
        }

        private void ClearInput()
        {
            SilentSetEnteredText(null);

            ErrorText = null;
            CompleteHintVisible = false;

            ClearSuggestions();
            ClearKeywordData();

            WorkingMode = MainWorkingMode.Idle;
        }

        private void ClearKeywordData()
        {
            CurrentKeyword = null;
        }

        private void ClearSuggestions()
        {
            suggestionData = null;
            listViewModel.Suggestions = null;
        }

        private void CollectSuggestions()
        {
            if (workingMode != MainWorkingMode.SuggestionList)
                System.Diagnostics.Debug.WriteLine("CollectSuggestions not in suggestion list mode!");
                          
            if (!String.IsNullOrEmpty(enteredText) || currentKeyword != null)
            {
                suggestionData = moduleService.GetSuggestionsFor(enteredText, currentKeyword?.Keyword);

                var moduleMatchStats = suggestionData
                    .GroupBy(sg => sg.Module)
                    .ToDictionary(g => g.Key, g => new { MaxMatch = g.Max(s => s.Suggestion.Match), AvgMatch = g.Average(s => s.Suggestion.Match) } );

                var averageGroupMatches = suggestionData.GroupBy(sd => sd.Suggestion.SortGroup)
                    .Select(g => new { Name = g.Key ?? string.Empty, Match = g.Average(sd => (double)sd.Suggestion.Match) })
                    .ToDictionary(x => x.Name, x => x.Match);

                if (suggestionData.Count > 0)
                {
                    Func<SuggestionData, SuggestionData, int> compareDisplayNames = (x, y) => String.Compare(x.Suggestion.Display, y.Suggestion.Display);

                    Func<SuggestionData, SuggestionData, int> compareModules = (x, y) =>
                    {
                        // Promote modules, which maximum match is bigger than others and then
                        // those, which average match is bigger than others. Finally, sort by name.

                        var diff = (int)moduleMatchStats[y.Module].MaxMatch - (int)moduleMatchStats[x.Module].MaxMatch;
                        if (diff != 0)
                            return diff;

                        diff = (int)moduleMatchStats[y.Module].AvgMatch - (int)moduleMatchStats[x.Module].AvgMatch;
                        if (diff != 0)
                            return diff;

                        diff = String.Compare(x.Module.DisplayName, y.Module.DisplayName);
                        if (diff != 0)
                            return diff;

                        return (int)y.Suggestion.Match - (int)x.Suggestion.Match;
                    };

                    Func<SuggestionData, SuggestionData, int> compareMatches = (x, y) => (int)y.Suggestion.Match - (int)x.Suggestion.Match;

                    Func<SuggestionData, SuggestionData, int> compareGroups = (x, y) =>
                    {
                        if (String.IsNullOrEmpty(x.Suggestion.SortGroup) && String.IsNullOrEmpty(y.Suggestion.SortGroup))
                            return 0;
                        else if (String.IsNullOrEmpty(x.Suggestion.SortGroup))
                            return 1;
                        else if (String.IsNullOrEmpty(y.Suggestion.SortGroup))
                            return -1;
                        else
                        {
                            if (averageGroupMatches.TryGetValue(x.Suggestion.SortGroup, out double first) && averageGroupMatches.TryGetValue(y.Suggestion.SortGroup, out double second))
                            {
                                if (first > second)
                                    return -1;
                                else if (first < second)
                                    return 1;
                                else
                                    return 0;
                            }
                            else
                                return string.Compare(x.Suggestion.SortGroup, y.Suggestion.SortGroup);
                        }
                    };

                    Func<SuggestionData, SuggestionData, IEnumerable<Func<SuggestionData, SuggestionData, int>>, int> combineCompares =
                        (x, y, compares) =>
                        {
                            return compares.Select(c => c(x, y))
                                .FirstOrDefault(r => r != 0);
                        };

                    switch (configurationService.Configuration.Behavior.SuggestionSorting)
                    {
                        case SuggestionSorting.ByModule:
                            {
                                suggestionData.Sort((x, y) => combineCompares(x, y, new[] { compareGroups, compareModules, compareDisplayNames }));
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

        private void CompleteSuggestion()
        {
            if (workingMode != MainWorkingMode.SuggestionList)
                System.Diagnostics.Debug.WriteLine("CompleteSuggestion not in suggestion list mode!");

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

            if (EnteredText != null && EnteredText.Length > 0)
            {
                WorkingMode = MainWorkingMode.SuggestionList;
                CollectSuggestions();
            }
            else
            {
                ClearSuggestions();
                WorkingMode = MainWorkingMode.Idle;
            }
        }

        private void DoExecuteCurrentAction()
        {
            if (workingMode != MainWorkingMode.SuggestionList)
                System.Diagnostics.Debug.WriteLine("DoExecuteCurrentAction not in suggestion list mode!");

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
                                options.ErrorText = string.Format(Strings.Z_Message_CannotExecute, e.Message);
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
            SetSuggestionGroupBy();
        }

        private void HandleCurrentKeywordChanged()
        {
            Keyword = currentKeyword?.Keyword.DisplayName;
            KeywordVisible = currentKeyword != null;
        }

        private void HandleListViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (workingMode != MainWorkingMode.SuggestionList)
                System.Diagnostics.Debug.WriteLine("HandleListViewModelPropertyChanged not in suggestion list mode!");

            if (e.PropertyName == nameof(ListViewModel.SelectedItemIndex))
            {
                SuggestionViewModel suggestion = listViewModel.SelectedSuggestion;

                if (suggestion != null)
                {
                    var text = suggestion.SuggestionData.Suggestion.Text;

                    SilentSetEnteredText(text);
                    
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

        private void HandleWorkingModeChanged()
        {
            switch (workingMode)
            {
                case MainWorkingMode.Idle:
                    mainWindowAccess.HideList();
                    mainWindowAccess.HideLauncher();
                    break;
                case MainWorkingMode.SuggestionList:
                    mainWindowAccess.HideLauncher();
                    mainWindowAccess.ShowList();
                    break;
                case MainWorkingMode.Launcher:
                    mainWindowAccess.ShowLauncher();
                    mainWindowAccess.HideList();
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unsupported working mode!");
            }
        }

        private void InternalDismissWindow()
        {
            mainWindowAccess.Hide();

            ClearInput();
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
            if (workingMode != MainWorkingMode.SuggestionList)
                System.Diagnostics.Debug.WriteLine("SetKeywordData not in suggestion list mode!");

            EnteredText = enteredText;
            SetKeywordData(action, keywordText);
        }

        private void SetKeywordData(Models.KeywordData action, string keywordText)
        {
            if (workingMode != MainWorkingMode.SuggestionList)
                System.Diagnostics.Debug.WriteLine("SetKeywordData not in suggestion list mode!");

            CurrentKeyword = new CurrentKeywordInfo(action, $"{keywordText} ");
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

        private void DoSwitchToZ()
        {
            windowService.ShowMainWindow();
        }

        private void DoSwitchToProCalc()
        {
            windowService.ShowProCalcWindow();
        }

        private void EnterLauncherMode()
        {
            launcherViewModel.Init();
            WorkingMode = MainWorkingMode.Launcher;
        }

        private void ExitLauncherMode()
        {
            WorkingMode = MainWorkingMode.Idle;
            launcherViewModel.Clear();
        }

        private void SetSuggestionGroupBy()
        {
            switch (configurationService.Configuration.Behavior.SuggestionSorting)
            {
                case SuggestionSorting.ByModule:
                    listViewModel.SuggestionGroupByProperty = nameof(SuggestionViewModel.Module);
                    break;
                case SuggestionSorting.ByDisplay:
                case SuggestionSorting.ByMatch:
                    listViewModel.SuggestionGroupByProperty = null;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unsupported suggestion sorting!");
            }
        }

        // Private properties -------------------------------------------------

        private CurrentKeywordInfo CurrentKeyword
        {
            get => currentKeyword;
            set => Set(ref currentKeyword, () => CurrentKeyword, value, HandleCurrentKeywordChanged);
        }

        // ILauncherHandler implementation ------------------------------------

        void ILauncherHandler.ExitLauncher()
        {
            ExitLauncherMode();
        }

        void ILauncherHandler.ExecuteShortcut(Models.Configuration.LauncherShortcut shortcut)
        {
            try
            {
                string path = shortcut.Command;
                string parameters = null;

                if (shortcut.Command.StartsWith("\""))
                {
                    int closingQuote = shortcut.Command.IndexOf('"', 1);
                    if (closingQuote >= 0)
                    {
                        path = shortcut.Command.Substring(1, closingQuote - 1);
                        if (closingQuote < shortcut.Command.Length - 1)
                            parameters = shortcut.Command.Substring(closingQuote + 1);
                    }
                }
                else if (shortcut.Command.Contains(" "))
                {
                    int space = shortcut.Command.IndexOf(' ');

                    path = shortcut.Command.Substring(0, space);
                    if (space < shortcut.Command.Length - 1)
                        parameters = shortcut.Command.Substring(space + 1);
                }

                Process.Start(path, parameters);
                InternalDismissWindow();
            }
            catch(Exception e)
            {
                ErrorText = string.Format(Strings.Z_Message_FailedToOpenShortcut, e.Message);
            }
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
            IAppWindowService windowService,
            IImageResources imageResources)
        {
            this.globalHotkeyService = globalHotkeyService;
            this.keywordService = keywordService;
            this.moduleService = moduleService;
            this.configurationService = configurationService;
            this.eventBus = eventBus;
            this.applicationController = applicationController;
            this.windowService = windowService;
            this.imageResources = imageResources;

            this.eventBus.Register((IEventListener<ShuttingDownEvent>)this);
            this.eventBus.Register((IEventListener<ConfigurationChangedEvent>)this);
            this.eventBus.Register((IEventListener<PositionChangedEvent>)this);

            this.suggestionData = null;

            workingMode = MainWorkingMode.Idle;

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

            SetSuggestionGroupBy();            
            
            launcherViewModel = new LauncherViewModel(this, imageResources, configurationService);
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

        public void Dismiss()
        {
            InternalDismissWindow();
        }

        public bool DownPressed()
        {
            switch (workingMode)
            {
                case MainWorkingMode.Idle:
                    if (!Reversed)
                        EnterLauncherMode();                    
                    break;
                case MainWorkingMode.SuggestionList:
                    listViewModel.SelectNextSuggestion();
                    break;
                case MainWorkingMode.Launcher:
                    launcherViewModel.MoveDown();
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unsupported working mode");
            }

            return true;
        }

        public bool EnterPressed()
        {
            switch (workingMode)
            {
                case MainWorkingMode.Idle:
                    // Nothing to do
                    break;
                case MainWorkingMode.SuggestionList:
                    DoExecuteCurrentAction();
                    break;
                case MainWorkingMode.Launcher:
                    launcherViewModel.EnterPressed();
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unsupported working mode");
            }
            
            return true;
        }

        public bool EscapePressed()
        {
            switch (workingMode)
            {
                case MainWorkingMode.Idle:
                    InternalDismissWindow();
                    break;
                case MainWorkingMode.SuggestionList:
                    ClearInput();
                    break;
                case MainWorkingMode.Launcher:
                    ExitLauncherMode();
                    break;
            }

            return true;
        }

        public void Initialized()
        {
            mainWindowAccess.RelativePosition = configurationService.Configuration.MainWindow.RelativePosition;
        }

        public bool LeftPressed()
        {
            switch (workingMode)
            {
                case MainWorkingMode.Idle:
                    return false;
                case MainWorkingMode.SuggestionList:
                    return false;
                case MainWorkingMode.Launcher:
                    launcherViewModel.MoveLeft();
                    return true;                    
                default:
                    throw new InvalidEnumArgumentException("Unsupported working mode!");
            }
        }

        public void NotifyPositionChanged(int left, int top)
        {
            if (!suspendPositionChangeNotifications)
                eventBus.Send(new PositionChangedEvent(left, top, this));
        }

        public bool RightPressed()
        {
            switch (workingMode)
            {
                case MainWorkingMode.Idle:
                    return false;
                case MainWorkingMode.SuggestionList:
                    return false;
                case MainWorkingMode.Launcher:
                    launcherViewModel.MoveRight();
                    return true;
                default:
                    throw new InvalidEnumArgumentException("Unsupported working mode!");
            }
        }

        public bool SpacePressed()
        {
            switch (workingMode)
            {
                case MainWorkingMode.Idle:
                case MainWorkingMode.SuggestionList:
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
                case MainWorkingMode.Launcher:
                    // TODO exit launcher
                    WorkingMode = MainWorkingMode.Idle;
                    return false;
                default:
                    throw new InvalidEnumArgumentException("Unsupported working mode!");
            }            
        }

        public void Summon()
        {
            InternalSummonWindow();
        }

        public bool TabPressed()
        {
            switch (workingMode)
            {
                case MainWorkingMode.Idle:
                    // Nothing to do, consume tab
                    return true;
                case MainWorkingMode.SuggestionList:
                    CompleteSuggestion();
                    return true;
                case MainWorkingMode.Launcher:
                    // Nothing to do, consume tab
                    return true;
                default:
                    throw new InvalidEnumArgumentException("Unsupported working mode!");
            }            
        }

        public bool UpPressed()
        {
            switch (workingMode)
            {
                case MainWorkingMode.Idle:
                    if (Reversed)
                        EnterLauncherMode();
                    break;
                case MainWorkingMode.SuggestionList:
                    listViewModel.SelectPreviousSuggestion();
                    break;
                case MainWorkingMode.Launcher:
                    launcherViewModel.MoveUp();
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unsupported working mode");
            }

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

        public bool Reversed { get; set; }

        public MainWorkingMode WorkingMode
        {
            get => workingMode;
            set => Set(ref workingMode, () => WorkingMode, value, HandleWorkingModeChanged);
        }

        public ListViewModel ListViewModel => listViewModel;

        public LauncherViewModel LauncherViewModel => launcherViewModel;
    }
}
