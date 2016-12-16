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
using Z.ViewModels.Interfaces;
using Z.ViewModels.Types;
using Microsoft.Practices.Unity;

namespace Z
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window, IConfigurationWindowAccess
    {
        private IConfigurationWindowViewModel viewModel;

        public ConfigurationWindow()
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<IConfigurationWindowViewModel>();
            viewModel.ConfigurationWindowAccess = this;
        }

        public void OpenScreen(ConfigurationScreen screen)
        {
            switch (screen)
            {
                case ConfigurationScreen.General:
                    {
                        break;
                    }
                case ConfigurationScreen.Keywords:
                    {
                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported screen!");
            }
        }
    }
}
