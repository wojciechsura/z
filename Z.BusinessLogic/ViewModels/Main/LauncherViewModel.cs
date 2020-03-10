using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Events;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.Services.EventBus;

namespace Z.BusinessLogic.ViewModels.Main
{
    public class LauncherViewModel : IEventListener<ConfigurationChangedEvent>
    {
        private readonly IConfigurationService configurationService;
        private readonly IMainHandler handler;

        private ILauncherWindowAccess launcherWindowAccess;

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

        public LauncherViewModel(IMainHandler handler, IConfigurationService configurationService)
        {
            this.handler = handler;
            this.configurationService = configurationService;

            UpdateLauncherRoot();
        }

        public ILauncherWindowAccess LauncherWindowAccess
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (launcherWindowAccess != null)
                    throw new InvalidOperationException("Access can be set only once!");

                launcherWindowAccess = value;
            }
        }

        internal void MoveDown()
        {
            throw new NotImplementedException();
        }

        internal void EnterPressed()
        {
            throw new NotImplementedException();
        }

        internal void SpacePressed()
        {
            throw new NotImplementedException();
        }

        internal void MoveUp()
        {
            throw new NotImplementedException();
        }
    }
}
