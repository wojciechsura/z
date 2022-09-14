using DesktopModule.Resources;
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
    public class Module : BaseFilesystemShortcutModule
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

        protected override List<(string path, bool recursive)> GetShortcutDirectories()
        {
            return new List<(string path, bool recursive)>
            {
                (Environment.GetFolderPath(Environment.SpecialFolder.Desktop), false),
                (Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), false)
            };
        }

        protected override string FormatError(string errorText)
        {
            return string.Format(Resources.Strings.Desktop_CannotExecute, errorText);
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
