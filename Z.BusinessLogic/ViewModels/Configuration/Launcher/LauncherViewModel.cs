using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Models.Configuration;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.ViewModels.Configuration.Base;

namespace Z.BusinessLogic.ViewModels.Configuration.Launcher
{
    public class LauncherViewModel : BaseConfigurationViewModel
    {
        public const string PAGE_DISPLAY_NAME = "Launcher";

        private readonly ObservableCollection<LauncherEntryViewModel> items;
        private readonly IConfigurationService configurationService;

        public LauncherViewModel(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            for (int i = 0; i < configurationService.Configuration.Launcher.Items.Count; i++)
            {
                var item = new LauncherEntryViewModel(configurationService.Configuration.Launcher.Items[i]);
                items.Add(item);
            }
        }

        public override void Save()
        {
            List<LauncherShortcut> resultItems = new List<LauncherShortcut>();
            
            for (int i = 0; i < items.Count; i++)
            {
                resultItems.Add(items[i].ToLauncherShortcut());
            }

            configurationService.Configuration.Launcher.Items = resultItems;
        }

        public override IEnumerable<string> Validate()
        {
            return new List<string>();
        }

        public ObservableCollection<LauncherEntryViewModel> Items => items;

        public override string DisplayName => PAGE_DISPLAY_NAME;
    }
}
