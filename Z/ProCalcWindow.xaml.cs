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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Z.BusinessLogic.ViewModels;
using Z.BusinessLogic.ViewModels.Interfaces;
using Z.Types;
using Microsoft.Practices.Unity;
using System.Windows.Interop;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Z
{
    /// <summary>
    /// Logika interakcji dla klasy ProCalcWindow.xaml
    /// </summary>
    public partial class ProCalcWindow : BaseOperatorWindow, IProCalcWindowAccess
    {
        // Private constants --------------------------------------------------

        private readonly int RESULTS_WINDOW_MARGIN = 16;
        private readonly int RESULTS_WINDOW_HEIGHT = 400;

        // Private fields -----------------------------------------------------

        private ProCalcViewModel viewModel;
        private WindowInteropHelper windowInteropHelper;
        private ProCalcResultsWindow resultsWindow;

        // Private methods ----------------------------------------------------

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void ProCalcKeyDown(object sender, KeyEventArgs e)
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

        private void ProCalcEditKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
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

        private void ProCalcLostFocus(object sender, RoutedEventArgs e)
        {
            viewModel.WindowLostFocus();
        }

        private void ProCalcWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void PositionResultsWindow()
        {
            PositionResultsWindow(this.Width, this.Height);
        }

        private void PositionResultsWindow(double width, double height)
        {
            // Reposition list window
            var screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);

            int halfScreenHeight = screen.WorkingArea.Height / 2;
            int listWindowHeight = Math.Min((int)resultsWindow.ActualHeight, (int)(halfScreenHeight - this.ActualHeight / 2) - RESULTS_WINDOW_MARGIN);
            int halfScreenHeightPos = screen.WorkingArea.Top + halfScreenHeight;
            var aboveHalf = this.Top + this.ActualHeight / 2 <= halfScreenHeightPos;

            resultsWindow.Height = listWindowHeight;

            if (aboveHalf)
            {
                resultsWindow.Left = this.Left;
                resultsWindow.Top = this.Top + height + RESULTS_WINDOW_MARGIN;
            }
            else
            {
                resultsWindow.Left = this.Left;
                resultsWindow.Top = this.Top - resultsWindow.Height - RESULTS_WINDOW_MARGIN;
            }
        }

        private void ProCalcWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PositionResultsWindow(e.NewSize.Width, e.NewSize.Height);
        }

        private void ShowResults()
        {
            resultsWindow.Show();

            // Schedule repositioning of list window
            resultsWindow.Dispatcher.Invoke(() => PositionResultsWindow(), DispatcherPriority.Render);
        }

        private void HideResults()
        {
            resultsWindow.Hide();
        }

        // IProCalcWindowAccess implementation --------------------------------

        void IProCalcWindowAccess.Show()
        {
            Show();
            ShowResults();
            PositionResultsWindow();

            SetForegroundWindow(this.windowInteropHelper.Handle);
        }

        void IProCalcWindowAccess.Hide()
        {
            HideResults();
            Hide();
        }

        void IProCalcWindowAccess.OpenConfiguration()
        {
            ConfigurationWindow configuration = new ConfigurationWindow();
            configuration.ShowDialog();
        }

        int IProCalcWindowAccess.CaretPosition
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

        Point IProCalcWindowAccess.Position
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

        bool IProCalcWindowAccess.IsVisible => this.IsVisible;

        // Protected methods --------------------------------------------------

        protected override void OnLocationChanged(EventArgs e)
        {
            PositionResultsWindow();

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

        public ProCalcWindow()
        {
            viewModel = Dependencies.Container.Instance.Resolve<ProCalcViewModel>();
            viewModel.ProCalcWindowAccess = this;

            InitializeComponent();

            windowInteropHelper = new WindowInteropHelper(this);

            this.DataContext = viewModel;

            resultsWindow = new ProCalcResultsWindow();
        }

        public override void Dismiss()
        {
            viewModel.Dismiss();
        }

        public override void Summon()
        {
            viewModel.Summon();
        }
    }
}
