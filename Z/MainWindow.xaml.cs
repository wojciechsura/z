using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Z.Common;
using Z.Dependencies;
using Microsoft.Practices.Unity;
using System.Windows.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Z.BusinessLogic;
using Z.BusinessLogic.ViewModels;
using Z.BusinessLogic.ViewModels.Interfaces;
using Z.Types;

namespace Z
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseOperatorWindow, IMainWindowAccess
    {
        // Private constants --------------------------------------------------

        private readonly int LIST_WINDOW_MARGIN = 16;
        private readonly int LIST_WINDOW_HEIGHT = 400;

        // Private fields -----------------------------------------------------

        private readonly MainViewModel viewModel;
        private readonly ListWindow listWindow;
        private readonly WindowInteropHelper windowInteropHelper;

        // Private methods ----------------------------------------------------

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void MainKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        bool handled = viewModel.EscapePressed();
                        e.Handled = handled;
                        break;
                    }
                default:
                    {
                        e.Handled = false;
                        break;
                    }
            }
        }

        private void MainEditKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    {
                        bool handled = viewModel.SpacePressed();
                        e.Handled = handled;
                        break;
                    }
                case Key.Back:
                    {
                        bool handled = viewModel.BackspacePressed();
                        e.Handled = handled;
                        break;
                    }
                case Key.Tab:
                    {
                        bool handled = viewModel.TabPressed();
                        e.Handled = handled;
                        break;
                    }
                case Key.Enter:
                    {
                        bool handled = viewModel.EnterPressed();
                        e.Handled = handled;
                        break;
                    }
                case Key.Up:
                    {
                        bool handled = viewModel.UpPressed();
                        e.Handled = handled;
                        break;
                    }
                case Key.Down:
                    {
                        bool handled = viewModel.DownPressed();
                        e.Handled = handled;
                        break;
                    }
                default:
                    {
                        e.Handled = false;
                        break;
                    }
            }
        }

        private void MainLostFocus(object sender, RoutedEventArgs e)
        {
            viewModel.WindowLostFocus();
        }

        private void MainWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void PositionListWindow()
        {
            PositionListWindow(this.Width, this.Height);
        }

        private void PositionListWindow(double width, double height)
        {
            // Reposition list window
            var screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);

            int halfScreenHeight = screen.WorkingArea.Height / 2;
            int listWindowHeight = Math.Min(LIST_WINDOW_HEIGHT, (int)(halfScreenHeight - this.ActualHeight / 2) - LIST_WINDOW_MARGIN);
            int halfScreenHeightPos = screen.WorkingArea.Top + halfScreenHeight;
            var aboveHalf = this.Top + this.ActualHeight / 2 <= halfScreenHeightPos;

            listWindow.Height = listWindowHeight;

            if (aboveHalf)
            {
                listWindow.Left = this.Left;
                listWindow.Top = this.Top + height + LIST_WINDOW_MARGIN;
            }
            else
            {
                listWindow.Left = this.Left;
                listWindow.Top = this.Top - listWindow.Height - LIST_WINDOW_MARGIN;
            }
        }

        private void MainWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PositionListWindow(e.NewSize.Width, e.NewSize.Height);
        }

        private void GearButtonClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsOpen = true;
        }

        // IMainViewModelAccess implementation --------------------------------

        void IMainWindowAccess.Show()
        {
            Show();
            PositionListWindow();

            SetForegroundWindow(this.windowInteropHelper.Handle);
        }

        void IMainWindowAccess.Hide()
        {
            ((IMainWindowAccess)this).HideList();
            Hide();
        }

        void IMainWindowAccess.ShowList()
        {
            listWindow.Show();

            // Schedule repositioning of list window
            listWindow.Dispatcher.Invoke(() => PositionListWindow(), DispatcherPriority.Render);
        }

        void IMainWindowAccess.HideList()
        {
            listWindow.Hide();
        }

        void IMainWindowAccess.OpenConfiguration()
        {
            ConfigurationWindow configuration = new ConfigurationWindow();
            configuration.ShowDialog();
        }

        bool? IMainWindowAccess.SelectSuggestion(SuggestionChoiceViewModel suggestionChoiceViewModel)
        {
            SuggestionChoiceWindow win = new SuggestionChoiceWindow(suggestionChoiceViewModel);
            return win.ShowDialog();
        }

        int IMainWindowAccess.CaretPosition
        {
            get
            {
                return MainEdit.CaretIndex;
            }
            set
            {
                MainEdit.CaretIndex = value;
            }
        }

        Point IMainWindowAccess.Position
        {
            get
            {
                return new Point(Left, Top);
            }
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        bool IMainWindowAccess.IsVisible => this.IsVisible;

        // Protected methods --------------------------------------------------

        protected override void OnLocationChanged(EventArgs e)
        {
            PositionListWindow();

            base.OnLocationChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            viewModel.Initialized();
        }

        // Public methods -----------------------------------------------------

        public MainWindow()
        {
            viewModel = Dependencies.Container.Instance.Resolve<MainViewModel>();
            viewModel.MainWindowAccess = this;

            InitializeComponent();

            windowInteropHelper = new WindowInteropHelper(this);

            this.DataContext = viewModel;

            listWindow = new ListWindow();
        }

        public override void Summon()
        {
            viewModel.Summon();
        }

        public override void Dismiss()
        {
            viewModel.Dismiss();
        }

        private void ZLauncherMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.SwitchToZCommand.CanExecute(null))
                viewModel.SwitchToZCommand.Execute(null);
        }

        private void ProCalcMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.SwitchToProCalcCommand.CanExecute(null))
                viewModel.SwitchToProCalcCommand.Execute(null);
        }

        private void ConfigurationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.ConfigurationCommand.CanExecute(null))
                viewModel.ConfigurationCommand.Execute(null);
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.CloseCommand.CanExecute(null))
                viewModel.CloseCommand.Execute(null);
        }
    }
}
