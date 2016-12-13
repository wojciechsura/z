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
using Z.ViewModels;
using Z.ViewModels.Interfaces;
using Microsoft.Practices.Unity;

namespace Z
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindowAccess
    {
        // Private constants --------------------------------------------------

        private readonly int LIST_WINDOW_MARGIN = 16;
        private readonly int LIST_WINDOW_MIN_HEIGHT = 20;

        // Private fields -----------------------------------------------------

        private readonly IMainWindowViewModel viewModel;
        private readonly ListWindow listWindow;
        private readonly WindowInteropHelper windowInteropHelper;

        // Private methods ----------------------------------------------------

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

        protected override void OnLocationChanged(EventArgs e)
        {
            PositionListWindow();

            base.OnLocationChanged(e);
        }

        private void PositionListWindow()
        {
            // Reposition list window
            var screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);

            int halfScreenHeight = screen.WorkingArea.Height / 2;
            int listWindowMaxHeight = Math.Max(LIST_WINDOW_MIN_HEIGHT, (int)(halfScreenHeight - this.ActualHeight / 2) - LIST_WINDOW_MARGIN);
            int listWindowMinHeight = LIST_WINDOW_MIN_HEIGHT;
            int halfScreenHeightPos = screen.WorkingArea.Top + halfScreenHeight;
            var aboveHalf = this.Top + this.ActualHeight / 2 <= halfScreenHeightPos;

            listWindow.MinHeight = listWindowMinHeight;
            listWindow.MaxHeight = listWindowMaxHeight;

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

            this.Focus();
        }

        void IMainWindowAccess.Hide()
        {
            ((IMainWindowAccess)this).HideList();
            Hide();
        }

        void IMainWindowAccess.ShowList()
        {
            listWindow.Show();
        }

        void IMainWindowAccess.HideList()
        {
            listWindow.Hide();
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

        // Public methods -----------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();
            windowInteropHelper = new WindowInteropHelper(this);

            viewModel = Container.Instance.Resolve<IMainWindowViewModel>();
            viewModel.MainWindowAccess = this;

            this.DataContext = viewModel;

            listWindow = new ListWindow();
        }
    }
}
