using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Z.BusinessLogic.ViewModels.Base;

namespace Z.BusinessLogic.ViewModels.Configuration.Launcher
{
    public class LauncherEntryViewModel : HierarchicalViewModel<LauncherEntryViewModel>
    {
        private readonly LauncherEntryViewModel parent;
        private readonly ObservableCollection<LauncherEntryViewModel> items = new ObservableCollection<LauncherEntryViewModel>();

        private string name;
        private string command;
        private Bitmap icon;
        private ImageSource iconSource;

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        private ImageSource GetIconSource()
        {
            if (iconSource != null)
                return iconSource;

            if (icon == null)
                return null;

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

            return iconSource;
        }

        private void HandleIconChanged()
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

        public LauncherEntryViewModel(LauncherEntryViewModel parent)
        {
            this.parent = parent;
        }

        public LauncherEntryViewModel(LauncherEntryViewModel parent, LauncherShortcut shortcut)
            : this(parent)
        {
            name = shortcut.Name;
            command = shortcut.Command;
            icon = IconFromBase64(shortcut.Base64Icon);            

            for (int i = 0; i < shortcut.SubItems.Count; i++)
            {
                var subitem = new LauncherEntryViewModel(this, shortcut.SubItems[i]);
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
                Base64Icon = IconToBase64(icon),
                SubItems = subitems
            };

            return result;
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

        public ImageSource IconSource
        {
            get => GetIconSource();
        }

        public ObservableCollection<LauncherEntryViewModel> Items => items;

        public override LauncherEntryViewModel Parent => parent;
    }
}
