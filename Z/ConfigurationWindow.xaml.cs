﻿using System;
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
using Microsoft.Practices.Unity;
using Z.BusinessLogic.Interfaces;
using Z.BusinessLogic.Types;
using Z.BusinessLogic;

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
