﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.ViewModels.Interfaces
{
    public interface IViewModelFactory
    {
        IMainWindowViewModel GenerateMainWindowViewModel(IMainWindowAccess access);
    }
}
