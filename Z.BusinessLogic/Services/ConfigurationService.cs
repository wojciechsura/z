using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Common;
using Z.BusinessLogic.Services.Interfaces;
using Z.Models.Configuration;

namespace Z.BusinessLogic.Services
{
    class ConfigurationService : IConfigurationService
    {
        private Configuration configuration;

        public ConfigurationService()
        {
            // Defaults
            configuration = new Configuration();
        }

        public Configuration Configuration
        {
            get
            {
                return configuration;
            }
        }
    }
}
