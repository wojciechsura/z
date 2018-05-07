using FavoritesModule.ViewModels;
using FavoritesModule.ViewModels.Interfaces;
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

namespace FavoritesModule.Windows
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window, IConfigurationWindowAccess
    {
        private ConfigurationWindowViewModel viewModel;

        public ConfigurationWindow(ConfigurationWindowViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            viewModel.Access = this;
            this.DataContext = viewModel;
        }

        public void CloseWindow()
        {
            Close();
        }

        private void HandleWindowClosed(object sender, EventArgs e)
        {
            DataContext = null;
            viewModel.Access = null;
            viewModel = null;            
        }
    }
}
