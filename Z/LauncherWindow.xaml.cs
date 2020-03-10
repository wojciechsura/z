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
using Z.BusinessLogic.ViewModels.Main;
using Microsoft.Practices.Unity;

namespace Z
{
    /// <summary>
    /// Interaction logic for LauncherWindow.xaml
    /// </summary>
    public partial class LauncherWindow : Window, ILauncherWindowAccess
    {
        private readonly LauncherViewModel viewModel;

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public LauncherWindow()
        {
            viewModel = Dependencies.Container.Instance.Resolve<MainViewModel>().LauncherViewModel;
            viewModel.LauncherWindowAccess = this;

            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
