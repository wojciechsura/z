using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Common;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.BusinessLogic.Services
{
    class ConfigurationService : IConfigurationService
    {
        private Key hotkey;
        private KeyModifier hotkeyModifier;
        private bool suspendNotification;

        private void OnConfigurationChanged()
        {
            if (ConfigurationChanged != null)
                ConfigurationChanged(this, EventArgs.Empty);
        }

        private void NotifyChanged()
        {
            if (!suspendNotification)
                OnConfigurationChanged();
        } 

        public ConfigurationService()
        {
            // Defaults
            hotkey = Key.Space;
            hotkeyModifier = KeyModifier.Alt;
        }

        public void SuspendNotifications()
        {
            suspendNotification = true;
        }

        public void ResumeNotifications()
        {
            suspendNotification = false;
            OnConfigurationChanged();
        }

        public Key Hotkey { get
            {
                return hotkey;
            }
            set
            {
                hotkey = value;
                NotifyChanged();
            }
        }

        public KeyModifier HotkeyModifier
        {
            get
            {
                return hotkeyModifier;
            }
            set
            {
                hotkeyModifier = value;
                NotifyChanged();
            }
        }

        public event EventHandler ConfigurationChanged;
    }
}
