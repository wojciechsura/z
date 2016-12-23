﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.BusinessLogic.Services.Interfaces
{
    interface IPathService
    {
        string GetConfigDirectory();
        string GetModuleConfigDirectory(string moduleName);
    }
}