using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Models.Configuration;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.Services.Dialogs;
using Z.BusinessLogic.ViewModels.Configuration.Base;
using Z.Wpf.Types;

namespace Z.BusinessLogic.ViewModels.Configuration.Launcher
{
    public class LauncherViewModel : BaseConfigurationViewModel
    {
        public const string PAGE_DISPLAY_NAME = "Launcher";

        private readonly ObservableCollection<LauncherEntryViewModel> items = new ObservableCollection<LauncherEntryViewModel>();
        private readonly IConfigurationService configurationService;
        private readonly IDialogService dialogService;
        private LauncherEntryViewModel selectedItem;

        private void DoChoosePath()
        {
            var result = dialogService.ShowOpenDialog("Applications (*.exe)|*.exe|All files (*.*)|*.*", "Choose application");
            if (result.Result)
            {
                selectedItem.Command = result.FileName;
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
            var newItem = new LauncherEntryViewModel(selectedItem);

            if (selectedItem == null)
                items.Add(newItem);
            else
                selectedItem.Items.Add(newItem);
        }

        private void DoAddSameLevel()
        {
            var parent = selectedItem?.Parent;
            var newItem = new LauncherEntryViewModel(parent);

            if (parent == null)
            {
                items.Add(newItem);
            }
            else
            {
                parent.Items.Add(newItem);
            }
        }


        public LauncherViewModel(IConfigurationService configurationService, IDialogService dialogService)
        {
            this.configurationService = configurationService;
            this.dialogService = dialogService;

            for (int i = 0; i < configurationService.Configuration.Launcher.Items.Count; i++)
            {
                var item = new LauncherEntryViewModel(null, configurationService.Configuration.Launcher.Items[i]);
                items.Add(item);
            }

            selectedItem = items.FirstOrDefault();

            ChoosePathCommand = new SimpleCommand(obj => DoChoosePath());

            AddSameLevelCommand = new SimpleCommand(obj => DoAddSameLevel());
            AddChildCommand = new SimpleCommand(obj => DoAddChild());
            DeleteCommand = new SimpleCommand(obj => DoDelete());
            MoveUpCommand = new SimpleCommand(obj => DoMoveUp());
            MoveDownCommand = new SimpleCommand(obj => DoMoveDown());
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

        public override string DisplayName => PAGE_DISPLAY_NAME;

        public ICommand ChoosePathCommand { get; }

        public ICommand AddSameLevelCommand { get; }
        public ICommand AddChildCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }

        public LauncherEntryViewModel SelectedItem
        {
            get => selectedItem;
            set => Set(ref selectedItem, () => SelectedItem, value);
        }
    }
}
