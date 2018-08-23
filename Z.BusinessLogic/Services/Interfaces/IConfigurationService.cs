﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.Models.Configuration;

namespace Z.BusinessLogic.Services.Interfaces
{
    public interface IConfigurationService
    {
        Configuration Configuration { get; }
        bool Load();
        bool Save();
        void NotifyConfigurationChanged();
    }
}
