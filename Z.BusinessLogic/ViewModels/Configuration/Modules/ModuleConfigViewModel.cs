using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Z.Api.Interfaces;
using Z.Wpf.Types;

namespace Z.BusinessLogic.ViewModels.Configuration.Modules
{
    public class ModuleConfigViewModel
    {
        private readonly IConfigurationProvider provider;

        public ModuleConfigViewModel(string module, ImageSource icon, IConfigurationProvider provider)
        {
            Module = module;
            Icon = icon;
            this.provider = provider;
            ShowConfigCommand = new SimpleCommand((obj) => provider.Show());
        }

        public string Module { get; private set; }
        public ImageSource Icon { get; private set; }
        public ICommand ShowConfigCommand { get; private set; }
    }
}
