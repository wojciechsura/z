using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.Practices.Unity;
using Z.BusinessLogic.ViewModels.ProCalc;

namespace Z
{
    /// <summary>
    /// Logika interakcji dla klasy ProCalcResultsWindow.xaml
    /// </summary>
    public partial class ProCalcResultsWindow : Window
    {
        private readonly ProCalcViewModel viewModel;

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public ProCalcResultsWindow()
        {
            viewModel = Dependencies.Container.Instance.Resolve<ProCalcViewModel>();

            InitializeComponent();

            this.DataContext = viewModel;
        }        
    }
}
