using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Models.Configuration;

namespace Z.BusinessLogic.ViewModels.Main.Launcher
{
    public interface ILauncherHandler
    {
        void ExitLauncher();
        void ExecuteShortcut(LauncherShortcut shortcut);
    }
}
