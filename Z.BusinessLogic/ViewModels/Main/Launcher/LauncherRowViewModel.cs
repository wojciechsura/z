using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.ViewModels.Base;

namespace Z.BusinessLogic.ViewModels.Main.Launcher
{
    public class LauncherRowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private LauncherShortcutViewModel selectedItem;

        // Private methods ----------------------------------------------------

        private void HandleBeforeSelectedItemChanged()
        {
            if (selectedItem != null)
                selectedItem.Selected = false;
        }

        private void HandleAfterSelectedItemChanged()
        {
            if (selectedItem != null)
                selectedItem.Selected = true;
        }

        private int IndexOfItem(LauncherShortcutViewModel item)
        {
            int i = Items.Count - 1;
            while (i >= 0)
            {
                if (Items[i] != item)
                    i--;
                else
                    break;
            }

            return i;
        }

        // Public methods -----------------------------------------------------

        public LauncherRowViewModel(string header, IReadOnlyList<LauncherShortcutViewModel> items)
        {
            this.Header = header;

            Items = items ?? throw new ArgumentNullException(nameof(items));
            SelectedItem = Items.FirstOrDefault();
        }

        public void SelectPrevious()
        {
            if (selectedItem == null)
            {
                selectedItem = Items?.LastOrDefault();
            }
            else
            {
                var index = IndexOfItem(selectedItem);
                if (index > 0)
                {
                    SelectedItem = Items[index - 1];
                }
            }
        }

        public void SelectNext()
        {
            if (selectedItem == null)
            {
                selectedItem = Items?.FirstOrDefault();
            }
            else
            {
                var index = IndexOfItem(selectedItem);
                if (index < Items.Count - 1)
                {
                    SelectedItem = Items[index + 1];
                }
            }
        }

        // Public properties --------------------------------------------------

        public IReadOnlyList<LauncherShortcutViewModel> Items { get; }

        public LauncherShortcutViewModel SelectedItem
        {
            get => selectedItem;
            set => Set(ref selectedItem, () => SelectedItem, value, HandleAfterSelectedItemChanged, HandleBeforeSelectedItemChanged);
        }

        public string Header { get; }
    }
}
