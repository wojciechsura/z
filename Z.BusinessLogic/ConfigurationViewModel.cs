using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Interfaces;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.BusinessLogic
{
    public class ConfigurationViewModel
    {
        private readonly IConfigurationService configurationService;

        private IConfigurationWindowAccess configWindowAccess;

        public ConfigurationViewModel(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;
        }

        public ConfigurationViewModel()
        {

        }

        public IConfigurationWindowAccess ConfigurationWindowAccess
        {
            set
            {
                if (configWindowAccess != null)
                    throw new InvalidOperationException("Access can be set only once!");
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                configWindowAccess = value;
            }
        }

    }
}
