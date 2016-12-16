using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Interfaces;

namespace Z.BusinessLogic
{
    class ConfigurationLogic : IConfigurationWindowLogic
    {
        private IConfigurationWindowViewModelAccess viewModel;

        private void Safe(Action<IConfigurationWindowViewModelAccess> action)
        {
            if (viewModel != null)
                action(viewModel);
        }

        private T Safe<T>(Func<IConfigurationWindowViewModelAccess, T> func, T defaultValue = default(T))
        {
            if (viewModel != null)
                return func(viewModel);
            else
                return defaultValue;
        }
 
        public IConfigurationWindowViewModelAccess ConfigurationWindowViewModel
        {
            set
            {
                if (viewModel != null)
                    throw new InvalidOperationException("ViewModel can be set only once!");
                if (value == null)
                    throw new ArgumentNullException("value");

                viewModel = value;
            }
        }
    }
}
