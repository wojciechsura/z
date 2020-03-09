using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Events;
using Z.BusinessLogic.Infrastructure;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.Services.EventBus;
using Z.Common.Types;

namespace Z.BusinessLogic.Services.GlobalHotkey
{
    class GlobalHotkeyService : IGlobalHotkeyService, IEventListener<ConfigurationChangedEvent>
    {
        private readonly HotkeyService hotkeyService;
        private readonly IConfigurationService configurationService;
        private readonly IEventBus eventBus;

        private bool hotkeyRegistered;
        private int? hotkeyId;

        private void OnHotkeyHit()
        {
            eventBus.Send(new GlobalHotkeyHitEvent());
        }

        private void HandleHotkeyHit()
        {
            OnHotkeyHit();
        }

        private void HandleConfigurationChanged()
        {
            if (hotkeyId != null)
                UnregisterHotkey();

            RegisterHotkey(configurationService.Configuration.Hotkey.Key, configurationService.Configuration.Hotkey.KeyModifier);
        }

        private void UnregisterHotkey()
        {
            if (hotkeyId == null)
                throw new InvalidOperationException("Cannot unregister hotkey: none is registered!");

            hotkeyService.Unregister(hotkeyId.Value);
            hotkeyId = null;
            hotkeyRegistered = false;
        }

        public GlobalHotkeyService(IConfigurationService configurationService, IEventBus eventBus)
        {
            hotkeyService = new HotkeyService();

            this.configurationService = configurationService;
            this.eventBus = eventBus;
            eventBus.Register((IEventListener<ConfigurationChangedEvent>)this);

            RegisterHotkey(configurationService.Configuration.Hotkey.Key, configurationService.Configuration.Hotkey.KeyModifier);
        }

        private void RegisterHotkey(Key key, KeyModifier keyModifier)
        {
            if (hotkeyId != null)
                throw new InvalidOperationException("Cannot register hotkey: one is already registered!");

            int id = 0;
            if (hotkeyService.Register(configurationService.Configuration.Hotkey.Key, configurationService.Configuration.Hotkey.KeyModifier, HandleHotkeyHit, ref id))
            {
                hotkeyRegistered = true;
                hotkeyId = id;
            }
            else
            {
                hotkeyRegistered = false;
                hotkeyId = null;
            }
        }

        void IEventListener<ConfigurationChangedEvent>.Receive(ConfigurationChangedEvent @event)
        {
            HandleConfigurationChanged();
        }

        public bool HotkeyRegistered => hotkeyRegistered;
    }
}
