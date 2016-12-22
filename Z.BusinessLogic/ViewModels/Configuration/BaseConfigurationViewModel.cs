using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.BusinessLogic.ViewModels.Configuration
{
    public abstract class BaseConfigurationViewModel
    {
        public abstract string DisplayName { get; }
        public abstract void Save();
        public abstract IEnumerable<string> Validate();
    }
}
