using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Interfaces;
using Z.ViewModels.Interfaces;

namespace Z.ViewModels
{
    class ConfigurationWindowViewModel : IConfigurationWindowViewModel, IConfigurationWindowViewModelAccess
    {
        private IConfigurationWindowLogic logic;
        private IConfigurationWindowAccess access;

        private void Safe(Action<IConfigurationWindowAccess> action)
        {
            if (access != null)
                action(access);
        }

        private T Safe<T>(Func<IConfigurationWindowAccess, T> func, T defaultValue = default(T))
        {
            if (access != null)
                return func(access);
            else
                return defaultValue;
        }

        public ConfigurationWindowViewModel(IConfigurationWindowLogic logic)
        {
            this.logic = logic;
            logic.ConfigurationWindowViewModel = this;
        }

        public IConfigurationWindowAccess ConfigurationWindowAccess
        {
            set
            {
                if (access != null)
                    throw new InvalidOperationException("Access can be set only once!");
                if (value == null)
                    throw new ArgumentNullException("value");

                access = value;
            }
        }
    }
}
