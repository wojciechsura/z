using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.BusinessLogic.Common
{
    // Commented values are valid (in terms of WinAPI), but not used in application.

    [Flags]
    public enum KeyModifier
    {
        [Description("None")]
        None = 0x0000,
        [Description("Alt")]
        Alt = 0x0001,
        [Description("Control")]
        Ctrl = 0x0002,
        // NoRepeat = 0x4000,
        [Description("Shift")]
        Shift = 0x0004,
        // Win = 0x0008
    }
}
