using Autofac;
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
using Z.Wpf.Types;
using Z.BusinessLogic;
using Z.BusinessLogic.ViewModels;
using Z.BusinessLogic.ViewModels.Configuration;

namespace Z
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window, IConfigurationWindowAccess
    {
        private ConfigurationViewModel viewModel;

        public ConfigurationWindow()
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<ConfigurationViewModel>();
            viewModel.ConfigurationWindowAccess = this;

            DataContext = viewModel;
        }

        public void CloseWindow()
        {
            Close();
        }

        public void ShowWarning(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
