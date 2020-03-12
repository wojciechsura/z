using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.BusinessLogic.Models.Configuration;
using Z.BusinessLogic.ViewModels.Base;

namespace Z.BusinessLogic.ViewModels.Main.Launcher
{
    public class LauncherShortcutViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private bool selected;
        private bool cachedIconValid = false;
        private ImageSource cachedIcon = null;

        // Private methods ----------------------------------------------------

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        private ImageSource GetIcon()
        {
            if (cachedIconValid)
                return cachedIcon;

            if (string.IsNullOrEmpty(LauncherShortcut.Base64Icon))
            {
                cachedIcon = null;
                cachedIconValid = true;
            }
            else
            {
                try
                {
                    MemoryStream ms = new MemoryStream(Convert.FromBase64String(LauncherShortcut.Base64Icon));
                    Bitmap bitmap = new Bitmap(ms);

                    IntPtr handle = IntPtr.Zero;
                    try
                    {
                        handle = bitmap.GetHbitmap();
                        cachedIcon = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        cachedIconValid = true;
                    }
                    finally
                    {
                        DeleteObject(handle);
                    }                  
                }
                catch
                {
                    cachedIcon = null;
                    cachedIconValid = true;
                }
            }

            return cachedIcon;
        }

        // Public methods -----------------------------------------------------

        public LauncherShortcutViewModel(LauncherShortcut launcherShortcut)
        {
            selected = false;
            LauncherShortcut = launcherShortcut ?? throw new ArgumentNullException(nameof(launcherShortcut));
            cachedIcon = null;
        }

        // Public properties --------------------------------------------------

        public string Name => LauncherShortcut.Name;

        public LauncherShortcut LauncherShortcut { get; }

        public bool HasSubItems => LauncherShortcut.SubItems.Count > 0;
        
        public bool Selected
        {
            get => selected;
            set => Set(ref selected, () => Selected, value);
        }

        public ImageSource Icon => GetIcon();
    }
}
