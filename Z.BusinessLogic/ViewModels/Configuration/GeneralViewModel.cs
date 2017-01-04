using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Services.Interfaces;
using Z.BusinessLogic.Types;
using Z.Common.Types;

namespace Z.BusinessLogic.ViewModels.Configuration
{
    public class GeneralViewModel : BaseConfigurationViewModel, INotifyPropertyChanged
    {
        // Public types -------------------------------------------------------

        public class KeyInfo
        {
            public Key Key { get; set; }
            public string Description { get; set; }
        }

        // Private constants --------------------------------------------------

        private const string PAGE_DISPLAY_NAME = "General";

        // Private fields -----------------------------------------------------

        private readonly IConfigurationService configurationService;
        private List<KeyInfo> availableKeyInfos;

        private Key key;
        private KeyModifier keyModifier;
        private bool hotkeySwitchesVisibility;

        // Private methods ----------------------------------------------------

        private bool ExtractModifier(KeyModifier modifier)
        {
            return (keyModifier & modifier) != 0;
        }

        private void SetModifier(KeyModifier modifier, bool value)
        {
            if (value)
                keyModifier |= modifier;
            else
                keyModifier &= ~modifier;
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public GeneralViewModel(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            var configuration = configurationService.Configuration;

            key = configuration.Hotkey.Key;
            keyModifier = configuration.Hotkey.KeyModifier;
            hotkeySwitchesVisibility = configuration.Hotkey.HotkeySwitchesVisibility;

            #region Available keys
            availableKeyInfos = new List<KeyInfo>
            {
                new KeyInfo { Key = System.Windows.Input.Key.Back, Description = "Backspace" },
                new KeyInfo { Key = System.Windows.Input.Key.Tab, Description = "Tab" },
                new KeyInfo { Key = System.Windows.Input.Key.Enter, Description = "Enter" },
                new KeyInfo { Key = System.Windows.Input.Key.CapsLock, Description = "CapsLock" },
                new KeyInfo { Key = System.Windows.Input.Key.Escape, Description = "Escape" },
                new KeyInfo { Key = System.Windows.Input.Key.Space, Description = "Space" },
                new KeyInfo { Key = System.Windows.Input.Key.PageUp, Description = "PageUp" },
                new KeyInfo { Key = System.Windows.Input.Key.PageDown, Description = "PageDown" },
                new KeyInfo { Key = System.Windows.Input.Key.End, Description = "End" },
                new KeyInfo { Key = System.Windows.Input.Key.Home, Description = "Home" },
                new KeyInfo { Key = System.Windows.Input.Key.Left, Description = "Left" },
                new KeyInfo { Key = System.Windows.Input.Key.Up, Description = "Up" },
                new KeyInfo { Key = System.Windows.Input.Key.Right, Description = "Right" },
                new KeyInfo { Key = System.Windows.Input.Key.Down, Description = "Down" },
                new KeyInfo { Key = System.Windows.Input.Key.Insert, Description = "Insert" },
                new KeyInfo { Key = System.Windows.Input.Key.Delete, Description = "Delete" },
                new KeyInfo { Key = System.Windows.Input.Key.D0, Description = "0" },
                new KeyInfo { Key = System.Windows.Input.Key.D1, Description = "1" },
                new KeyInfo { Key = System.Windows.Input.Key.D2, Description = "2" },
                new KeyInfo { Key = System.Windows.Input.Key.D3, Description = "3" },
                new KeyInfo { Key = System.Windows.Input.Key.D4, Description = "4" },
                new KeyInfo { Key = System.Windows.Input.Key.D5, Description = "5" },
                new KeyInfo { Key = System.Windows.Input.Key.D6, Description = "6" },
                new KeyInfo { Key = System.Windows.Input.Key.D7, Description = "7" },
                new KeyInfo { Key = System.Windows.Input.Key.D8, Description = "8" },
                new KeyInfo { Key = System.Windows.Input.Key.D9, Description = "9" },
                new KeyInfo { Key = System.Windows.Input.Key.A, Description = "A" },
                new KeyInfo { Key = System.Windows.Input.Key.B, Description = "B" },
                new KeyInfo { Key = System.Windows.Input.Key.C, Description = "C" },
                new KeyInfo { Key = System.Windows.Input.Key.D, Description = "D" },
                new KeyInfo { Key = System.Windows.Input.Key.E, Description = "E" },
                new KeyInfo { Key = System.Windows.Input.Key.F, Description = "F" },
                new KeyInfo { Key = System.Windows.Input.Key.G, Description = "G" },
                new KeyInfo { Key = System.Windows.Input.Key.H, Description = "H" },
                new KeyInfo { Key = System.Windows.Input.Key.I, Description = "I" },
                new KeyInfo { Key = System.Windows.Input.Key.J, Description = "J" },
                new KeyInfo { Key = System.Windows.Input.Key.K, Description = "K" },
                new KeyInfo { Key = System.Windows.Input.Key.L, Description = "L" },
                new KeyInfo { Key = System.Windows.Input.Key.M, Description = "M" },
                new KeyInfo { Key = System.Windows.Input.Key.N, Description = "N" },
                new KeyInfo { Key = System.Windows.Input.Key.O, Description = "O" },
                new KeyInfo { Key = System.Windows.Input.Key.P, Description = "P" },
                new KeyInfo { Key = System.Windows.Input.Key.Q, Description = "Q" },
                new KeyInfo { Key = System.Windows.Input.Key.R, Description = "R" },
                new KeyInfo { Key = System.Windows.Input.Key.S, Description = "S" },
                new KeyInfo { Key = System.Windows.Input.Key.T, Description = "T" },
                new KeyInfo { Key = System.Windows.Input.Key.U, Description = "U" },
                new KeyInfo { Key = System.Windows.Input.Key.V, Description = "V" },
                new KeyInfo { Key = System.Windows.Input.Key.W, Description = "W" },
                new KeyInfo { Key = System.Windows.Input.Key.X, Description = "X" },
                new KeyInfo { Key = System.Windows.Input.Key.Y, Description = "Y" },
                new KeyInfo { Key = System.Windows.Input.Key.Z, Description = "Z" },
                new KeyInfo { Key = System.Windows.Input.Key.NumPad0, Description = "NumPad 0" },
                new KeyInfo { Key = System.Windows.Input.Key.NumPad1, Description = "NumPad 1" },
                new KeyInfo { Key = System.Windows.Input.Key.NumPad2, Description = "NumPad 2" },
                new KeyInfo { Key = System.Windows.Input.Key.NumPad3, Description = "NumPad 3" },
                new KeyInfo { Key = System.Windows.Input.Key.NumPad4, Description = "NumPad 4" },
                new KeyInfo { Key = System.Windows.Input.Key.NumPad5, Description = "NumPad 5" },
                new KeyInfo { Key = System.Windows.Input.Key.NumPad6, Description = "NumPad 6" },
                new KeyInfo { Key = System.Windows.Input.Key.NumPad7, Description = "NumPad 7" },
                new KeyInfo { Key = System.Windows.Input.Key.NumPad8, Description = "NumPad 8" },
                new KeyInfo { Key = System.Windows.Input.Key.NumPad9, Description = "NumPad 9" },
                new KeyInfo { Key = System.Windows.Input.Key.Multiply, Description = "Multiply" },
                new KeyInfo { Key = System.Windows.Input.Key.Add, Description = "Add" },
                new KeyInfo { Key = System.Windows.Input.Key.Separator, Description = "Separator" },
                new KeyInfo { Key = System.Windows.Input.Key.Subtract, Description = "Subtract" },
                new KeyInfo { Key = System.Windows.Input.Key.Decimal, Description = "Decimal" },
                new KeyInfo { Key = System.Windows.Input.Key.Divide, Description = "Divide" },
                new KeyInfo { Key = System.Windows.Input.Key.F1, Description = "F1" },
                new KeyInfo { Key = System.Windows.Input.Key.F2, Description = "F2" },
                new KeyInfo { Key = System.Windows.Input.Key.F3, Description = "F3" },
                new KeyInfo { Key = System.Windows.Input.Key.F4, Description = "F4" },
                new KeyInfo { Key = System.Windows.Input.Key.F5, Description = "F5" },
                new KeyInfo { Key = System.Windows.Input.Key.F6, Description = "F6" },
                new KeyInfo { Key = System.Windows.Input.Key.F7, Description = "F7" },
                new KeyInfo { Key = System.Windows.Input.Key.F8, Description = "F8" },
                new KeyInfo { Key = System.Windows.Input.Key.F9, Description = "F9" },
                new KeyInfo { Key = System.Windows.Input.Key.F10, Description = "F10" },
                new KeyInfo { Key = System.Windows.Input.Key.F11, Description = "F11" },
                new KeyInfo { Key = System.Windows.Input.Key.F12, Description = "F12" }
            };
            #endregion
        }

        public override void Save()
        {
            var configuration = configurationService.Configuration;
            configuration.Hotkey.Key = key;
            configuration.Hotkey.KeyModifier = keyModifier;
            configuration.Hotkey.HotkeySwitchesVisibility = hotkeySwitchesVisibility;
        }

        public override IEnumerable<string> Validate()
        {
            return null;
        }

        // Public properties --------------------------------------------------

        public override string DisplayName
        {
            get
            {
                return PAGE_DISPLAY_NAME;
            }
        }

        public bool ShiftModifier
        {
            get
            {
                return ExtractModifier(KeyModifier.Shift);
            }
            set
            {
                SetModifier(KeyModifier.Shift, value);
            }
        }

        public bool ControlModifier
        {
            get
            {
                return ExtractModifier(KeyModifier.Ctrl);
            }
            set
            {
                SetModifier(KeyModifier.Ctrl, value);
            }
        }

        public bool AltModifier
        {
            get
            {
                return ExtractModifier(KeyModifier.Alt);
            }
            set
            {
                SetModifier(KeyModifier.Alt, value);
            }
        }

        public IEnumerable<KeyInfo> Keys
        {
            get
            {
                return availableKeyInfos.OrderBy(k => k.Description);
            }
        }

        public KeyInfo Key
        {
            get
            {
                return availableKeyInfos.FirstOrDefault(k => k.Key == key);
            }
            set
            {
                key = value.Key;
            }
        }

        public bool HotkeySwitchesVisibility
        {
            get
            {
                return hotkeySwitchesVisibility;                
            }
            set
            {
                hotkeySwitchesVisibility = value;                 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
