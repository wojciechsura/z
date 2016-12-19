using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Types;

namespace Z.BusinessLogic.Interfaces
{
    public interface IConfigurationWindowAccess
    {
        void OpenScreen(ConfigurationScreen screen);
    }
}
