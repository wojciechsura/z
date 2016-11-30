﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.Common;

namespace Z.Services.Interfaces
{
    public interface IHotkeyService
    {
        bool Register(Key key, KeyModifier modifier, Action action, ref int id);
        void Unregister(Key key, KeyModifier modifier);
    }
}
