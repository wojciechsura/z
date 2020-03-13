using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
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

namespace Z.BusinessLogic.ViewModels.Configuration.Launcher
{
    public class LauncherEntryViewModel : HierarchicalViewModel<LauncherEntryViewModel>
    {
        private readonly LauncherEntryViewModel parent;
        private readonly IImageResources imageResources;
        private readonly ObservableCollection<LauncherEntryViewModel> items = new ObservableCollection<LauncherEntryViewModel>();

        private string name;
        private string command;
        private Bitmap icon;
        private ImageSource iconSource;
        private bool isExpanded;
        private IconMode iconMode;

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        private ImageSource GetIconSource()
        {
            if (iconSource != null)
                return iconSource;

            switch (iconMode)
            {
                case IconMode.Default:
                    iconSource = imageResources.GetIconByName("LauncherGeneric32.png");
                    break;
                case IconMode.Folder:
                    iconSource = imageResources.GetIconByName("Folder32.png");
                    break;
                case IconMode.Custom:
                    {
                        if (icon == null)
                        {
                            iconSource = null;
                            break;
                        }

                        IntPtr handle = IntPtr.Zero;
                        try
                        {
                            handle = icon.GetHbitmap();
                            iconSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        }
                        finally
                        {
                            DeleteObject(handle);
                        }

                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported icon mode!");
            }

            return iconSource;
        }

        private void HandleIconChanged()
        {
            IconMode = IconMode.Custom;

            iconSource = null;
            OnPropertyChanged(() => IconSource);

        }

        private void HandleIconModeChanged()
        {
            iconSource = null;
            OnPropertyChanged(() => IconSource);
        }

        private Bitmap IconFromBase64(string base64Icon)
        {
            if (string.IsNullOrEmpty(base64Icon))
                return null;

            try
            {
                var stream = new MemoryStream(Convert.FromBase64String(base64Icon));
                Bitmap bitmap = new Bitmap(stream);
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private string IconToBase64(Bitmap icon)
        {
            if (icon == null)
                return String.Empty;

            MemoryStream stream = new MemoryStream();
            icon.Save(stream, ImageFormat.Png);

            return Convert.ToBase64String(stream.ToArray());
        }

        public LauncherEntryViewModel(LauncherEntryViewModel parent, IImageResources imageResources)
        {
            this.parent = parent;
            this.imageResources = imageResources;

            isExpanded = false;
        }

        public LauncherEntryViewModel(LauncherEntryViewModel parent, IImageResources imageResources, LauncherShortcut shortcut)
            : this(parent, imageResources)
        {
            name = shortcut.Name;
            command = shortcut.Command;
            icon = IconFromBase64(shortcut.IconData);
            iconMode = shortcut.IconMode;

            for (int i = 0; i < shortcut.SubItems.Count; i++)
            {
                var subitem = new LauncherEntryViewModel(this, imageResources, shortcut.SubItems[i]);
                items.Add(subitem);
            }
        }

        public LauncherShortcut ToLauncherShortcut()
        {
            var subitems = new List<LauncherShortcut>();

            for (int i = 0; i < items.Count; i++)
            {
                var subitem = items[i].ToLauncherShortcut();
                subitems.Add(subitem);
            }

            var result = new LauncherShortcut()
            {
                Name = this.name,
                Command = this.command,
                IconData = IconToBase64(icon),
                IconMode = iconMode,
                SubItems = subitems
            };

            return result;
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set => Set(ref isExpanded, () => IsExpanded, value);
        }

        public string Name
        {
            get => name;
            set => Set(ref name, () => Name, value);
        }

        public string Command
        {
            get => command;
            set => Set(ref command, () => Command, value);
        }

        public Bitmap Icon
        {
            get => icon;
            set => Set(ref icon, () => Icon, value, HandleIconChanged);
        }

        public IconMode IconMode
        {
            get => iconMode;
            set => Set(ref iconMode, () => IconMode, value, HandleIconModeChanged);
        }

        public ImageSource IconSource
        {
            get => GetIconSource();
        }

        public ObservableCollection<LauncherEntryViewModel> Items => items;

        public override LauncherEntryViewModel Parent => parent;
    }
}
