using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Common.Interfaces;

namespace Z.Services.Interfaces
{
    public interface IModuleService
    {
        IZModule GetModule(int index);
        IZModule GetModule(string internalName);
        int ModuleCount { get; }
    }
}
