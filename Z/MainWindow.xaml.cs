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
using Z.BusinessLogic.Interfaces.ViewModels;

namespace Z
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindowAccess
    {
        // Private constants --------------------------------------------------

        private readonly int LIST_WINDOW_MARGIN = 16;
        private readonly int LIST_WINDOW_HEIGHT = 400;

        // Private fields -----------------------------------------------------

        private readonly IMainWindowViewModel viewModel;
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
                listWindow.Top = this.Top + this.Height + LIST_WINDOW_MARGIN;
            }
            else
            {
                listWindow.Left = this.Left;
                listWindow.Top = this.Top - listWindow.Height - LIST_WINDOW_MARGIN;
            }


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

        // Protected methods --------------------------------------------------

        protected override void OnLocationChanged(EventArgs e)
        {
            PositionListWindow();

            base.OnLocationChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!viewModel.Closing())
                e.Cancel = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            listWindow.Close();
            base.OnClosed(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            viewModel.Initialized();
        }

        // Public methods -----------------------------------------------------

        public MainWindow()
        {
            viewModel = Dependencies.Container.Instance.Resolve<IMainWindowViewModel>();
            viewModel.MainWindowAccess = this;

            InitializeComponent();

            windowInteropHelper = new WindowInteropHelper(this);

            this.DataContext = viewModel;

            listWindow = new ListWindow();
        }
    }
}
