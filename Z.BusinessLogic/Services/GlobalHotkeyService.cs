using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public GlobalHotkeyService(IConfigurationService configurationService)
        {
            hotkeyService = new HotkeyService();

            this.configurationService = configurationService;

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
