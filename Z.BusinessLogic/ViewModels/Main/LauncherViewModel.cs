using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Events;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.BusinessLogic.ViewModels.Main
{
    public class LauncherViewModel : IEventListener<ConfigurationChangedEvent>
    {
        private readonly IConfigurationService configurationService;

        private LauncherShortcutViewModel launcherRoot;

        private void UpdateLauncherRoot()
        {
            if (configurationService.Configuration.Launcher.Root != null)
                launcherRoot = new LauncherShortcutViewModel(configurationService.Configuration.Launcher.Root);
            else
                launcherRoot = null;
        }

        void IEventListener<ConfigurationChangedEvent>.Receive(ConfigurationChangedEvent @event)
        {
            UpdateLauncherRoot();
        }

        public LauncherViewModel(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            UpdateLauncherRoot();
        }
    }
}
