using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Common;

namespace Z.BusinessLogic.Services.Interfaces
{
    public interface IConfigurationService
    {
        Key Hotkey { get; set; }
        KeyModifier HotkeyModifier { get; set; }

        event EventHandler ConfigurationChanged;
    }
}
