using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Events;
using Z.BusinessLogic.Services.Interfaces;
using Z.BusinessLogic.ViewModels.Interfaces;
using Z.Wpf.Types;

namespace Z.BusinessLogic.ViewModels
{
    public class AppViewModel : IEventListener<GlobalHotkeyHitEvent>
    {
        private readonly IWindowService windowService;
        private IApplicationAccess applicationAccess;
        private readonly IConfigurationService configurationService;
        private readonly IEventBus eventBus;
        private readonly IGlobalHotkeyService globalHotkeyService;

        private void HandleTaskbarClick()
        {
            windowService.ShowMainWindow();
        }

        public void Shutdown()
        {
            eventBus.Send(new ShuttingDownEvent());

            configurationService.Save();

            applicationAccess.Shutdown();
        }

        void IEventListener<GlobalHotkeyHitEvent>.Receive(GlobalHotkeyHitEvent @event)
        {
            if (configurationService.Configuration.Hotkey.HotkeySwitchesVisibility)
                windowService.ToggleMainWindow();
            else
                windowService.ShowMainWindow();
        }

        public AppViewModel(IWindowService windowService, IConfigurationService configurationService, IEventBus eventBus, IGlobalHotkeyService globalHotkeyService)
        {
            this.windowService = windowService;
            this.configurationService = configurationService;
            this.eventBus = eventBus;
            this.globalHotkeyService = globalHotkeyService;

            eventBus.Register((IEventListener<GlobalHotkeyHitEvent>)this);

            TaskbarClickCommand = new SimpleCommand((obj) => HandleTaskbarClick());
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
