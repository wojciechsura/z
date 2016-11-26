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
using Z.Viewmodels;

namespace Z
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Private fields -----------------------------------------------------

        private readonly MainWindowViewModel viewModel;

        // Public methods -----------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();

            this.viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
        }
    }
}
