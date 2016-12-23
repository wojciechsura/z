using IWshRuntimeLibrary;
using ShortcutModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.Api.Interfaces;
using Z.Api.Types;


namespace StartMenuModule
{
    public class Module : BaseShortcutModule
    {
        private const string MODULE_DISPLAY_NAME = "Start menu";
        private const string MODULE_NAME = "StartMenu";

        private const string ACTION_KEYWORD = "start";
        private const string ACTION_NAME = "Start";
        private const string ACTION_DISPLAY = "Start menu";
        private const string ACTION_COMMENT = "Browse through Start Menu entries";

        private readonly ImageSource icon;

        protected override List<ShortcutInfo> LoadShortcuts()
        {
            string startMenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            string commonStartMenu = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);

            return LoadShortcutsFrom(startMenu, true)
                .Union(LoadShortcutsFrom(commonStartMenu, true))
                .ToList();
        }

        // Protected properties -----------------------------------------------

        protected override string ActionKeyword => ACTION_KEYWORD;
        protected override string ActionName => ACTION_NAME;
        protected override string ActionDisplay => ACTION_DISPLAY;
        protected override string ActionComment => ACTION_COMMENT;

        // Public methods -----------------------------------------------------

        public Module()
        {
            LoadShortcuts();
            icon = new BitmapImage(new Uri("pack://application:,,,/StartMenuModule;component/Resources/winlogo.png"));
        }

        // Public properties --------------------------------------------------

        public override string DisplayName => MODULE_DISPLAY_NAME;

        public override string Name => MODULE_NAME;

        public override ImageSource Icon => icon;
    }
}
