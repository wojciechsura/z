using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using Z.BusinessLogic.Common;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.BusinessLogic.Services
{
    class HotkeyService : IDisposable, IHotkeyService
    {
        // Private types ------------------------------------------------------

        private class HotKey
        {
            public HotKey(Key key, KeyModifier keyModifier, Action action, int id)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                Key = key;
                Modifier = keyModifier;
                Action = action;
                Id = id;
            }

            public Key Key { get; private set; }
            public KeyModifier Modifier { get; private set; }
            public Action Action { get; private set; }
            public int Id { get; private set; }
        }

        // Public constants ---------------------------------------------------

        public const int WmHotKey = 0x0312;

        // Private fields -----------------------------------------------------

        private static Dictionary<int, HotKey> hotkeys;
        private Boolean _disposed;

        // Private methods ----------------------------------------------------

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private static void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == WmHotKey)
                {
                    HotKey hotkey;

                    if (hotkeys.TryGetValue((int)msg.wParam, out hotkey) && hotkey != null)
                    {
                        hotkey.Action?.Invoke();
                        handled = true;
                    }
                }
            }
        }

        private void InternalUnregister(int id)
        {
            UnregisterHotKey(IntPtr.Zero, id);
        }

        private bool InternalRegister(Key key, KeyModifier modifier, int id)
        {
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(key);
            bool result = RegisterHotKey(IntPtr.Zero, id, (UInt32)modifier, (UInt32)virtualKeyCode);

            // Debug.Print(result.ToString() + ", " + Id + ", " + virtualKeyCode);
            return result;
        }

        private void UnregisterAll()
        {
            while (hotkeys.FirstOrDefault().Value != null)
            {
                var pair = hotkeys.First();
                InternalUnregister(pair.Value.Id);
                hotkeys.Remove(pair.Key);
            }
        }

        private int GenerateId(Key key, KeyModifier modifier)
        {
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(key);
            return virtualKeyCode + ((int)modifier * 0x10000);
        }

        // Protected methods --------------------------------------------------

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be _disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be _disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    UnregisterAll();
                }

                // Note disposing has been done.
                _disposed = true;
            }
        }

        // Static constructor -------------------------------------------------

        static HotkeyService()
        {
            ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
            hotkeys = new Dictionary<int, HotKey>();
        }

        // Public methods -----------------------------------------------------

        public HotkeyService()
        {

        }

        public bool Register(Key key, KeyModifier modifier, Action action, ref int id)
        {
            id = GenerateId(key, modifier);

            if (hotkeys.ContainsKey(id))
                return false;

            bool result = InternalRegister(key, modifier, id);

            if (result)
                hotkeys.Add(id, new HotKey(key, modifier, action, id));

            return result;
        }

        public void Unregister(Key key, KeyModifier modifier)
        {
            int id = GenerateId(key, modifier);

            if (hotkeys.ContainsKey(id))
            {
                InternalUnregister(id);
                hotkeys.Remove(id);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
    }
}
