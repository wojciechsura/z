using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Interfaces;

namespace Z.BusinessLogic
{
    partial class ConfigurationLogic
    {
        private class ConfigurationWindowViewModelImplementation : IConfigurationWindowViewModel
        {
            private IConfigurationWindowAccess access;
            private ConfigurationLogic configurationLogic;

            private void Safe(Action<IConfigurationWindowAccess> action)
            {
                if (access != null)
                    action(access);
            }

            public ConfigurationWindowViewModelImplementation(ConfigurationLogic configurationLogic)
            {
                this.configurationLogic = configurationLogic;
            }

            public IConfigurationWindowAccess ConfigurationWindowAccess
            {
                set
                {
                    if (access != null)
                        throw new InvalidOperationException("Access can be set only once!");
                    if (value == null)
                        throw new ArgumentNullException(nameof(value));

                    access = value;
                }
            }
        }
    }
}
