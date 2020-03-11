using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Models.Configuration;
using Z.BusinessLogic.ViewModels.Base;

namespace Z.BusinessLogic.ViewModels.Configuration.Launcher
{
    public class LauncherEntryViewModel : BaseViewModel
    {
        private string name;
        private readonly ObservableCollection<LauncherEntryViewModel> items = new ObservableCollection<LauncherEntryViewModel>();

        public LauncherEntryViewModel(LauncherShortcut shortcut)
        {
            name = shortcut.Name;

            for (int i = 0; i < shortcut.SubItems.Count; i++)
            {
                var subitem = new LauncherEntryViewModel(shortcut.SubItems[i]);
                items.Add(subitem);
            }
        }

        public LauncherShortcut ToLauncherShortcut()
        {
            var subitems = new List<LauncherShortcut>();

            for (int i = 0; i < items.Count; i++)
            {
                var subitem = items[i].ToLauncherShortcut();
                subitems.Add(subitem);
            }

            var result = new LauncherShortcut()
            {
                Name = this.name,
                SubItems = subitems
            };

            return result;
        }

        public string Name
        {
            get => name;
            set => Set(ref name, () => Name, value);
        }

        public ObservableCollection<LauncherEntryViewModel> Items => items;
    }
}
