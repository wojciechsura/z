using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Z.BusinessLogic.Services.Image;
using Z.BusinessLogic.Types.Launcher;
using Z.BusinessLogic.ViewModels.Base;

namespace Z.BusinessLogic.ViewModels.Main.Launcher
{
    public class LauncherShortcutViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private bool selected;
        private bool cachedIconValid = false;
        private ImageSource cachedIcon = null;
        private readonly IImageResources imageResources;

        // Private methods ----------------------------------------------------

        private ImageSource GetIcon()
        {
            if (cachedIconValid)
                return cachedIcon;

            switch (LauncherShortcut.IconMode)
            {
                case IconMode.Default:
                    cachedIcon = imageResources.GetIconByName("LauncherGeneric32.png");
                    cachedIconValid = true;
                    break;
                case IconMode.Folder:
                    cachedIcon = imageResources.GetIconByName("Folder32.png");
                    cachedIconValid = true;
                    break;
                case IconMode.Url:
                    cachedIcon = imageResources.GetIconByName("Url32.png");
                    cachedIconValid = true;
                    break;
                case IconMode.Custom:
                    {
                        if (string.IsNullOrEmpty(LauncherShortcut.IconData))
                        {
                            cachedIcon = null;
                            cachedIconValid = true;
                        }
                        else
                        {
                            try
                            {
                                MemoryStream ms = new MemoryStream(Convert.FromBase64String(LauncherShortcut.IconData));

                                BitmapImage image = new BitmapImage();
                                image.BeginInit();
                                image.StreamSource = ms;
                                image.EndInit();

                                cachedIcon = image;
                            }
                            catch
                            {
                                cachedIcon = null;
                                cachedIconValid = true;
                            }
                        }

                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported icon mode!");
            }

            return cachedIcon;
        }

        // Public methods -----------------------------------------------------

        public LauncherShortcutViewModel(IImageResources imageResources, LauncherShortcut launcherShortcut)
        {
            this.imageResources = imageResources;

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
