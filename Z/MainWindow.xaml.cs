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
using Z.Types;
using Z.BusinessLogic.ViewModels.Main;
using Z.Controls;

namespace Z
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseOperatorWindow, IMainWindowAccess
    {
        // Private types ------------------------------------------------------

        private class WindowPositioningInfo
        {
            public WindowPositioningInfo(double windowWidth, double windowHeight, double halfScreenWidth, double halfScreenHeight, bool beforeHalf, bool aboveHalf)
            {
                WindowWidth = windowWidth;
                WindowHeight = windowHeight;
                HalfScreenWidth = halfScreenWidth;
                HalfScreenHeight = halfScreenHeight;
                BeforeHalf = beforeHalf;
                AboveHalf = aboveHalf;
            }

            public double WindowWidth { get; }
            public double WindowHeight { get; }
            public double HalfScreenWidth { get; }
            public double HalfScreenHeight { get; }
            public bool BeforeHalf { get; }
            public bool AboveHalf { get; }
        }

        // Private constants --------------------------------------------------

        private readonly int LIST_WINDOW_MARGIN = 16;

        // Private fields -----------------------------------------------------

        private readonly MainViewModel viewModel;
        private readonly ListWindow listWindow;
        private readonly LauncherWindow launcherWindow;

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
                case Key.Left:
                    {
                        bool handled = viewModel.LeftPressed();
                        e.Handled = handled;
                        break;
                    }
                case Key.Right:
                    {
                        bool handled = viewModel.RightPressed();
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

        private void PositionSubWindow(BaseSubWindow subWindow, WindowPositioningInfo positioningInfo)
        {
            subWindow.Reversed = !positioningInfo.AboveHalf;

            if (positioningInfo.AboveHalf)
                subWindow.Top = this.Top + this.ActualHeight + LIST_WINDOW_MARGIN;
            else
                subWindow.Top = this.Top - subWindow.ActualHeight - LIST_WINDOW_MARGIN;

            if (positioningInfo.BeforeHalf)
                subWindow.Left = this.Left;
            else
                subWindow.Left = this.Left + this.ActualWidth - subWindow.ActualWidth;
        }

        private WindowPositioningInfo GetWindowPositioningInfo(double width, double height)
        {
            // Reposition list window
            var screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);

            double halfScreenHeight = screen.WorkingArea.Height / 2;
            double halfScreenHeightPos = screen.WorkingArea.Top + halfScreenHeight;
            bool aboveHalf = this.Top + height / 2 <= halfScreenHeightPos;

            double halfScreenWidth = screen.WorkingArea.Width / 2;
            double halfScreenWidthPos = screen.WorkingArea.Left + halfScreenWidth;
            bool beforeHalf = this.Left + width / 2 <= halfScreenWidthPos;

            return new WindowPositioningInfo(width, height, halfScreenWidth, halfScreenHeight, beforeHalf, aboveHalf);
        }

        private void PositionSubWindows(WindowPositioningInfo positioningInfo)
        {
            PositionSubWindow(listWindow, positioningInfo);
            PositionSubWindow(launcherWindow, positioningInfo);
        }

        private void MainWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var positionInfo = GetWindowPositioningInfo(e.NewSize.Width, e.NewSize.Height);
            PositionSubWindows(positionInfo);
        }

        private void GearButtonClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsOpen = true;
        }

        // IMainViewModelAccess implementation --------------------------------

        void IMainWindowAccess.Show()
        {
            Show();

            var positionInfo = GetWindowPositioningInfo(this.ActualWidth, this.ActualHeight);
            PositionSubWindows(positionInfo);

            SetForegroundWindow(this.windowInteropHelper.Handle);
        }

        void IMainWindowAccess.Hide()
        {
            ((IMainWindowAccess)this).HideLauncher();
            ((IMainWindowAccess)this).HideList();
            Hide();
        }

        void IMainWindowAccess.ShowList()
        {
            ((IMainWindowAccess)this).HideLauncher();
            listWindow.Show();

            // Schedule repositioning of list window
            listWindow.Dispatcher.Invoke(() => 
            {
                var positionInfo = GetWindowPositioningInfo(this.ActualWidth, this.ActualHeight);
                PositionSubWindows(positionInfo);
            }, DispatcherPriority.Render);
        }

        void IMainWindowAccess.ShowLauncher()
        {
            ((IMainWindowAccess)this).HideList();
            launcherWindow.Show();

            // Schedule repositioning of list window
            launcherWindow.Dispatcher.Invoke(() =>
            {
                var positionInfo = GetWindowPositioningInfo(this.ActualWidth, this.ActualHeight);
                PositionSubWindows(positionInfo);
            }, DispatcherPriority.Render);
        }

        void IMainWindowAccess.HideList()
        {
            listWindow.Hide();
        }

        void IMainWindowAccess.HideLauncher()
        {
            launcherWindow.Hide();
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

        Point IMainWindowAccess.RelativePosition
        {
            get
            {
                return EvalRelativePosition();
            }
            set
            {
                SetRelativePosition(value);
            }
        }

        bool IMainWindowAccess.IsVisible => this.IsVisible;

        // Protected methods --------------------------------------------------

        protected override void OnLocationChanged(EventArgs e)
        {
            var positionInfo = GetWindowPositioningInfo(this.ActualWidth, this.ActualHeight);
            PositionSubWindows(positionInfo);

            viewModel.NotifyPositionChanged((int)Left, (int)Top);

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

            this.DataContext = viewModel;

            listWindow = new ListWindow();

            launcherWindow = new LauncherWindow();
            launcherWindow.SizeChanged += HandleLauncherWindowSizeChanged;
        }

        private void HandleLauncherWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var positionInfo = GetWindowPositioningInfo(this.ActualWidth, this.ActualHeight);

            PositionSubWindow(launcherWindow, positionInfo);
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
