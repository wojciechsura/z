using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Models.Configuration;

namespace Z.BusinessLogic.ViewModels.Main
{
    public class LauncherShortcutViewModel
    {
        public LauncherShortcutViewModel(LauncherShortcut launcherShortcut)
        {
            Name = launcherShortcut.Name;

            List<LauncherShortcutViewModel> subItems = new List<LauncherShortcutViewModel>();

            for (int i = 0; i < launcherShortcut.SubItems.Count; i++)
            {
                var subItem = new LauncherShortcutViewModel(launcherShortcut.SubItems[i]);
                subItems.Add(subItem);
            }

            SubItems = subItems;
        }

        public string Name { get; }
        public IReadOnlyCollection<LauncherShortcutViewModel> SubItems { get; }
    }
}
