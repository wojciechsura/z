using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.ViewModels.Types;

namespace Z.ViewModels.Interfaces
{
    public interface IConfigurationWindowAccess
    {
        void OpenScreen(ConfigurationScreen screen);
    }
}
