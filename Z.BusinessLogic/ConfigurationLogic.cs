using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Interfaces;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.BusinessLogic
{
    partial class ConfigurationLogic
    {
        private readonly IConfigurationService configurationService;
        private readonly ConfigurationWindowViewModelImplementation viewModel;
 
        public ConfigurationLogic(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;
        }

        public ConfigurationLogic()
        {
            viewModel = new ConfigurationWindowViewModelImplementation(this);
        }

        public IConfigurationWindowViewModel ConfigurationWindowViewModel => viewModel;
    }
}
