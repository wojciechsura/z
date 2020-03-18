using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Models.Configuration;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.Services.Dialogs;
using Z.BusinessLogic.Services.Image;
using Z.BusinessLogic.Services.Messaging;
using Z.BusinessLogic.Types.Launcher;
using Z.BusinessLogic.ViewModels.Configuration.Base;
using Z.Resources;
using Z.Wpf.Types;

namespace Z.BusinessLogic.ViewModels.Configuration.Launcher
{
    public class LauncherViewModel : BaseConfigurationViewModel
    {
        private readonly ObservableCollection<LauncherEntryViewModel> items = new ObservableCollection<LauncherEntryViewModel>();
        private readonly IConfigurationService configurationService;
        private readonly IDialogService dialogService;
        private readonly IMessagingService messagingService;
        private readonly IImageResources imageResources;
        private LauncherEntryViewModel selectedItem;

        private void DoChoosePath()
        {
            var result = dialogService.ShowOpenDialog(Strings.Z_Filter_Application, Strings.Z_DialogTitle_ChooseApplication);
            if (result.Result)
            {
                if (result.FileName.Contains(" "))
                {
                    selectedItem.Command = $"\"{result.FileName}\"";
                }
                else
                {
                    selectedItem.Command = result.FileName;
                }
                
                // Resolve icon if no icon is set yet.
                if (selectedItem.Icon == null)
                    TryAutoResolveIcon();
            }
        }

        private bool TryAutoResolveIcon()
        {
            // Check if command is an URL
            if (selectedItem.Command.ToLower().StartsWith("http"))
            {
                selectedItem.IconMode = IconMode.Url;
                return true;
            }

            try
            {
                string path = null;

                // Checking for command in form "<path>" param1 param2 ...
                if (selectedItem.Command.StartsWith("\""))
                {
                    int closingQuote = selectedItem.Command.IndexOf('"', 1);
                    if (closingQuote > 0)
                    {
                        path = selectedItem.Command.Substring(1, closingQuote - 1);
                    }
                }

                // Otherwise treat whole command as path
                if (path == null)
                    path = selectedItem.Command;

                var icon = Icon.ExtractAssociatedIcon(path);
                Bitmap bitmap = icon.ToBitmap();

                selectedItem.Icon = bitmap;
                selectedItem.IconMode = IconMode.Custom;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void DoAutoResolveIcon()
        {
            if (!TryAutoResolveIcon())
                messagingService.Warn(Strings.Z_Message_CouldNotResolveIconAutomatically);            
        }

        private void DoBrowseIcon()
        {
            var result = dialogService.ShowOpenDialog(Strings.Z_Filter_IconFiles, Strings.Z_DialogTitle_ChooseIcon);
            if (result.Result)
            {
                switch (System.IO.Path.GetExtension(result.FileName).ToLower())
                {
                    case ".exe":

                        try
                        {
                            Icon icon = Icon.ExtractAssociatedIcon(result.FileName);
                            Bitmap bitmap = icon.ToBitmap();

                            selectedItem.Icon = bitmap;
                            selectedItem.IconMode = IconMode.Custom;
                        }
                        catch
                        {
                            messagingService.Warn(Strings.Z_Message_FailedToExtractIconFromSelectedFile);
                        }

                        break;
                    case ".ico":

                        try
                        {
                            Icon icon = new Icon(result.FileName, 32, 32);
                            Bitmap bitmap = icon.ToBitmap();

                            selectedItem.Icon = bitmap;
                            selectedItem.IconMode = IconMode.Custom;
                        }
                        catch
                        {
                            messagingService.Warn(Strings.Z_Message_FailedToExtractIconFromSelectedFile);
                        }

                        break;
                    default:
                        messagingService.Warn(Strings.Z_Message_UnsupportedFileType);
                        break;
                }
            }
        }

        private void DoMoveDown()
        {
            var parent = selectedItem.Parent;
            if (parent == null)
            {
                // Main list
                var index = items.IndexOf(selectedItem);
                if (index < items.Count - 1)
                    items.Move(index, index + 1);
            }
            else
            {
                var index = parent.Items.IndexOf(selectedItem);
                if (index < parent.Items.Count - 1)
                    parent.Items.Move(index, index + 1);
            }
        }

        private void DoMoveUp()
        {
            var parent = selectedItem.Parent;
            if (parent == null)
            {
                // Main list
                var index = items.IndexOf(selectedItem);
                if (index > 0)
                    items.Move(index, index - 1);
            }
            else
            {
                var index = parent.Items.IndexOf(selectedItem);
                if (index > 0)
                    parent.Items.Move(index, index - 1);
            }
        }

        private void DoDelete()
        {
            var parent = selectedItem.Parent;
            if (parent == null)
            {
                items.Remove(selectedItem);
            }
            else
            {
                parent.Items.Remove(selectedItem);
            }
        }

        private void DoAddChild()
        {
            var newItem = new LauncherEntryViewModel(selectedItem, imageResources);

            if (selectedItem == null)
                items.Add(newItem);
            else
            {
                selectedItem.Items.Add(newItem);
                selectedItem.IsExpanded = true;

                if (selectedItem.IconMode == IconMode.Default)
                    selectedItem.IconMode = IconMode.Folder;
            }
        }

        private void DoAddSameLevel()
        {
            var parent = selectedItem?.Parent;
            var newItem = new LauncherEntryViewModel(parent, imageResources);

            if (parent == null)
            {
                items.Add(newItem);
            }
            else
            {
                parent.Items.Add(newItem);
            }
        }

        public LauncherViewModel(IConfigurationService configurationService,
            IDialogService dialogService,
            IMessagingService messagingService,
            IImageResources imageResources)
        {
            this.configurationService = configurationService;
            this.dialogService = dialogService;
            this.messagingService = messagingService;
            this.imageResources = imageResources;

            for (int i = 0; i < configurationService.Configuration.Launcher.Items.Count; i++)
            {
                var item = new LauncherEntryViewModel(null, imageResources, configurationService.Configuration.Launcher.Items[i]);
                items.Add(item);
            }

            selectedItem = items.FirstOrDefault();

            ChoosePathCommand = new SimpleCommand(obj => DoChoosePath());

            AddSameLevelCommand = new SimpleCommand(obj => DoAddSameLevel());
            AddChildCommand = new SimpleCommand(obj => DoAddChild());
            DeleteCommand = new SimpleCommand(obj => DoDelete());
            MoveUpCommand = new SimpleCommand(obj => DoMoveUp());
            MoveDownCommand = new SimpleCommand(obj => DoMoveDown());
            AutoResolveIconCommand = new SimpleCommand(obj => DoAutoResolveIcon());
            BrowseIconCommand = new SimpleCommand(obj => DoBrowseIcon());

            IconModes = (IconMode[])Enum.GetValues(typeof(IconMode));
        }

        public override void Save()
        {
            List<LauncherShortcut> resultItems = new List<LauncherShortcut>();
            
            for (int i = 0; i < items.Count; i++)
            {
                resultItems.Add(items[i].ToLauncherShortcut());
            }

            configurationService.Configuration.Launcher.Items = resultItems;
        }

        public override IEnumerable<string> Validate()
        {
            return new List<string>();
        }

        public ObservableCollection<LauncherEntryViewModel> Items => items;

        public override string DisplayName => Strings.Z_ConfigurationPage_Launcher;

        public ICommand ChoosePathCommand { get; }

        public ICommand AddSameLevelCommand { get; }
        public ICommand AddChildCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand AutoResolveIconCommand { get; }
        public ICommand BrowseIconCommand { get; }
        public LauncherEntryViewModel SelectedItem
        {
            get => selectedItem;
            set => Set(ref selectedItem, () => SelectedItem, value);
        }

        public IconMode[] IconModes { get; }
    }
}
