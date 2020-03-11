using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Models.Configuration;
using Z.BusinessLogic.ViewModels.Base;

namespace Z.BusinessLogic.ViewModels.Main.Launcher
{
    public class LauncherShortcutViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private bool selected;

        // Public methods -----------------------------------------------------

        public LauncherShortcutViewModel(LauncherShortcut launcherShortcut)
        {
            selected = false;

            Name = launcherShortcut.Name;

            List<LauncherShortcutViewModel> subItems = new List<LauncherShortcutViewModel>();

            for (int i = 0; i < launcherShortcut.SubItems.Count; i++)
            {
                var subItem = new LauncherShortcutViewModel(launcherShortcut.SubItems[i]);
                subItems.Add(subItem);
            }

            SubItems = subItems;
        }

        // Public properties --------------------------------------------------

        public string Name { get; }
        
        public IReadOnlyList<LauncherShortcutViewModel> SubItems { get; }
        
        public bool Selected
        {
            get => selected;
            set => Set(ref selected, () => Selected, value);
        }
    }
}
