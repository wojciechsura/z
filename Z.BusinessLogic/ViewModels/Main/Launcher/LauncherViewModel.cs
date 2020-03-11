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
    public class LauncherViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IConfigurationService configurationService;
        private readonly IMainHandler handler;
        private readonly List<LauncherRowViewModel> rows = new List<LauncherRowViewModel>();

        private ILauncherWindowAccess launcherWindowAccess;

        private bool reversed;

        // Private methods ----------------------------------------------------

        private void UpdateLauncherRoot()
        {
            rows.Clear();

            if (configurationService.Configuration.Launcher.Items != null)
            {
                var items = configurationService.Configuration.Launcher.Items
                    .Select(i => new LauncherShortcutViewModel(i))
                    .ToList();

                var row = new LauncherRowViewModel(items);
                rows.Add(row);
            }

            OnPropertyChanged(() => SelectedRow);
        }

        private void GoBack()
        {
            if (rows.Count > 1)
            {
                rows.RemoveAt(rows.Count - 1);
                OnPropertyChanged(() => SelectedRow);
            }
        }

        private void GoForward()
        {
            if (SelectedRow != null && SelectedRow.SelectedItem.HasSubItems)
            {
                var items = SelectedRow.SelectedItem.LauncherShortcut.SubItems
                    .Select(i => new LauncherShortcutViewModel(i))
                    .ToList();

                var row = new LauncherRowViewModel(items);
                rows.Add(row);

                OnPropertyChanged(() => SelectedRow);
            }
        }

        // Public methods -----------------------------------------------------

        public LauncherViewModel(IMainHandler handler, IConfigurationService configurationService)
        {
            this.handler = handler;
            this.configurationService = configurationService;

            reversed = false;
        }

        public void Init()
        {
            UpdateLauncherRoot();
        }

        public void Clear()
        {
            rows.Clear();

            OnPropertyChanged(() => SelectedRow);
        }

        // Public properties --------------------------------------------------

        public void MoveDown()
        {
            if (reversed)
                GoBack();
            else
                GoForward();
        }

        public void MoveUp()
        {
            if (reversed)
                GoForward();
            else
                GoBack();
        }

        public void MoveLeft()
        {
            if (SelectedRow != null)
                SelectedRow.SelectPrevious();
        }

        public void MoveRight()
        {
            if (SelectedRow != null)
                SelectedRow.SelectNext();
        }

        public void EnterPressed()
        {
            // TODO choose item
        }

        // Public properties --------------------------------------------------

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

        // TODO Make selected row always the last item on the list
        // TODO Handle all cases when SelectedRow is null or has no items
        public LauncherRowViewModel SelectedRow => rows?.LastOrDefault();

        public bool Reversed
        {
            get => reversed;
            set => Set(ref reversed, () => Reversed, value);
        }
    }
}
