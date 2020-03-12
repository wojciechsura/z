using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.ViewModels.Base;

namespace Z.BusinessLogic.ViewModels.Configuration.Base
{
    public abstract class BaseConfigurationViewModel : BaseViewModel
    {
        public abstract string DisplayName { get; }
        public abstract void Save();
        public abstract IEnumerable<string> Validate();
    }
}
