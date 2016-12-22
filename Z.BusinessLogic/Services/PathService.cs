using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.BusinessLogic.Services
{
    class PathService : IPathService
    {
        public string GetConfigDirectory()
        {
            string localRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!localRoaming.EndsWith("\\"))
                localRoaming += '\\';

            string result = $"{localRoaming}Spooksoft\\Z\\";
            Directory.CreateDirectory(result);
            return result;
        }

        public string GetModuleConfigDirectory(string moduleName)
        {          
            string configDir = GetConfigDirectory();
            string result = Path.Combine(configDir, $"Modules\\{moduleName}");

            Directory.CreateDirectory(result);
            return result;
        }
    }
}
