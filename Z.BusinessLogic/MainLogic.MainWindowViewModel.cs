using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Z.BusinessLogic.Interfaces.ViewModels;
using Z.BusinessLogic.Types;

namespace Z.BusinessLogic
{
    partial class MainLogic
    {
        class MainWindowViewModelImplementation : INotifyPropertyChanged, IMainWindowViewModel
        {
            private IMainWindowAccess access;
            private MainLogic logic;

            private string keyword;
            private string enteredText;
            private bool keywordVisible;
            private bool showHint;

            private readonly SimpleCommand configurationCommand;

            // Private methods ----------------------------------------------------

            private T Safe<T>(Func<IMainWindowAccess, T> func)
            {
                if (access != null)
                    return func(access);
                else
                    return default(T);
            }

            private void Safe(Action<IMainWindowAccess> action)
            {
                if (access != null)
                    action(access);
            }

            private void HandleOpenConfiguration()
            {
                logic.OpenConfigurationPressed();
            }

            // Internal methods -----------------------------------------------

            internal void ShowWindow()
            {
                Safe(access => access.Show());
            }

            internal void HideWindow()
            {
                Safe(access => access.Hide());
            }

            internal void ShowList()
            {
                Safe(access => access.ShowList());
            }

            internal void HideList()
            {
                Safe(access => access.HideList());
            }

            internal void OpenConfiguration()
            {
                Safe(access => access.OpenConfiguration());
            }

            // Internal properties --------------------------------------------

            internal string InternalEnteredText
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

            internal string InternalKeyword
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

            internal bool InternalKeywordVisible
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

            internal int CaretPosition
            {
                get
                {
                    return Safe(access => access.CaretPosition);
                }
                set
                {
                    Safe(access => access.CaretPosition = value);
                }
            }

            internal bool InternalShowHint
            {
                get
                {
                    return showHint;
                }
                set
                {
                    showHint = value;
                    OnPropertyChanged(nameof(ShowHint));
                }
            }

            internal Point Position
            {
                get
                {
                    return Safe(access => access.Position);
                }
                set
                {
                    Safe(access => access.Position = value);
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

            public MainWindowViewModelImplementation(MainLogic logic)
            {
                configurationCommand = new SimpleCommand((obj) => HandleOpenConfiguration());

                this.logic = logic;
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

            public bool UpPressed()
            {
                return logic.UpPressed();
            }

            public bool DownPressed()
            {
                return logic.DownPressed();
            }

            public void WindowLostFocus()
            {
                logic.WindowLostFocus();
            }

            public bool Closing()
            {
                return logic.WindowClosing();
            }

            public void Initialized()
            {
                logic.WindowInitialized();
            }

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

            public bool ShowHint
            {
                get
                {
                    return showHint;
                }
            }

            public ICommand ConfigurationCommand
            {
                get
                {
                    return configurationCommand;
                }
            }

            public IMainWindowAccess MainWindowAccess
            {
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    if (access != null)
                        throw new InvalidOperationException("Only one main window access is allowed!");

                    access = value;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
