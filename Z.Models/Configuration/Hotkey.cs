using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Common;

namespace Z.Models.Configuration
{
    public class Hotkey
    {
        public KeyModifier KeyModifier { get; set; } = KeyModifier.Alt;
        public Key Key { get; set; } = Key.Space;
    }
}
