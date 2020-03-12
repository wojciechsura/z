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
    public class LauncherEntryViewModel : HierarchicalViewModel<LauncherEntryViewModel>
    {
        private readonly LauncherEntryViewModel parent;
        private readonly ObservableCollection<LauncherEntryViewModel> items = new ObservableCollection<LauncherEntryViewModel>();

        private string name;
        private string command;

        public LauncherEntryViewModel(LauncherEntryViewModel parent)
        {
            this.parent = parent;
        }

        public LauncherEntryViewModel(LauncherEntryViewModel parent, LauncherShortcut shortcut)
            : this(parent)
        {
            name = shortcut.Name;
            command = shortcut.Command;

            for (int i = 0; i < shortcut.SubItems.Count; i++)
            {
                var subitem = new LauncherEntryViewModel(this, shortcut.SubItems[i]);
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
                Command = this.command,
                SubItems = subitems
            };

            return result;
        }

        public string Name
        {
            get => name;
            set => Set(ref name, () => Name, value);
        }

        public string Command
        {
            get => command;
            set => Set(ref command, () => Command, value);
        }

        public ObservableCollection<LauncherEntryViewModel> Items => items;

        public override LauncherEntryViewModel Parent => parent;
    }
}
