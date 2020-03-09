using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Events;
using Z.BusinessLogic.Services.AppWindows;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.Services.EventBus;
using Z.BusinessLogic.Services.GlobalHotkey;
using Z.Wpf.Types;

namespace Z.BusinessLogic.ViewModels.Application
{
    public class AppViewModel : IEventListener<GlobalHotkeyHitEvent>
    {
        private readonly IAppWindowService windowService;
        private IApplicationAccess applicationAccess;
        private readonly IConfigurationService configurationService;
        private readonly IEventBus eventBus;
        private readonly IGlobalHotkeyService globalHotkeyService;

        private void HandleTaskbarClick()
        {
            windowService.ShowMainWindow();
        }

        private void SaveConfigOnShutdown()
        {
            eventBus.Send(new ShuttingDownEvent());

            configurationService.Save();
        }

        private void HandleSessionEnding(object sender, SessionEndingEventArgs e)
        {
            SaveConfigOnShutdown();
        }

        public void Shutdown()
        {
            SaveConfigOnShutdown();
            SystemEvents.SessionEnding -= HandleSessionEnding;

            applicationAccess.Shutdown();
        }

        void IEventListener<GlobalHotkeyHitEvent>.Receive(GlobalHotkeyHitEvent @event)
        {
            if (configurationService.Configuration.Hotkey.HotkeySwitchesVisibility)
                windowService.ToggleMainWindow();
            else
                windowService.ShowMainWindow();
        }

        public AppViewModel(IAppWindowService windowService, IConfigurationService configurationService, IEventBus eventBus, IGlobalHotkeyService globalHotkeyService)
        {
            this.windowService = windowService;
            this.configurationService = configurationService;
            this.eventBus = eventBus;
            this.globalHotkeyService = globalHotkeyService;

            eventBus.Register((IEventListener<GlobalHotkeyHitEvent>)this);

            TaskbarClickCommand = new SimpleCommand((obj) => HandleTaskbarClick());

            SystemEvents.SessionEnding += HandleSessionEnding;
        }

        public ICommand TaskbarClickCommand { get; private set; }

        public IApplicationAccess ApplicationAccess
        {
            set
            {
                if (applicationAccess != null)
                    throw new InvalidOperationException("ApplicationAccess can only be set once!");
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                applicationAccess = value;
            }
        }
    }
}
