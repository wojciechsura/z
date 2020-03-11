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
        private readonly LauncherShortcut launcherShortcut;

        // Public methods -----------------------------------------------------

        public LauncherShortcutViewModel(LauncherShortcut launcherShortcut)
        {
            selected = false;
            LauncherShortcut = launcherShortcut ?? throw new ArgumentNullException(nameof(launcherShortcut));
        }

        // Public properties --------------------------------------------------

        public string Name => LauncherShortcut.Name;

        public LauncherShortcut LauncherShortcut { get; }

        public bool HasSubItems => LauncherShortcut.SubItems.Count > 0;
        
        public bool Selected
        {
            get => selected;
            set => Set(ref selected, () => Selected, value);
        }
    }
}
