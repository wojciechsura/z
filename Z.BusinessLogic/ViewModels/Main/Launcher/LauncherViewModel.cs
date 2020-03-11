using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Events;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.Services.EventBus;
using Z.BusinessLogic.ViewModels.Base;

namespace Z.BusinessLogic.ViewModels.Main.Launcher
{
    public class LauncherViewModel : BaseViewModel, IEventListener<ConfigurationChangedEvent>
    {
        // Private fields -----------------------------------------------------

        private readonly IConfigurationService configurationService;
        private readonly IMainHandler handler;

        private readonly ObservableCollection<LauncherRowViewModel> rows = new ObservableCollection<LauncherRowViewModel>();

        private ILauncherWindowAccess launcherWindowAccess;

        private LauncherShortcutViewModel launcherRoot;

        private LauncherRowViewModel selectedRow;

        // Private methods ----------------------------------------------------

        private void UpdateLauncherRoot()
        {
            rows.Clear();

            if (configurationService.Configuration.Launcher.Root != null)
            {
                launcherRoot = new LauncherShortcutViewModel(configurationService.Configuration.Launcher.Root);

                var row = new LauncherRowViewModel(launcherRoot.SubItems);                
                rows.Add(row);

                SelectedRow = row;
            }
            else
            {
                launcherRoot = null;

                SelectedRow = null;
            }
        }

        private void HandleBeforeSelectedRowChanged()
        {
            if (selectedRow != null)
                selectedRow.Active = false;
        }

        private void HandleAfterSelectedRowChanged()
        {
            if (selectedRow != null)
                selectedRow.Active = true;
        }

        // IEventListener implementations -------------------------------------

        void IEventListener<ConfigurationChangedEvent>.Receive(ConfigurationChangedEvent @event)
        {
            UpdateLauncherRoot();
        }

        // Public methods -----------------------------------------------------

        public LauncherViewModel(IMainHandler handler, IConfigurationService configurationService)
        {
            this.handler = handler;
            this.configurationService = configurationService;

            UpdateLauncherRoot();
        }

        public ILauncherWindowAccess LauncherWindowAccess
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (launcherWindowAccess != null)
                    throw new InvalidOperationException("Access can be set only once!");

                launcherWindowAccess = value;
            }
        }

        public void MoveDown()
        {
            if (selectedRow != null)
            {
                int selectedRowIndex = rows.IndexOf(selectedRow);

                if (selectedRowIndex > 0)
                {
                    SelectedRow = rows[selectedRowIndex - 1];
                    while (rows.Count > selectedRowIndex)
                        rows.RemoveAt(rows.Count - 1);
                }
            }
        }

        public void MoveUp()
        {
            if (selectedRow.SelectedItem.SubItems.Count > 0)
            {
                var row = new LauncherRowViewModel(selectedRow.SelectedItem.SubItems);
                rows.Add(row);

                SelectedRow = row;
            }
        }

        public void MoveLeft()
        {
            if (selectedRow != null)
            {
                selectedRow.SelectPrevious();
            }
        }

        public void MoveRight()
        {
            if (selectedRow != null)
            {
                selectedRow.SelectNext();
            }
        }

        public void EnterPressed()
        {

        }

        // Public properties --------------------------------------------------

        public ObservableCollection<LauncherRowViewModel> Rows => rows;

        // TODO Make selected row always the last item on the list
        // TODO Handle all cases when SelectedRow is null or has no items
        public LauncherRowViewModel SelectedRow
        {
            get => selectedRow;
            set => Set(ref selectedRow, () => SelectedRow, value, HandleAfterSelectedRowChanged, HandleBeforeSelectedRowChanged);
        }

        public void Init()
        {
            UpdateLauncherRoot();
        }

        public void Clear()
        {
            rows.Clear();
            SelectedRow = null;
        }
    }
}
