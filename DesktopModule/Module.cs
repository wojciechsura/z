﻿using DesktopModule.Resources;
using ShortcutModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopModule
{
    public class Module : BaseShortcutModule
    {
        private const string MODULE_NAME = "Desktop";
        private const string DESKTOP_ACTION_KEYWORD = "desktop";
        private const string DESKTOP_ACTION_NAME = "Desktop";

        // Private fields -----------------------------------------------------

        private readonly ImageSource icon;

        // Protected properties -----------------------------------------------

        protected override string ActionComment => Strings.Desktop_ActionComment;
        protected override string ActionDisplay => Strings.Desktop_ActionDisplay;
        protected override string ActionKeyword => DESKTOP_ACTION_KEYWORD;
        protected override string ActionName => DESKTOP_ACTION_NAME;

        // Protected methods --------------------------------------------------

        protected override List<ShortcutInfo> LoadShortcuts()
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string commonDesktop = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);

            return LoadShortcutsFrom(desktop, false)
                .Union(LoadShortcutsFrom(commonDesktop, false))
                .ToList();
        }

        // Public methods -----------------------------------------------------

        public Module()
        {
            icon = new BitmapImage(new Uri("pack://application:,,,/DesktopModule;component/Resources/desktop.png"));
        }

        // Public properties --------------------------------------------------

        public override string DisplayName => Strings.Desktop_ModuleDisplayName;
        public override string Name => MODULE_NAME;
        public override ImageSource Icon => icon;
    }
}
