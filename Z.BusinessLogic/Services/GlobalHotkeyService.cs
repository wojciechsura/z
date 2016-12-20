using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Common;
using Z.BusinessLogic.Infrastructure;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.BusinessLogic.Services
{
    class GlobalHotkeyService : IGlobalHotkeyService
    {
        private readonly HotkeyService hotkeyService;
        private readonly IConfigurationService configurationService;

        private bool hotkeyRegistered;
        private int? hotkeyId;

        private void OnHotkeyHit()
        {
            if (HotkeyHit != null)
                HotkeyHit(this, EventArgs.Empty);
        }

        private void HandleHotkeyHit()
        {
            OnHotkeyHit();
        }

        private void HandleConfigurationChanged(object sender, EventArgs e)
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

        public GlobalHotkeyService(IConfigurationService configurationService)
        {
            hotkeyService = new HotkeyService();

            this.configurationService = configurationService;
            configurationService.ConfigurationChanged += HandleConfigurationChanged;
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

        public bool HotkeyRegistered
        {
            get
            {
                return hotkeyRegistered;
            }
        }

        public event EventHandler HotkeyHit;
    }
}
