using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Wpf.Types;

namespace Z.BusinessLogic.ViewModels.Configuration
{
    public interface IConfigurationWindowAccess
    {
        void CloseWindow();
        void ShowWarning(string message, string caption);
    }
}
