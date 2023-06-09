﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Models.Configuration;

namespace Z.BusinessLogic.Services.Config
{
    public interface IConfigurationService
    {
        Configuration Configuration { get; }
        bool Load();
        bool Save();
        void NotifyConfigurationChanged();
    }
}
