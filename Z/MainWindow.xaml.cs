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
using Z.Services.Interfaces;
using Z.Viewmodels;
using Z.Viewmodels.Interfaces;
using Microsoft.Practices.Unity;

namespace Z
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainViewModelAccess
    {
        // Private fields -----------------------------------------------------

        private readonly MainWindowViewModel viewModel;

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

        // IMainViewModelAccess implementation --------------------------------

        void IMainViewModelAccess.Show()
        {
            Show();
        }

        void IMainViewModelAccess.Hide()
        {
            Hide();
        }

        void IMainViewModelAccess.ShowError(string error)
        {
            MessageBox.Show(error);
        }

        void IMainViewModelAccess.Shutdown()
        {
            Application.Current.Shutdown();
        }

        int IMainViewModelAccess.CaretPosition
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

            this.viewModel = new MainWindowViewModel(this, 
                Configuration.Container.Resolve<IHotkeyService>(),
                Configuration.Container.Resolve<IKeywordService>());
            this.DataContext = viewModel;
        }
    }
}
