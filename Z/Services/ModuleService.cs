using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Common.Interfaces;
using Z.Services.Interfaces;

namespace Z.Services
{
    class ModuleService : IModuleService
    {
        private List<IZModule> modules;

        private void InitDefaultModules()
        {
            modules.Add(new WebSearchModule.Module());
        }

        public ModuleService()
        {
            modules = new List<IZModule>();
            InitDefaultModules();
        }

        public IZModule GetModule(int index)
        {
            return modules[index];
        }

        public IZModule GetModule(string internalName)
        {
            return modules
                .SingleOrDefault(m => m.InternalName == internalName);
        }

        public int ModuleCount
        {
            get
            {
                return modules.Count;
            }
        }
    }
}
